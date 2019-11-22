using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BrickABracket.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace BrickABracket.Core.ORM
{
    public class BabContext : DbContext
    {
        public BabContext(DbContextOptions<BabContext> options) : base(options)
        { }

        public DbSet<Classification> Classifications { get; set; }
        public DbSet<Competitor> Competitors { get; set; }
        public DbSet<Moc> Mocs { get; set; }
        public DbSet<MocImage> MocImages { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentSummary> TournamentSummaries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Set primary keys (legacy values from LiteDB storage)
            builder.Entity<Classification>()
                .HasKey(c => c._id);
            builder.Entity<Competitor>()
                .HasKey(c => c._id);
            builder.Entity<Moc>()
                .HasKey(m => m._id);
            builder.Entity<TournamentSummary>()
                .HasKey(t => t._id);
            builder.Entity<MocImage>()
                .HasKey(mi => mi._id);

            // Entity relationships
            // TODO: Can any be changed to Owned Entities?
            builder.Entity<Tournament>()
                .HasMany(c => c.Categories)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Category>()
                .HasMany(c => c.Rounds)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Category>()
                .HasMany(c => c.Standings)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Round>()
                .HasMany(r => r.Matches)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Round>()
                .HasMany(r => r.Standings)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Match>()
                .HasMany(m => m.Results)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<MatchResult>()
                .HasMany(mr => mr.Scores)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            // Property conversions
            Expression<Func<List<int>, string>> idsToString = mi => string.Join(',', mi);
            Expression<Func<string, List<int>>> stringToIds = mi => mi.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => int.Parse(s))
                            .ToList();
            builder.Entity<Tournament>()
                .Property(t => t.MocIds)
                .HasConversion(idsToString, stringToIds);
            builder.Entity<Category>()
                .Property(c => c.MocIds)
                .HasConversion(idsToString, stringToIds);
            builder.Entity<Round>()
                .Property(r => r.MocIds)
                .HasConversion(idsToString, stringToIds);
            builder.Entity<Match>()
                .Property(m => m.MocIds)
                .HasConversion(idsToString, stringToIds);
        }
    }
}