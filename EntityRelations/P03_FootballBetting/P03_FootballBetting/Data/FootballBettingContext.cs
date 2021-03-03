using Microsoft.EntityFrameworkCore;

using P03_FootballBetting.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P03_FootballBetting.Data
{
    public class FootballBettingContext : DbContext
    {
        public FootballBettingContext()
        {

        }

        public FootballBettingContext(DbContextOptions options)
            : base(options)
        {

        }

        public DbSet<Bet> Bets { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerStatistic> PlayerStatistics { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=FootballBetting;Integrated Security=True;");
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>(x =>
            {
                x.HasOne(x => x.PrimaryKitColor)
                .WithMany(t => t.PrimaryKitTeams)
                .HasForeignKey(k => k.PrimaryKitColorId)
                .OnDelete(DeleteBehavior.Restrict);

                x.HasOne(x => x.SecondaryKitColor)
               .WithMany(s => s.SecondaryKitTeams)
               .HasForeignKey(s => s.SecondaryKitColorId)
               .OnDelete(DeleteBehavior.Restrict);
            });
                

            modelBuilder.Entity<Game>(z =>
            {
                z.HasOne(g => g.HomeTeam)
                .WithMany(x => x.HomeGames)
                .HasForeignKey(f => f.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

                z.HasOne(g => g.AwayTeam)
                .WithMany(m => m.AwayGames)
                .HasForeignKey(f => f.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<PlayerStatistic>(x =>
            {
                    x.HasKey(x => new { x.GameId, x.PlayerId });
            });

            //base.OnModelCreating(modelBuilder);
        }
    }
}
