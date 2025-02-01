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
        public async static Task<AuthTokenDto> GenerateToken(this UserManager<AuthUserEntity> userManager, AuthUserEntity user, ITokenService tokenService, IEnumerable<Claim>? claims = null, RefreshTokenEntity? oldRefreshTokenEntity = null)
        {
            claims ??= await GetAuthClaims(userManager, user);

            AccessTokenDto accessToken = tokenService.GenerateAccessToken(claims);

            RefreshTokenEntity refreshTokenEntity;
            if (oldRefreshTokenEntity != null)
                refreshTokenEntity = await tokenService.RefreshRefreshToken(oldRefreshTokenEntity, accessToken.Token);
            else
                refreshTokenEntity = await tokenService.CreateRefreshToken(user, accessToken.Token);


            await userManager.UpdateAsync(user);

            return new AuthTokenDto
            {
                UserId = user.Id,
                AccessToken = accessToken.Token,
                AccessExpire = accessToken.Expire,
                RefreshToken = refreshTokenEntity.Token,
                RefreshExpire = refreshTokenEntity.ExpiryTime
            };
        }

        private static async Task<List<Claim>> GetAuthClaims(UserManager<AuthUserEntity> userManager, AuthUserEntity user)
        {
            var userRoles = await userManager.GetRolesAsync(user);
            List<Claim> authClaims = [
                new (ClaimTypes.Name, user.UserName ?? ""),
                new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ];

            authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            return authClaims;
        }

    }
}
