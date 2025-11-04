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
    public class PatioController : ControllerBase
    {
        private readonly IPatioService _patioService;

        public PatioController(IPatioService patioService)
        {
            _patioService = patioService;
        }

        /// <summary>
        /// Lista todos os pátios e suas posições.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patio>>> GetPatios()
        {
            return Ok(await _patioService.GetAllAsync());
        }

        /// <summary>
        /// Busca um pátio por ID, incluindo suas posições.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Patio>> GetPatio(int id)
        {
            var patio = await _patioService.GetByIdAsync(id);
            if (patio == null) return NotFound();
            return Ok(patio);
        }

        /// <summary>
        /// Cria um novo pátio.
        /// </summary>
        /// <remarks>
        /// Você pode criar posições junto com o pátio:
        /// {
        ///   "nome": "Pátio Central",
        ///   "endereco": "Rua A, 123",
        ///   "posicoes": [
        ///     { "codigo": "A01" },
        ///     { "codigo": "A02" }
        ///   ]
        /// }
        /// </remarks>
        [HttpPost]
        public async Task<ActionResult<Patio>> PostPatio(Patio patio)
        {
            var novoPatio = await _patioService.CreateAsync(patio);
            return CreatedAtAction(nameof(GetPatio), new { id = novoPatio.Id }, novoPatio);
        }
    }
}
