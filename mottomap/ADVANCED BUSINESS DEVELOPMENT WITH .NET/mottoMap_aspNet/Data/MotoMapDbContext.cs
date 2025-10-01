using Microsoft.EntityFrameworkCore;
using MotoMap.Api.DotNet.Models;

namespace MotoMap.Api.DotNet.Data
{
    public class MotoMapDbContext : DbContext
    {
        public MotoMapDbContext(DbContextOptions<MotoMapDbContext> options) : base(options)
        {
        }

        public DbSet<Movimentacao> Movimentacoes { get; set; }
        public DbSet<HistoricoPosicao> HistoricoPosicoes { get; set; }

        public DbSet<Moto> Motos { get; set; }
    }
}