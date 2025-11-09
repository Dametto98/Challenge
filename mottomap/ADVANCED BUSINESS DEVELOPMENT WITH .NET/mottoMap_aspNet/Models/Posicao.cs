using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MotoMap.Api.DotNet.Models
{
    /// <summary>
    /// Representa uma posição específica (vaga) dentro de um pátio.
    /// </summary>
    public class Posicao
    {
        /// <summary>
        /// ID único da posição.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Código da posição (ex: "A-01", "B-12").
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = null!;

        /// <summary>
        /// Chave estrangeira para o Pátio ao qual esta posição pertence.
        /// </summary>
        public int PatioId { get; set; }

        /// <summary>
        /// Propriedade de navegação para o Pátio (ignorado no JSON para evitar loops).
        /// </summary>
        [JsonIgnore]
        public Patio? Patio { get; set; }
    }


}