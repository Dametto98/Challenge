using MotoMap.Api.DotNet.Dtos; 
using MotoMap.Api.DotNet.Models;

namespace MotoMap.Api.DotNet.Services
{
    public interface IUsuarioService
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(int id);
        Task<Usuario> CreateAsync(Usuario usuario);
        Task<bool> UpdateAsync(int id, Usuario usuario);
        Task<bool> DeleteAsync(int id);

        Task<Usuario?> RegisterAsync(UsuarioRegisterDto registerDto);
        Task<string?> LoginAsync(UsuarioLoginDto loginDto);
    }
}

