using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoMap.Api.DotNet.Dtos;
using MotoMap.Api.DotNet.MLModels;
using MotoMap.Api.DotNet.Services;

namespace MotoMap.Api.DotNet.Controllers
{
    /// <summary>
    /// Endpoint que utiliza ML.NET para fazer previsões.
    /// </summary>
    [Authorize] // Protegido por JWT
    [ApiVersion("1.0")] // Faz parte da v1
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PrevisaoController : ControllerBase
    {
        private readonly TempoPatioPredictionService _predictionService;

        public PrevisaoController(TempoPatioPredictionService predictionService)
        {
            _predictionService = predictionService;
        }

        /// <summary>
        /// Prevê o tempo de estadia (em horas) de uma moto em um pátio/posição.
        /// </summary>
        /// <remarks>
        /// Exemplo:
        ///
        ///     POST /api/v1/Previsao/tempo-estadia
        ///     {
        ///       "patioId": 1,
        ///       "posicaoId": 1
        ///     }
        ///
        /// </remarks>
        /// <returns>A previsão em horas.</returns>
        [HttpPost("tempo-estadia")]
        [ProducesResponseType(typeof(TempoPatioOutput), StatusCodes.Status200OK)]
        public IActionResult PreverTempoEstadia(PrevisaoRequestDto request)
        {
            var input = new TempoPatioInput
            {
                PatioId = request.PatioId,
                PosicaoId = request.PosicaoId
            };

            var prediction = _predictionService.Predict(input);

            return Ok(prediction);
        }
    }
}