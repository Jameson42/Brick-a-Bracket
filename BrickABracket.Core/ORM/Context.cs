using BrickABracket.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace BrickABracket.Core.ORM
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        { }

        public DbSet<Classification> Classifications { get; set; }
        public DbSet<Competitor> Competitors { get; set; }
        public DbSet<Moc> Mocs { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentSummary> TournamentSummaries { get; set; }

        // TODO: Can I create any shortcuts to common .Include chains?
        // TODO: Need Image storage

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Set primary keys (legacy values from LiteDB storage)
            builder.Entity<Classification>()
                .HasKey(c => c._id);
            builder.Entity<Competitor>()
                .HasKey(c => c._id);
            builder.Entity<Moc>()
                .HasKey(m => m._id);
            builder.Entity<Tournament>()
                .HasKey(t => t._id);

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
        }
    }
}