﻿using SportSpot.V1.User.Dtos.Auth;
using SportSpot.V1.User.Dtos.Auth.OAuth;
using SportSpot.V1.User.Entities;

namespace SportSpot.V1.User.Services
{
    public interface IAuthService
    {
        Task<AuthTokenDto> Register(AuthUserRegisterRequestDto request);
        Task<AuthTokenDto> Login(AuthUserLoginRequestDto request);
        Task<AuthTokenDto> OAuth(OAuthUserRequestDto request);
        Task Delete(AuthUserEntity authUser);
        Task DeleteAllUser();
        Task<AuthTokenDto> RefreshAccessToken(RefreshTokenRequestDto request);
        Task RevokeRefreshToken(AuthUserEntity authUserEntity, string accessToken);
        Task<AuthUserEntity> AuthorizeUser(string authorization);
    }
}
