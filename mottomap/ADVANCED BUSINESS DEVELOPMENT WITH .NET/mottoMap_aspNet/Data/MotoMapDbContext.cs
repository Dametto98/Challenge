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

        // --- ADICIONE AS LINHAS ABAIXO ---
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Patio> Patios { get; set; }
        public DbSet<Posicao> Posicoes { get; set; }
        // --- FIM DA ADIÇÃO ---

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configura o relacionamento 1-N entre Patio e Posicao
            modelBuilder.Entity<Patio>()
                .HasMany(p => p.Posicoes)
                .WithOne(pos => pos.Patio)
                .HasForeignKey(pos => pos.PatioId);

            // Garante que o Email do Usuário é único
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}