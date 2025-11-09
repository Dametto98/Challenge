using System.ComponentModel.DataAnnotations;

namespace MotoMap.Api.DotNet.Models
{
    /// <summary>
    /// Representa um evento de movimentação (uma entrada ou saída).
    /// </summary>
    public class Movimentacao
    {
        /// <summary>
        /// ID único do evento de movimentação.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Tipo de evento (ex: "ENTRADA", "SAIDA").
        /// </summary>
        [Required]
        [StringLength(10)]
        public string Tipo { get; set; } = null!;

        /// <summary>
        /// Data e hora em que o evento ocorreu.
        /// </summary>
        public DateTime DataHora { get; set; }

        /// <summary>
        /// Observações (ex: "Manutenção", "Entrega").
        /// </summary>
        [StringLength(500)]
        public string? Observacoes { get; set; }

        // --- Chaves Estrangeiras ---

        /// <summary>
        /// ID da moto que foi movimentada.
        /// </summary>
        public int MotoId { get; set; }

        /// <summary>
        /// ID do usuário (operador) que registrou a movimentação.
        /// </summary>
        public int? UsuarioId { get; set; }

        /// <summary>
        /// ID da posição (vaga) envolvida na movimentação.
        /// </summary>
        public int PosicaoId { get; set; }
    }


}