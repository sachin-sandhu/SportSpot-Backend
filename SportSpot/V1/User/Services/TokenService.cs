using Microsoft.IdentityModel.Tokens;
using SportSpot.V1.Exceptions.User;
using SportSpot.V1.User.Context;
using SportSpot.V1.User.Dtos;
using SportSpot.V1.User.Dtos.Auth;
using SportSpot.V1.User.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SportSpot.V1.User.Services
{
    public class TokenService(JwtConfigurationDto _configuration, AuthContext _context) : ITokenService
    {
        public AccessTokenDto GenerateAccessToken(IEnumerable<Claim> claims)
        {
            SymmetricSecurityKey secretKey = new(Encoding.UTF8.GetBytes(_configuration.Secret));
            SigningCredentials signinCredentials = new(secretKey, SecurityAlgorithms.HmacSha256);
            DateTime expire = DateTime.UtcNow.AddMinutes(_configuration.Duration.TotalMinutes);

            JwtSecurityToken tokeOptions = new(
                issuer: _configuration.ValidIsUser,
                audience: _configuration.ValidAudience,
                claims: claims,
                expires: expire,
                signingCredentials: signinCredentials
            );
            string? tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return new AccessTokenDto
            {
                Token = tokenString ?? throw new AccessTokenGenerateException(),
                Expire = expire
            };
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidAudience = _configuration.ValidAudience,
                ValidIssuer = _configuration.ValidIsUser,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Secret))
            };

            JwtSecurityTokenHandler tokenHandler = new();
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public async Task<RefreshTokenEntity> CreateRefreshToken(AuthUserEntity authUser, string accessToken)
        {
            RefreshTokenEntity tokenEntity = new()
            {
                UserId = authUser.Id,
                Token = GenerateRefreshToken(authUser.Id),
                ExpiryTime = DateTime.UtcNow.AddDays(30),
                User = authUser,
                AccessToken = accessToken
            };
            _context.RefreshTokens.Add(tokenEntity);
            await _context.SaveChangesAsync();
            return tokenEntity;
        }

        public async Task<RefreshTokenEntity> RefreshRefreshToken(RefreshTokenEntity entity, string accessToken)
        {
            entity.ExpiryTime = DateTime.UtcNow.AddDays(30);
            entity.Token = GenerateRefreshToken(entity.UserId);
            entity.AccessToken = accessToken;
            _context.RefreshTokens.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        private static string GenerateRefreshToken(Guid userId)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            userId.ToByteArray().CopyTo(randomNumber, 0);
            return Convert.ToBase64String(randomNumber);
        }
    }
}