using Microsoft.EntityFrameworkCore;
using MotoMap.Api.DotNet.Data;
using MotoMap.Api.DotNet.Models;

namespace MotoMap.Api.DotNet.Services
{
    public class MotoService : IMotoService
    {
        private readonly MotoMapDbContext _context;

        public MotoService(MotoMapDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Moto>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.Motos
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();
        }

        public async Task<Moto?> GetByIdAsync(int id)
        {
            return await _context.Motos.FindAsync(id);
        }

        public async Task<Moto> CreateAsync(Moto novaMoto)
        {
            _context.Motos.Add(novaMoto);
            await _context.SaveChangesAsync();
            return novaMoto;
        }

        public async Task<bool> UpdateAsync(int id, Moto motoAtualizada)
        {
            if (id != motoAtualizada.Id)
            {
                return false;
            }

            _context.Entry(motoAtualizada).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true; 
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Motos.Any(e => e.Id == id))
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
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null)
            {
                return false; 
            }

            _context.Motos.Remove(moto);
            await _context.SaveChangesAsync();
            return true; 
        }
    }
}