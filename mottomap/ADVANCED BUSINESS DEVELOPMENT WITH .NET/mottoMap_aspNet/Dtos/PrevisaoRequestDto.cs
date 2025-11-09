using System.ComponentModel.DataAnnotations;

namespace MotoMap.Api.DotNet.Dtos
{
    /// <summary>
    /// DTO para requisitar uma nova previsão de tempo de estadia.
    /// </summary>
    public class PrevisaoRequestDto
    {
        [Required]
        public int PatioId { get; set; }

        [Required]
        public int PosicaoId { get; set; }
    }
}