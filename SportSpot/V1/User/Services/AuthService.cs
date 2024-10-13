using Microsoft.AspNetCore.Identity;
using SportSpot.V1.Exceptions;
using SportSpot.V1.Exceptions.User;
using SportSpot.V1.Extensions;
using SportSpot.V1.Media;

namespace SportSpot.V1.User
{
    public class AuthService(IEventService _eventService, IMediaService _mediaService, IOAuthFactory _oauthFactory, UserManager<AuthUserEntity> _userManager, ITokenService _tokenService) : IAuthService
    {
        public async Task Delete(AuthUserEntity authUser)
        {
            if (await _eventService.FireEvent(new AuthUserDeleteEvent { AuthUser = authUser })) return;

            await _userManager.DeleteAsync(authUser);

            await _eventService.FireEvent(new AuthUserDeletedEvent { AuthUser = authUser });
        }

        public async Task<AuthTokenDto> Login(AuthUserLoginRequestDto request)
        {
            AuthUserEntity? user = await _userManager.FindByEmailAsync(request.Email);
            user ??= await _userManager.FindByNameAsync(request.Email);
            if (user is null)
                throw new UserLoginException();
            if (user.IsOAuth)
                throw new UserLoginException();
            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                throw new UserLoginException();
            return await _userManager.GenerateToken(user, _tokenService);
        }

        public async Task<AuthTokenDto> OAuth(OAuthUserRequestDto request)
        {
            OAuthUserDataDto userData = await _oauthFactory.GetOAuthProvider(request.Provider).GetUserDataDto(request.AccessToken);
            AuthUserEntity? user = await _userManager.FindByEmailAsync(userData.Email);
            if (user is not null)
            {
                if (!user.IsOAuth)
                    throw new UserLoginException();
                if (user.OAuthProviderType != request.Provider)
                    throw new OAuthProviderException();
                return await _userManager.GenerateToken(user, _tokenService);
            }

            UserEntity authUser = new()
            {
                UserName = await GenerateUsername(userData.FirstName),
                Email = userData.Email,
                ProfileType = ProfileType.USER,
                FirstName = userData.FirstName,
                LastName = userData.LastName,
                IsOAuth = true
            };

            IdentityResult result = await _userManager.CreateAsync(authUser);
            if (!result.Succeeded)
                throw new UserRegisterException(result.Errors);

            if (userData.Picture is not null)
            {
                MediaEntity media = await _mediaService.CreateMedia("User_Avatar", userData.Picture, authUser);
                authUser.AvatarId = media.Id;
                await _userManager.UpdateAsync(authUser);
            }

            await _eventService.FireEvent(new AuthUserCreatedEvent { AuthUserEntity = authUser });
            return await _userManager.GenerateToken(authUser, _tokenService);
        }

        public async Task<AuthTokenDto> RefreshAccessToken(AuthUserEntity user, string accessToken, RefreshTokenRequestDto request)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
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

            if (request.AvatarAsBase64 is not null)
            {
                MediaEntity media = await _mediaService.CreateMedia("Club_Avatar", request.AvatarAsBase64.GetAsByteImage(), authUser);
                authUser.AvatarId = media.Id;
                await _userManager.UpdateAsync(authUser);
            }

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

            if (request.AvatarAsBase64 is not null)
            {
                MediaEntity media = await _mediaService.CreateMedia("User_Avatar", request.AvatarAsBase64.GetAsByteImage(), authUser);
                authUser.AvatarId = media.Id;
                await _userManager.UpdateAsync(authUser);
            }

            await _eventService.FireEvent(new AuthUserCreatedEvent { AuthUserEntity = authUser });

            return await _userManager.GenerateToken(authUser, _tokenService);
        }

        public async Task RevokeRefreshToken(AuthUserEntity authUserEntity)
        {
            authUserEntity.RefreshToken = null;
            authUserEntity.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(authUserEntity);
        }

        private async Task<string> GenerateUsername(string firstName)
        {
            if (await _userManager.FindByNameAsync(firstName) is null)
                return firstName;
            Random random = new();
            AuthUserEntity? entity;
            do
            {
                firstName += random.Next(0, 9);
                entity = await _userManager.FindByNameAsync(firstName);
            } while (entity is not null);
            return firstName;
        }
    }
}
