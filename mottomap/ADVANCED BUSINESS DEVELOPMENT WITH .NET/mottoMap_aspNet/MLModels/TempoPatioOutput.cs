using Microsoft.ML.Data;

namespace MotoMap.Api.DotNet.MLModels
{
    /// <summary>
    /// Classe de SAÍDA (Previsão) do modelo de ML.
    /// </summary>
    public class TempoPatioOutput
    {
        /// <summary>
        /// O valor previsto pelo modelo (em horas).
        /// </summary>
        [ColumnName("Score")]
        public float HorasEstadiaPrevistas { get; set; }
    }
}