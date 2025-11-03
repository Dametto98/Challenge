using Microsoft.EntityFrameworkCore;
using MotoMap.Api.DotNet.Data;
using MotoMap.Api.DotNet.Models;

namespace MotoMap.Api.DotNet.Services
{
    public class PatioService : IPatioService
    {
        private readonly MotoMapDbContext _context;

        public PatioService(MotoMapDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Patio>> GetAllAsync()
        {
            // Include(p => p.Posicoes) traz as posições junto com o pátio
            return await _context.Patios.Include(p => p.Posicoes).ToListAsync();
        }

        public async Task<Patio?> GetByIdAsync(int id)
        {
            return await _context.Patios.Include(p => p.Posicoes)
                                      .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Patio> CreateAsync(Patio patio)
        {
            _context.Patios.Add(patio);
            await _context.SaveChangesAsync();
            return patio;
        }
    }
}
