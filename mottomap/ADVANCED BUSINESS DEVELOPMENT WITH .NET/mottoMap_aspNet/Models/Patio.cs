using System.ComponentModel.DataAnnotations;

namespace MotoMap.Api.DotNet.Models
{
    /// <summary>
    /// Representa um pátio ou filial da Mottu.
    /// </summary>
    public class Patio
    {
        /// <summary>
        /// ID único do pátio.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome do pátio (ex: "Pátio Central SP").
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = null!;

        /// <summary>
        /// Endereço do pátio.
        /// </summary>
        [StringLength(255)]
        public string? Endereco { get; set; }

        /// <summary>
        /// Lista de posições (vagas) que pertencem a este pátio.
        /// </summary>
        public ICollection<Posicao> Posicoes { get; set; } = new List<Posicao>();
    }


}