using System.ComponentModel.DataAnnotations;

namespace MotoMap.Api.DotNet.Models
{
    /// <summary>
    /// Representa um evento de movimentação (entrada ou saída).
    /// </summary>
    public class Movimentacao
    {
        public int Id { get; set; }

        /// <summary>
        /// Tipo de evento.
        /// </summary>
        /// <example>ENTRADA</example>
        [Required]
        [StringLength(10)]
        public string Tipo { get; set; } = null!; // "ENTRADA" ou "SAIDA"

        /// <summary>
        /// Data e hora em que o evento ocorreu.
        /// </summary>
        public DateTime DataHora { get; set; }

        /// <summary>
        /// ID da moto que foi movimentada.
        /// </summary>
        /// <example>1</example>
        public int MotoId { get; set; }

        /// <summary>
        /// ID do usuário (operador) que registrou a movimentação.
        /// </summary>
        /// <example>1</example>
        public int UsuarioId { get; set; }

        /// <summary>
        /// ID da posição envolvida na movimentação.
        /// </summary>
        /// <example>1</example>
        public int PosicaoId { get; set; }

        /// <summary>
        /// Observações adicionais.
        /// </summary>
        /// <example>Entrada para manutenção.</example>
        [StringLength(500)]
        public string? Observacoes { get; set; }
    }
}
