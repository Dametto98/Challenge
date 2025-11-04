using System.ComponentModel.DataAnnotations;

namespace MotoMap.Api.DotNet.Dtos
{
    public class UsuarioRegisterDto
    {
        [Required]
        public string Nome { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;

        public string? Perfil { get; set; } = "Operador";
    }
}
