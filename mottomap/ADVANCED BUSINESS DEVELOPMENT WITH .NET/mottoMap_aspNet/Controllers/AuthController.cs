using Microsoft.AspNetCore.Mvc;
using MotoMap.Api.DotNet.Dtos;
using MotoMap.Api.DotNet.Services;

namespace MotoMap.Api.DotNet.Controllers
{
    [ApiVersion("1.0")] 
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public AuthController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UsuarioRegisterDto registerDto)
        {
            var usuario = await _usuarioService.RegisterAsync(registerDto);
            if (usuario == null)
            {
                return BadRequest("Email já cadastrado.");
            }
            return Ok(usuario);
        }

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