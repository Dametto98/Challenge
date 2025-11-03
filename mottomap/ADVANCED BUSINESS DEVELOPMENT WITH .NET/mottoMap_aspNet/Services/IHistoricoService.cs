using MotoMap.Api.DotNet.Models;

namespace MotoMap.Api.DotNet.Services
{
    public interface IHistoricoService
    {
        Task<IEnumerable<HistoricoPosicao>> GetPosicoesAtuaisAsync();
        Task<IEnumerable<HistoricoPosicao>> GetHistoricoPorMotoAsync(int motoId);
    }
}
