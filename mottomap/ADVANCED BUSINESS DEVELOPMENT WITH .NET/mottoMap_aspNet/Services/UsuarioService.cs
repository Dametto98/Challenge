using Microsoft.EntityFrameworkCore;
using MotoMap.Api.DotNet.Data;
using MotoMap.Api.DotNet.Models;

namespace MotoMap.Api.DotNet.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly MotoMapDbContext _context;

        public UsuarioService(MotoMapDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            // No Passo 4 (Segurança), vamos hashear a senha aqui.
            // Por enquanto, apenas salvamos.
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<bool> UpdateAsync(int id, Usuario usuario)
        {
            if (id != usuario.Id) return false;

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                // --- INÍCIO DA CORREÇÃO ---
                // O código foi reescrito para evitar o erro CS1525
                if (!_context.Usuarios.Any(e => e.Id == id))
                {
                    return false; // Usuário não foi encontrado
                }
                else
                {
                    throw; // Lança a exceção original
                }
                // --- FIM DA CORREÇÃO ---
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

