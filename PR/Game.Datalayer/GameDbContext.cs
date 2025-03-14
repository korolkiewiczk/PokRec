using Game.Datalayer.Model;
using Microsoft.EntityFrameworkCore;

namespace Game.Datalayer;

public class GameDbContext : DbContext
{
    public DbSet<Stats> Stats { get; set; }
    public DbSet<Nickname> Nicknames { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite("Data Source=game_stats.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Stats>()
            .HasOne(s => s.Nickname)
            .WithOne(n => n.Stats)
            .HasForeignKey<Stats>(s => s.NicknameId);

        modelBuilder.Entity<Nickname>()
            .HasIndex(n => n.NicknameId)
            .IsUnique();

        modelBuilder.Entity<Nickname>()
            .HasIndex(n => n.Name);

        modelBuilder.Entity<Stats>()
            .HasIndex(s => s.NicknameId);
    }
}