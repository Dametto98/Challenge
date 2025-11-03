using Microsoft.AspNetCore.Mvc;
using MotoMap.Api.DotNet.Models;
using MotoMap.Api.DotNet.Services;

namespace MotoMap.Api.DotNet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MotosController : ControllerBase
    {
        private readonly IMotoService _motoService;

        public MotosController(IMotoService motoService)
        {
            _motoService = motoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Moto>>> GetMotos([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var motos = await _motoService.GetAllAsync(pageNumber, pageSize);
            return Ok(motos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Moto>> GetMoto(int id)
        {
            var moto = await _motoService.GetByIdAsync(id);

            if (moto == null)
            {
                return NotFound();
            }

            return Ok(moto);
        }

        [HttpPost]
        public async Task<ActionResult<Moto>> PostMoto(Moto moto)
        {
            var motoCriada = await _motoService.CreateAsync(moto);
            return CreatedAtAction(nameof(GetMoto), new { id = motoCriada.Id }, motoCriada);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMoto(int id, Moto moto)
        {
            if (id != moto.Id)
            {
                return BadRequest();
            }

            var sucesso = await _motoService.UpdateAsync(id, moto);
            if (!sucesso)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMoto(int id)
        {
            var sucesso = await _motoService.DeleteAsync(id);
            if (!sucesso)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
