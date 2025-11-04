using System.ComponentModel.DataAnnotations;

namespace MotoMap.Api.DotNet.Dtos
{
    /// <summary>
    /// Dados necessários para o login de um usuário.
    /// </summary>
    public class UsuarioLoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
