using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Adicione se estiver faltando
using MotoMap.Api.DotNet.Data;
using MotoMap.Api.DotNet.Dtos;
using MotoMap.Api.DotNet.Models;

namespace MotoMap.Api.DotNet.Controllers
{
    /// Gerencia o registro de entradas e saídas de motos no pátio.
    [Route("api/[controller]")]
    [ApiController]
    /// Registra a entrada de uma moto em uma posição do pátio.
    public class MovimentacoesController : ControllerBase
    {
        private readonly MotoMapDbContext _context;

        public MovimentacoesController(MotoMapDbContext context)
        {
            _context = context;
        }

        [HttpPost("entrada")]
        public async Task<IActionResult> RegistrarEntrada([FromBody] MovimentacaoEntradaDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var movimentacao = new Movimentacao
            {
                MotoId = dto.MotoId,
                UsuarioId = dto.UsuarioId,
                PosicaoId = dto.PosicaoId,
                Observacoes = dto.Observacoes,
                Tipo = TipoMovimentacao.ENTRADA,
                DataHora = DateTime.UtcNow
            };
            _context.Movimentacoes.Add(movimentacao);

            if (dto.PosicaoId.HasValue)
            {
                var historicoAnteriorAtivo = await _context.HistoricoPosicoes
                    .Where(h => h.MotoId == dto.MotoId && h.DataFim == null)
                    .FirstOrDefaultAsync();
                if (historicoAnteriorAtivo != null)
                {
                    historicoAnteriorAtivo.DataFim = DateTime.UtcNow;
                }

                var novoHistoricoPosicao = new HistoricoPosicao
                {
                    MotoId = dto.MotoId,
                    PosicaoId = dto.PosicaoId.Value,
                    DataInicio = DateTime.UtcNow
                };
                _context.HistoricoPosicoes.Add(novoHistoricoPosicao);
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar no banco (esperado na FIAP sem DB): {ex.Message}");
                return Ok(new { Message = "Tentativa de entrada registrada (DB offline/não acessível).", Movimentacao = movimentacao });
            }
            return Ok(new { Message = "Entrada registrada com sucesso.", MovimentacaoId = movimentacao.Id });
        }

        [HttpPost("saida")]
        public async Task<IActionResult> RegistrarSaida([FromBody] MovimentacaoSaidaDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var movimentacao = new Movimentacao
            {
                MotoId = dto.MotoId,
                UsuarioId = dto.UsuarioId,
                Observacoes = dto.Observacoes,
                Tipo = TipoMovimentacao.SAIDA,
                DataHora = DateTime.UtcNow
            };
            _context.Movimentacoes.Add(movimentacao);

            var historicoAtual = await _context.HistoricoPosicoes
                .Where(h => h.MotoId == dto.MotoId && h.DataFim == null)
                .FirstOrDefaultAsync();
            if (historicoAtual != null)
            {
                historicoAtual.DataFim = DateTime.UtcNow;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar no banco (esperado na FIAP sem DB): {ex.Message}");
                return Ok(new { Message = "Tentativa de saída registrada (DB offline/não acessível).", Movimentacao = movimentacao });
            }
            return Ok(new { Message = "Saída registrada com sucesso.", MovimentacaoId = movimentacao.Id });
        }
    }
}