using System.ComponentModel.DataAnnotations;

namespace MotoMap.Api.DotNet.Models
{
    /// <summary>
    /// Representa o estado de ocupação de uma posição por uma moto ao longo do tempo.
    /// </summary>
    public class HistoricoPosicao
    {
        /// <summary>
        /// ID único do registro de histórico.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID da moto que está (ou esteve) na posição.
        /// </summary>
        public int MotoId { get; set; }

        /// <summary>
        /// ID da posição que está (ou esteve) ocupada.
        /// </summary>
        public int PosicaoId { get; set; }

        /// <summary>
        /// Data e hora em que a moto entrou na posição.
        /// </summary>
        public DateTime DataInicio { get; set; }

        /// <summary>
        /// Data e hora em que a moto saiu da posição.
        /// (Nulo se a moto ainda estiver na posição).
        /// </summary>
        public DateTime? DataFim { get; set; }
    }
}