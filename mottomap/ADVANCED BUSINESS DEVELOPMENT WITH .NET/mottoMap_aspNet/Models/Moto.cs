using System.ComponentModel.DataAnnotations;

namespace MotoMap.Api.DotNet.Models
{
    /// <summary>
    /// Representa uma moto no sistema.
    /// </summary>
    public class Moto
    {
        /// <summary>
        /// ID único da moto.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Placa da moto (ex: BRA2E19).
        /// </summary>
        [Required]
        [StringLength(10)]
        public string Placa { get; set; } = null!;

        /// <summary>
        /// Modelo da moto (ex: Honda Biz).
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Modelo { get; set; } = null!;

        /// <summary>
        /// Ano de fabricação da moto.
        /// </summary>
        public int Ano { get; set; }
    }
}