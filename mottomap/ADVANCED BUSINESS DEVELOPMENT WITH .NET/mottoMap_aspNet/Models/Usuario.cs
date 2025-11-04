using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization; 

namespace MotoMap.Api.DotNet.Models
{

    public class Usuario
    {

        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = null!;


        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = null!;


        [Required]
        [JsonIgnore] 
        public string PasswordHash { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Perfil { get; set; } = "Operador"; 
    }
}