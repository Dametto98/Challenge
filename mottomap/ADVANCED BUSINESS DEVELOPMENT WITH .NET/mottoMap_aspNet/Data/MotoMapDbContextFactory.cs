using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MotoMap.Api.DotNet.Data
{
    public class MotoMapDbContextFactory : IDesignTimeDbContextFactory<MotoMapDbContext>
    {
        public MotoMapDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<MotoMapDbContext>();

            // Alterado para ler a nova string de conexão
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Alterado de UseOracle para UseSqlite
            optionsBuilder.UseSqlite(connectionString);

            return new MotoMapDbContext(optionsBuilder.Options);
        }
    }
}