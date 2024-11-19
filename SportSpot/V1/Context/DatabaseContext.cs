using Microsoft.EntityFrameworkCore;
using SportSpot.V1.Session.Entities;
using SportSpot.V1.Media.Entities;

namespace SportSpot.V1.Context
{
    public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
    {

        public DbSet<MediaEntity> Media { get; set; } = null!;
        public DbSet<SessionEntity> Session { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MediaEntity>()
                .HasNoDiscriminator()
                .HasKey(x => x.Id);

            modelBuilder.Entity<SessionEntity>()
                .HasNoDiscriminator()
                .HasKey(x => x.Id);
        }
    }
}
