using System.ComponentModel.DataAnnotations;

namespace MotoMap.Api.DotNet.Models
{
    /// <summary>
    /// Representa um pátio ou filial da Mottu.
    /// </summary>
    public class Patio
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = null!;

        [StringLength(255)]
        public string? Endereco { get; set; }

        // Propriedade de navegação: Um Pátio pode ter várias Posições
        public ICollection<Posicao> Posicoes { get; set; } = new List<Posicao>();
    }
}
