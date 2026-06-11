using Battleship.Api.Summaries;
using Microsoft.EntityFrameworkCore;

namespace Battleship.Api.Persistence;

public sealed class BattleshipDbContext(DbContextOptions<BattleshipDbContext> options)
    : DbContext(options)
{
    public DbSet<GameSummary> GameSummaries => Set<GameSummary>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameSummary>()
            .HasIndex(summary => summary.GameId)
            .IsUnique();
    }
}