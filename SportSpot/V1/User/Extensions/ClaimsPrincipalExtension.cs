using Microsoft.AspNetCore.Identity;
using SportSpot.V1.Exceptions;
using SportSpot.V1.User.Entities;
using System.Security.Claims;

namespace SportSpot.V1.User.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static async Task<AuthUserEntity> GetAuthUser(this ClaimsPrincipal principal, UserManager<AuthUserEntity> userManager)
        {
            Claim subClaim = principal.Claims.First(x => x.Type != null && x.Type.EndsWith("nameidentifier")) ?? throw new UnauthorizedException();
            return await userManager.FindByIdAsync(subClaim.Value) ?? throw new UnauthorizedException();
        }
    }
}
