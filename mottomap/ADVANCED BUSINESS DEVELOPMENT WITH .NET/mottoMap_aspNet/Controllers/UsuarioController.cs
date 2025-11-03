using Microsoft.AspNetCore.Mvc;
using MotoMap.Api.DotNet.Models;
using MotoMap.Api.DotNet.Services;

namespace MotoMap.Api.DotNet.Controllers
{
    /// <summary>
    /// Gerencia o cadastro de Usuários (Operadores, Admins).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Lista todos os usuários.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return Ok(await _usuarioService.GetAllAsync());
        }

        /// <summary>
        /// Busca um usuário por ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _usuarioService.GetByIdAsync(id);
            if (usuario == null) return NotFound();
            return Ok(usuario);
        }

        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            var novoUsuario = await _usuarioService.CreateAsync(usuario);
            return CreatedAtAction(nameof(GetUsuario), new { id = novoUsuario.Id }, novoUsuario);
        }

        /// <summary>
        /// Atualiza um usuário existente.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id) return BadRequest();
            var sucesso = await _usuarioService.UpdateAsync(id, usuario);
            if (!sucesso) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Deleta um usuário.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var sucesso = await _usuarioService.DeleteAsync(id);
            if (!sucesso) return NotFound();
            return NoContent();
        }
    }
}
