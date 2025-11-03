using System.ComponentModel.DataAnnotations;

namespace MotoMap.Api.DotNet.Models
{
    /// <summary>
    /// Representa uma moto no sistema.
    /// </summary>
    public class Moto
    {
        public int Id { get; set; }

        /// <summary>
        /// Placa da moto (ex: BRA2E19).
        /// </summary>
        /// <example>BRA2E19</example>
        [Required]
        [StringLength(10)]
        public string Placa { get; set; } = null!;

        /// <summary>
        /// Modelo da moto (ex: Honda Biz).
        /// </summary>
        /// <example>Honda Biz</example>
        [Required]
        [StringLength(100)]
        public string Modelo { get; set; } = null!;

        /// <summary>
        /// Ano de fabricação da moto.
        /// </summary>
        /// <example>2022</example>
        public int Ano { get; set; }
    }
}
