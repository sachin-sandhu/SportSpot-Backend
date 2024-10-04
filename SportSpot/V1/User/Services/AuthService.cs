using Microsoft.AspNetCore.Identity;
using SportSpot.V1.Exceptions.User;

namespace SportSpot.V1.User
{
    public class AuthService(IEventService _eventService, UserManager<AuthUserEntity> _userManager, ITokenService _tokenService) : IAuthService
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
            ClubEntity authUser = new()
            {
                UserName = request.Username,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                ProfileType = ProfileType.CLUB,
                Address = request.Address?.Convert(),
            };
            IdentityResult result = await _userManager.CreateAsync(authUser, request.Password);
            if (!result.Succeeded)
                throw new UserRegisterException(result.Errors);

            await _eventService.FireEvent(new AuthUserCreatedEvent { AuthUserEntity = authUser });
            return await _userManager.GenerateToken(authUser, _tokenService);
        }

        public async Task<AuthTokenDto> Register(UserRegisterRequestDto request)
        {
            UserEntity authUser = new()
            {
                UserName = request.Username,
                Email = request.Email,
                ProfileType = ProfileType.USER,
                FirstName = request.FirstName,
                LastName = request.LastName
            };
            IdentityResult result = await _userManager.CreateAsync(authUser, request.Password);
            if (!result.Succeeded)
                throw new UserRegisterException(result.Errors);

            await _eventService.FireEvent(new AuthUserCreatedEvent { AuthUserEntity = authUser });

            return await _userManager.GenerateToken(authUser, _tokenService);
        }

        public async Task RevokeRefreshToken(AuthUserEntity authUserEntity)
        {
            authUserEntity.RefreshToken = null;
            authUserEntity.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(authUserEntity);
        }
    }
}
