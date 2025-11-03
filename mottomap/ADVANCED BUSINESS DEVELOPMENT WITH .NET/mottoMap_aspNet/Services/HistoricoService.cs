using Microsoft.EntityFrameworkCore;
using MotoMap.Api.DotNet.Data;
using MotoMap.Api.DotNet.Models;

namespace MotoMap.Api.DotNet.Services
{
    public class HistoricoService : IHistoricoService
    {
        private readonly MotoMapDbContext _context;

        public HistoricoService(MotoMapDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HistoricoPosicao>> GetPosicoesAtuaisAsync()
        {
            // Busca todos os históricos onde a moto ainda está (DataFim == null)
            return await _context.HistoricoPosicoes
                .Where(h => h.DataFim == null)
                .ToListAsync();
        }

        public async Task<IEnumerable<HistoricoPosicao>> GetHistoricoPorMotoAsync(int motoId)
        {
            // Busca todos os registros (passados e presente) de uma moto
            return await _context.HistoricoPosicoes
                .Where(h => h.MotoId == motoId)
                .ToListAsync();
        }
    }
}
