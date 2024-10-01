using Microsoft.AspNetCore.Identity;
using SportSpot.V1.Exceptions;
using System.Security.Claims;

namespace SportSpot.V1.User
{
    public static class ClaimsPrincipalExtension
    {
        public static async Task<AuthUserEntity> GetAuthUser(this ClaimsPrincipal principal, UserManager<AuthUserEntity> userManager) => await userManager.FindByNameAsync(principal.Identity?.Name ?? throw new UnauthorizedException()) ?? throw new UnauthorizedException();
    }
}
