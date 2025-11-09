using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MotoMap.Api.DotNet.Models
{
    /// <summary>
    /// Representa um usuário (operador ou administrador) do sistema.
    /// </summary>
    public class Usuario
    {
        /// <summary>
        /// ID único do usuário.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome completo do usuário.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = null!;

        /// <summary>
        /// Email de login (deve ser único).
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Hash da senha do usuário (nunca retornado pela API).
        /// </summary>
        [Required]
        [JsonIgnore] // Garante que este campo NUNCA seja enviado de volta pela API
        public string PasswordHash { get; set; } = null!;

        /// <summary>
        /// Perfil de permissão (ex: "Operador", "Admin").
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Perfil { get; set; } = "Operador";
    }
}