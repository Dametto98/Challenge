using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MotoMap.Api.DotNet.Models
{
    /// <summary>
    /// Representa uma posição específica (vaga) dentro de um pátio.
    /// </summary>
    public class Posicao
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!; // Ex: "A-01", "B-12"

        // Chave estrangeira para o Pátio
        public int PatioId { get; set; }

        // Propriedade de navegação
        [JsonIgnore] // Evita loops de referência na serialização JSON
        public Patio? Patio { get; set; }
    }
}
