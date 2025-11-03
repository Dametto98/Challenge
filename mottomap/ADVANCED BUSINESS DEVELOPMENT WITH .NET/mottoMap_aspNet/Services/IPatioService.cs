using MotoMap.Api.DotNet.Models;

namespace MotoMap.Api.DotNet.Services
{
    public interface IPatioService
    {
        Task<IEnumerable<Patio>> GetAllAsync();
        Task<Patio?> GetByIdAsync(int id);
        Task<Patio> CreateAsync(Patio patio);
    }
}
