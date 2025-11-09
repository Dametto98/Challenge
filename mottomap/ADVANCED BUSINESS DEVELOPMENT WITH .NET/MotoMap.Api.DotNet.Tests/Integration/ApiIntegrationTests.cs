using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

/*
 * NOTA IMPORTANTE:
 * IClassFixture<CustomWebApplicationFactory> diz ao xUnit para usar a nossa
 * fábrica customizada (com o banco em memória), em vez da padrão.
 */

namespace MotoMap.Api.DotNet.Tests.Integration
{
    public class ApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public ApiIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(); // Cria um cliente HTTP que roda a API em memória
        }

        [Fact]
        public async Task Get_Health_ReturnsOk()
        {
            // Arrange (Organizar)
            var url = "/health";

            // Act (Agir)
            var response = await _client.GetAsync(url);

            // Assert (Verificar)
            // Agora o DbContextCheck vai usar o banco em memória e retornar "Healthy"
            response.EnsureSuccessStatusCode(); // Deve ser 200 OK
            
            var stringResponse = await response.Content.ReadAsStringAsync();
            Assert.Contains("Healthy", stringResponse);
        }

        [Fact]
        public async Task Get_Motos_WithoutAuth_ReturnsUnauthorized()
        {
            // Arrange (Organizar)
            var url = "/api/v1/Motos"; // Este é um endpoint protegido com [Authorize]

            // Act (Agir)
            var response = await _client.GetAsync(url);

            // Assert (Verificar)
            // Verifica se o status code é 401 Unauthorized
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}