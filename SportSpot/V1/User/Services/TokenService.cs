using Microsoft.IdentityModel.Tokens;
using SportSpot.V1.Exceptions;
using SportSpot.V1.Exceptions.User;
using SportSpot.V1.User.Dtos;
using SportSpot.V1.User.Dtos.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SportSpot.V1.User.Services
{
    public class TokenService(JwtConfigurationDto _configuration) : ITokenService
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
                ValidateLifetime = true,
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

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}