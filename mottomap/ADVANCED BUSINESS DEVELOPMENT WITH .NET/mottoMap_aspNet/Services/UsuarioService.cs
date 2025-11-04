using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MotoMap.Api.DotNet.Data;
using MotoMap.Api.DotNet.Dtos;
using MotoMap.Api.DotNet.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net; // Para encriptação de palavra-passe

namespace MotoMap.Api.DotNet.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly MotoMapDbContext _context;
        private readonly IConfiguration _configuration;

        public UsuarioService(MotoMapDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // --- MÉTODOS DE AUTENTICAÇÃO ---

        public async Task<Usuario?> RegisterAsync(UsuarioRegisterDto registerDto)
        {
            // 1. Verifica se o email já existe
            if (await _context.Usuarios.AnyAsync(u => u.Email == registerDto.Email))
            {
                return null; // Email já registado
            }

            // 2. Encripta a palavra-passe (Hashing)
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // 3. Cria o novo utilizador
            var usuario = new Usuario
            {
                Nome = registerDto.Nome,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                Perfil = registerDto.Perfil ?? "Operador"
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return usuario;
        }

        public async Task<string?> LoginAsync(UsuarioLoginDto loginDto)
        {
            // 1. Encontra o utilizador pelo email
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (usuario == null)
            {
                return null; // Utilizador não encontrado
            }

            // 2. Verifica a palavra-passe
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, usuario.PasswordHash))
            {
                return null; // Palavra-passe inválida
            }

            // 3. Gera o Token JWT
            return GenerateJwtToken(usuario);
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            // A chave deve ser longa e complexa, armazenada em appsettings.json
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("Chave JWT não configurada."));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(JwtRegisteredClaimNames.Name, usuario.Nome),
                new Claim(ClaimTypes.Role, usuario.Perfil) // Adiciona o Perfil/Role
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8), // Duração do token
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        // --- MÉTODOS CRUD PADRÃO ---

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        // Este método CreateAsync é para criação interna, se necessário.
        // O método RegisterAsync é o correto para o público.
        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            // Idealmente, este método também deveria fazer o hash da palavra-passe se ela for passada
            // Mas vamos focar no RegisterAsync
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<bool> UpdateAsync(int id, Usuario usuario)
        {
            if (id != usuario.Id) return false;

            // Não atualiza a palavra-passe por este método
            _context.Entry(usuario).State = EntityState.Modified;
            _context.Entry(usuario).Property(x => x.PasswordHash).IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Usuarios.Any(e => e.Id == id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return false;

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}