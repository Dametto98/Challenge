using Microsoft.AspNetCore.Mvc;
using MotoMap.Api.DotNet.Models;
using MotoMap.Api.DotNet.Services;
using Microsoft.AspNetCore.Authorization;

namespace MotoMap.Api.DotNet.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class HistoricoController : ControllerBase
    {
        private readonly IHistoricoService _historicoService;

        public HistoricoController(IHistoricoService historicoService)
        {
            _historicoService = historicoService;
        }

        /// <summary>
        /// Consulta todas as posições que estão atualmente ocupadas.
        /// </summary>
        /// <remarks>
        /// Retorna uma lista de registros de histórico onde a DataFim é nula.
        /// </remarks>
        /// <response code="200">Retorna a lista de posições ocupadas.</response>
        [HttpGet("posicoes/atuais")]
        [ProducesResponseType(typeof(IEnumerable<HistoricoPosicao>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPosicoesAtuais()
        {
            var posicoes = await _historicoService.GetPosicoesAtuaisAsync();
            return Ok(posicoes);
        }

        /// <summary>
        /// Lista todo o histórico de posições (passadas e atual) de uma moto específica.
        /// </summary>
        /// <param name="motoId">O ID da moto a ser consultada.</param>
        /// <response code="200">Retorna o histórico da moto.</response>
        /// <response code="404">Se a moto não tiver nenhum histórico.</response>
        [HttpGet("moto/{motoId}")]
        [ProducesResponseType(typeof(IEnumerable<HistoricoPosicao>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHistoricoPorMoto(int motoId)
        {
            var historico = await _historicoService.GetHistoricoPorMotoAsync(motoId);

            if (historico == null || !historico.Any())
            {
                return NotFound("Nenhum histórico encontrado para esta moto.");
            }

            return Ok(historico);
        }
    }
}