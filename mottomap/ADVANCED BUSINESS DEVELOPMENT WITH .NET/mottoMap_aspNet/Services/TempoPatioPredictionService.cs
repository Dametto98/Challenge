// Caminho: mottoMap_aspNet/Services/TempoPatioPredictionService.cs

using Microsoft.ML;
using MotoMap.Api.DotNet.Data;
using MotoMap.Api.DotNet.MLModels;
using Microsoft.EntityFrameworkCore;

namespace MotoMap.Api.DotNet.Services
{
    /// <summary>
    /// Serviço Singleton que treina e mantém o modelo de ML na memória.
    /// </summary>
    public class TempoPatioPredictionService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _trainedModel;

        public TempoPatioPredictionService(IServiceProvider serviceProvider)
        {
            _mlContext = new MLContext(seed: 0);

            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MotoMapDbContext>();
                var trainingData = CarregarDadosDeTreinamento(dbContext);

                if (trainingData.Any())
                {
                    _trainedModel = TreinarModelo(trainingData);
                }
                else
                {
                    _trainedModel = CriarModeloVazio();
                }
            }
        }

        // --- MÉTODO 100% CORRIGIDO ---
        private List<TempoPatioInput> CarregarDadosDeTreinamento(MotoMapDbContext dbContext)
        {
            // 1. Puxa os dados brutos do histórico (APENAS os que já saíram)
            var historicosFechados = dbContext.HistoricoPosicoes
                .Where(h => h.DataFim.HasValue) // <- Consulta SQL simples (traduzível)
                .ToList(); // <- Traz para a memória PRIMEIRO!

            // 2. AGORA filtramos em memória (C#), onde .TotalHours FUNCIONA
            var historicosValidos = historicosFechados
                .Where(h => (h.DataFim.Value - h.DataInicio).TotalHours > 0);

            // 3. Puxa as posições para um dicionário (para lookup rápido em memória)
            var posicoesMap = dbContext.Posicoes.ToDictionary(p => p.Id, p => p.PatioId);

            var trainingData = new List<TempoPatioInput>();

            // 4. Faz o "join" em memória (C#)
            foreach (var h in historicosValidos) // <- Usa a lista filtrada
            {
                if (posicoesMap.TryGetValue(h.PosicaoId, out int patioId))
                {
                    trainingData.Add(new TempoPatioInput
                    {
                        PatioId = (float)patioId,
                        PosicaoId = (float)h.PosicaoId,
                        DuracaoHoras = (float)(h.DataFim.Value - h.DataInicio).TotalHours
                    });
                }
            }

            return trainingData;
        }

        private ITransformer TreinarModelo(List<TempoPatioInput> trainingData)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            var featureColumns = new[] { "PatioId", "PosicaoId" };

            var pipeline = _mlContext.Transforms.Concatenate("Features", featureColumns)
                .Append(_mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(_mlContext.Regression.Trainers.FastTree(labelColumnName: "DuracaoHoras", featureColumnName: "Features"));

            var model = pipeline.Fit(dataView);
            return model;
        }

        private ITransformer CriarModeloVazio()
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(new List<TempoPatioInput>());
            var pipeline = _mlContext.Transforms.Concatenate("Features", "PatioId", "PosicaoId");
            return pipeline.Fit(dataView);
        }

        public TempoPatioOutput Predict(TempoPatioInput input)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<TempoPatioInput, TempoPatioOutput>(_trainedModel);
            var prediction = predictionEngine.Predict(input);
            return prediction;
        }
    }
}