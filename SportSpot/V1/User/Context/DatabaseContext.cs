using Microsoft.EntityFrameworkCore;

namespace SportSpot.V1.User.Context
{
    public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
    {
        public DbSet<ClubEntity> Clubs { get; set; }
        public DbSet<UserEntity> User { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ClubEntity>()
                .HasNoDiscriminator()
                .HasKey(u => u.Id);
            builder.Entity<UserEntity>()
                .HasNoDiscriminator()
                .HasKey(u => u.Id);
        }
    }
}
