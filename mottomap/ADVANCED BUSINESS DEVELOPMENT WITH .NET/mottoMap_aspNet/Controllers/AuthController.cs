using Microsoft.AspNetCore.Mvc;
using MotoMap.Api.DotNet.Dtos;
using MotoMap.Api.DotNet.Services;

namespace MotoMap.Api.DotNet.Controllers
{
    /// <summary>
    /// Gerencia o registro e login de usuários.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public AuthController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Registra um novo usuário no sistema.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register(UsuarioRegisterDto registerDto)
        {
            var usuario = await _usuarioService.RegisterAsync(registerDto);
            if (usuario == null)
            {
                return BadRequest("Email já cadastrado.");
            }
            // Retorna o usuário criado (sem a senha)
            return CreatedAtAction(nameof(Register), new { id = usuario.Id }, usuario);
        }

        /// <summary>
        /// Autentica um usuário e retorna um token JWT.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login(UsuarioLoginDto loginDto)
        {
            var token = await _usuarioService.LoginAsync(loginDto);
            if (token == null)
            {
                return Unauthorized("Email ou senha inválidos.");
            }
            return Ok(new { token = token });
        }
    }
}
