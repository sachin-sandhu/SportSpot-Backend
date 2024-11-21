using Microsoft.AspNetCore.Identity;
using SportSpot.V1.User.Dtos;
using SportSpot.V1.User.Dtos.Auth;
using SportSpot.V1.User.Entities;
using SportSpot.V1.User.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SportSpot.V1.User.Extensions
{
    public static class UserManagerExtension
    {
        public async static Task<AuthTokenDto> GenerateToken(this UserManager<AuthUserEntity> userManager, AuthUserEntity user, ITokenService tokenService, IEnumerable<Claim>? claims = null)
        {
            claims ??= await GetAuthClaims(userManager, user);
            AccessTokenDto accessToken = tokenService.GenerateAccessToken(claims);
            string refreshToken = tokenService.GenerateRefreshToken();
            DateTime refreshTokenExpire = DateTime.Now.AddDays(30);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = refreshTokenExpire;
            await userManager.UpdateAsync(user);
            return new AuthTokenDto
            {
                UserId = user.Id,
                AccessToken = accessToken.Token,
                AccessExpire = accessToken.Expire,
                RefreshToken = refreshToken,
                RefreshExpire = refreshTokenExpire
            };
        }

        private static async Task<List<Claim>> GetAuthClaims(UserManager<AuthUserEntity> userManager, AuthUserEntity user)
        {
            var userRoles = await userManager.GetRolesAsync(user);
            List<Claim> authClaims = [
                new (ClaimTypes.Name, user.UserName ?? ""),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ];
            foreach (var userRole in userRoles)
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            return authClaims;
        }

    }
}
