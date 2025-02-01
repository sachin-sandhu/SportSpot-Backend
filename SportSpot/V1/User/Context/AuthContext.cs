using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportSpot.V1.User.Entities;

namespace SportSpot.V1.User.Context
{
    public class AuthContext(DbContextOptions<AuthContext> options) : IdentityDbContext<AuthUserEntity, AuthRoleEntity, Guid>(options)
    {

        public DbSet<RefreshTokenEntity> RefreshTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<AuthUserEntity>()
                .HasMany(e => e.RefreshTokens)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .IsRequired();

            builder.Entity<RefreshTokenEntity>()
                .HasKey(e => e.Id);
        }
    }
}
