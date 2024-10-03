using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SportSpot.V1.User.Context
{
    public class AuthContext(DbContextOptions<AuthContext> options) : IdentityDbContext<AuthUserEntity, AuthRoleEntity, Guid>(options)
    {
    }
}
