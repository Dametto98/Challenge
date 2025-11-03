using MotoMap.Api.DotNet.Models;

namespace MotoMap.Api.DotNet.Services
{
    public interface IMotoService
    {
        Task<IEnumerable<Moto>> GetAllAsync(int pageNumber, int pageSize);
        Task<Moto?> GetByIdAsync(int id);
        Task<Moto> CreateAsync(Moto novaMoto);
        Task<bool> UpdateAsync(int id, Moto motoAtualizada);
        Task<bool> DeleteAsync(int id);
    }
}