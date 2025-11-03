using MotoMap.Api.DotNet.Dtos; // Assumindo que seus DTOs estão em 'Dtos'
using MotoMap.Api.DotNet.Models;

namespace MotoMap.Api.DotNet.Services
{
    public interface IMovimentacaoService
    {
        Task<Movimentacao> RegistrarEntradaAsync(MovimentacaoEntradaDto entradaDto);
        Task<HistoricoPosicao?> RegistrarSaidaAsync(MovimentacaoSaidaDto saidaDto);
    }
}
