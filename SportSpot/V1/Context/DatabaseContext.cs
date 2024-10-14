using Microsoft.EntityFrameworkCore;
using SportSpot.V1.Media;

namespace SportSpot.V1.Context
{
    public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
    {

        public DbSet<MediaEntity> Media { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MediaEntity>()
                .HasNoDiscriminator()
                .HasKey(x => x.Id);
        }
    }
}
