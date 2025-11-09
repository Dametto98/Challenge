using Microsoft.ML.Data;

namespace MotoMap.Api.DotNet.MLModels
{
    /// <summary>
    /// Classe de ENTRADA para o treinamento do modelo de ML.
    /// </summary>
    public class TempoPatioInput
    {
        /// <summary>
        /// ID do Pátio (Feature 1)
        /// </summary>
        [LoadColumn(0)]
        public float PatioId { get; set; }

        /// <summary>
        /// ID da Posição (Feature 2)
        /// </summary>
        [LoadColumn(1)]
        public float PosicaoId { get; set; }

        /// <summary>
        /// O que queremos prever (O "Label")
        /// </summary>
        [LoadColumn(2)]
        public float DuracaoHoras { get; set; }
    }
}