using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoMap.Api.DotNet.Data;
using MotoMap.Api.DotNet.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MotoMap.Api.DotNet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MotosController : ControllerBase
    {
        private readonly MotoMapDbContext _context;

        public MotosController(MotoMapDbContext context)
        {
            _context = context;
        }

        // GET: api/motos (com paginação)
        [HttpGet]
        /// Dados da moto a ser criada
        public async Task<ActionResult<IEnumerable<Moto>>> GetMotos([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return await _context.Motos
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();
        }

        // GET: api/motos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Moto>> GetMoto(int id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null)
            {
                return NotFound();
            }
            return moto;
        }

        // POST: api/motos
        [HttpPost]
        public async Task<ActionResult<Moto>> PostMoto(Moto moto)
        {
            _context.Motos.Add(moto);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMoto), new { id = moto.Id }, moto);
        }

        // PUT: api/motos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMoto(int id, Moto moto)
        {
            if (id != moto.Id)
            {
                return BadRequest();
            }
            _context.Entry(moto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/motos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMoto(int id)
        {
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null)
            {
                return NotFound();
            }
            _context.Motos.Remove(moto);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}