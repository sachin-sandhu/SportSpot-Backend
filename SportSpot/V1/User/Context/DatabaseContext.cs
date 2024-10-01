using Microsoft.EntityFrameworkCore;

namespace SportSpot.V1.User
{
    public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
    {
        public DbSet<ClubEntity> Clubs { get; set; }
        public DbSet<UserEntity> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClubEntity>()
                .HasNoDiscriminator()
                .HasKey(u => u.Id);
            modelBuilder.Entity<UserEntity>()
                .HasNoDiscriminator()
                .HasKey(u => u.Id);
        }
    }
}
