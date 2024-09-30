using Microsoft.AspNetCore.Identity;
using SportSpot.Exceptions.User;
using SportSpot.V1.User.Dtos.Auth;
using SportSpot.V1.User.Services;

namespace SportSpot.V1.User
{
    public class AuthService(UserManager<AuthUserEntity> _userManager, ITokenService _tokenService, IUserService _userService, IClubService _clubService) : IAuthService
    {
        public async Task<AuthTokenDto> Login(AuthUserLoginRequestDto request)
        {
            AuthUserEntity? user = await _userManager.FindByEmailAsync(request.Email);
            user ??= await _userManager.FindByNameAsync(request.Email);
            if (user is null)
                throw new UserLoginException();
            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                throw new UserLoginException();
            return await _userManager.GenerateToken(user, _tokenService);
        }

        public async Task<AuthTokenDto> RefreshAccessToken(AuthUserEntity user, RefreshTokenRequestDto request)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            if (user is null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                throw new InvalidTokenRequestException();

            return await _userManager.GenerateToken(user, _tokenService, principal.Claims);
        }

        public async Task<AuthTokenDto> Register(ClubRegisterRequestDto request)
        {
            AuthUserEntity authUser = new()
            {
                UserName = request.Username,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                ProfileType = ProfileType.CLUB
            };
            IdentityResult result = await _userManager.CreateAsync(authUser, request.Password);
            if (!result.Succeeded)
                throw new UserRegisterException(result.Errors);

            AuthTokenDto token = await _userManager.GenerateToken(authUser, _tokenService);
            await _clubService.CreateClub(authUser.Id, request);
            return token;
        }

        public async Task<AuthTokenDto> Register(UserRegisterRequestDto request)
        {
            AuthUserEntity authUser = new()
            {
                UserName = request.Username,
                Email = request.Email,
                ProfileType = ProfileType.USER
            };
            IdentityResult result = await _userManager.CreateAsync(authUser, request.Password);
            if (!result.Succeeded)
                throw new UserRegisterException(result.Errors);

            AuthTokenDto token = await _userManager.GenerateToken(authUser, _tokenService);
            await _userService.CreateUser(authUser.Id, request);
            return token;
        }

        public async Task RevokeRefreshToken(AuthUserEntity authUserEntity)
        {
            authUserEntity.RefreshToken = null;
            authUserEntity.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(authUserEntity);
        }
    }
}
