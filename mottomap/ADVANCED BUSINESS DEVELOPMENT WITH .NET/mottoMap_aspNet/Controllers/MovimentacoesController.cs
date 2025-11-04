using Microsoft.AspNetCore.Mvc;
using MotoMap.Api.DotNet.Dtos;
using MotoMap.Api.DotNet.Services;
using Microsoft.AspNetCore.Authorization;

namespace MotoMap.Api.DotNet.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class MovimentacaoController : ControllerBase
    {
        private readonly IMovimentacaoService _movimentacaoService;

        public MovimentacaoController(IMovimentacaoService movimentacaoService)
        {
            _movimentacaoService = movimentacaoService;
        }

        /// <summary>
        /// Registra a entrada de uma moto em uma posição do pátio.
        /// </summary>
        /// <remarks>
        /// Cria um evento de "ENTRADA" e um novo registro de "HistoricoPosicao" com DataFim nula.
        /// </remarks>
        /// <param name="entradaDto">Dados da moto, posição e usuário.</param>
        /// <response code="201">Entrada registrada com sucesso.</response>
        /// <response code="400">Se os dados da requisição forem inválidos.</response>
        [HttpPost("entrada")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegistrarEntrada([FromBody] MovimentacaoEntradaDto entradaDto)
        {
            var movimentacao = await _movimentacaoService.RegistrarEntradaAsync(entradaDto);
            return CreatedAtAction(nameof(RegistrarEntrada), new { id = movimentacao.Id }, movimentacao);
        }

        /// <summary>
        /// Registra a saída de uma moto de uma posição do pátio.
        /// </summary>
        /// <remarks>
        /// Cria um evento de "SAIDA" e atualiza o "HistoricoPosicao" existente, preenchendo a DataFim.
        /// </remarks>
        /// <param name="saidaDto">Dados da moto e da posição de saída.</param>
        /// <response code="200">Saída registrada com sucesso.</response>
        /// <response code="404">Se não for encontrado um registro de entrada aberto para a moto.</response>
        [HttpPost("saida")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RegistrarSaida([FromBody] MovimentacaoSaidaDto saidaDto)
        {
            var historicoFechado = await _movimentacaoService.RegistrarSaidaAsync(saidaDto);

            if (historicoFechado == null)
            {
                return NotFound("Não foi encontrado um registro de entrada aberto para esta moto.");
            }

            return Ok(historicoFechado);
        }
    }
}
