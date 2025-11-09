using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MotoMap.Api.DotNet.Data;
using System.Linq;

namespace MotoMap.Api.DotNet.Tests.Integration
{
    /// <summary>
    /// Esta é uma "Fábrica" customizada que nos permite
    /// modificar a configuração da API *antes* que ela inicie para os testes.
    /// Vamos usá-la para substituir o banco de dados SQLite por um banco em memória.
    /// </summary>
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // 1. Encontra o registro do DbContext original (o do SQLite)
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<MotoMapDbContext>));

                // 2. Remove o DbContext original (SQLite)
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // 3. Adiciona um NOVO DbContext que usa um banco de dados em memória
                // O nome "InMemoryDbForTesting" garante que cada teste use o mesmo banco em memória.
                services.AddDbContext<MotoMapDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });
            });

            builder.UseEnvironment("Development");
        }
    }
}