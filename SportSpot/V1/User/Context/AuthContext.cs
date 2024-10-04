using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SportSpot.V1.User
{
    public class AuthContext(DbContextOptions<AuthContext> options) : IdentityDbContext<AuthUserEntity, AuthRoleEntity, Guid>(options)
    {

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AuthUserEntity>()
                .HasDiscriminator<ProfileType>("ProfileType")
                .HasValue<AuthUserEntity>(ProfileType.NONE)
                .HasValue<UserEntity>(ProfileType.USER)
                .HasValue<ClubEntity>(ProfileType.CLUB);
        }
    }
}
