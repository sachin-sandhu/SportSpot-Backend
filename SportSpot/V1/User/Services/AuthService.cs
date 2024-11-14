using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using SportSpot.Events.Services;
using SportSpot.V1.Exceptions.User;
using SportSpot.V1.Extensions;
using SportSpot.V1.Media.Entities;
using SportSpot.V1.Media.Services;
using SportSpot.V1.User.Context;
using SportSpot.V1.User.Dtos.Auth;
using SportSpot.V1.User.Dtos.Auth.OAuth;
using SportSpot.V1.User.Entities;
using SportSpot.V1.User.Events;
using SportSpot.V1.User.Extensions;
using SportSpot.V1.User.OAuth;

namespace SportSpot.V1.User.Services
{
    public class AuthService(IEventService _eventService, IMediaService _mediaService, IOAuthFactory _oauthFactory, UserManager<AuthUserEntity> _userManager, ITokenService _tokenService, AuthContext _dbContext) : IAuthService
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
            using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(CancellationToken.None);
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

            AuthUserEntity authUser = new()
            {
                UserName = await GenerateUsername(userData.FirstName),
                Email = userData.Email,
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

        public async Task<AuthTokenDto> Register(AuthUserRegisterRequestDto request)
        {
            using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(CancellationToken.None);
            AuthUserEntity authUser = new()
            {
                UserName = request.Username,
                Email = request.Email,
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
            AuthTokenDto token = await _userManager.GenerateToken(authUser, _tokenService);
            await transaction.CommitAsync();
            return token;
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
