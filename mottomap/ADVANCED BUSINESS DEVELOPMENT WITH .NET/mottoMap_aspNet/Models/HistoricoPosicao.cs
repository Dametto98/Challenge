namespace MotoMap.Api.DotNet.Models
{
    public class HistoricoPosicao
    {
        public int Id { get; set; }

        public int MotoId { get; set; }

        public int PosicaoId { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime? DataFim { get; set; }
    }
}
