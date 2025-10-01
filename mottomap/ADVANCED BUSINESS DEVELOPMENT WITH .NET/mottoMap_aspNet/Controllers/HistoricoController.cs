using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoMap.Api.DotNet.Data;
using MotoMap.Api.DotNet.Models;

namespace MotoMap.Api.DotNet.Controllers
{
    /// Fornece endpoints para consultar o histórico de ocupação das posições.
    [Route("api/[controller]")]
    [ApiController]
    public class HistoricoController : ControllerBase
    {
        private readonly MotoMapDbContext _context;

        public HistoricoController(MotoMapDbContext context)
        {
            _context = context;
        }

        [HttpGet("posicoesatuais")]
        public async Task<ActionResult<IEnumerable<HistoricoPosicao>>> GetPosicoesAtuais()
        {
            // Sem o banco, isso retornará um erro em tempo de execução.
            // Em casa, funcionará.
            try
            {
                var posicoes = await _context.HistoricoPosicoes.Where(h => h.DataFim == null).ToListAsync();
                return Ok(posicoes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar no banco (esperado na FIAP sem DB): {ex.Message}");
                return StatusCode(503, "Serviço de banco de dados indisponível no momento."); // Service Unavailable
            }
        }

        [HttpGet("moto/{motoId}")]
        public async Task<ActionResult<IEnumerable<HistoricoPosicao>>> GetHistoricoPorMoto(int motoId)
        {
            try
            {
                var historico = await _context.HistoricoPosicoes
                                            .Where(h => h.MotoId == motoId)
                                            .OrderByDescending(h => h.DataInicio)
                                            .ToListAsync();
                if (!historico.Any()) return NotFound($"Nenhum histórico encontrado para a moto com ID {motoId} (DB offline/não acessível).");
                return Ok(historico);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar no banco (esperado na FIAP sem DB): {ex.Message}");
                return StatusCode(503, "Serviço de banco de dados indisponível no momento.");
            }
        }
    }
}