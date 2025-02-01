using SportSpot.V1.User.Dtos;
using SportSpot.V1.User.Entities;
using System.Security.Claims;

namespace SportSpot.V1.User.Services
{
    public interface ITokenService
    {
        AccessTokenDto GenerateAccessToken(IEnumerable<Claim> claims);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Task<RefreshTokenEntity> CreateRefreshToken(AuthUserEntity authUser, string accessToken);
        Task<RefreshTokenEntity> RefreshRefreshToken(RefreshTokenEntity entity, string accessToken);
    }
}
