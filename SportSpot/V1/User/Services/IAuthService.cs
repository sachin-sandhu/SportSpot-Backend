namespace SportSpot.V1.User
{
    public interface IAuthService
    {
        Task<AuthTokenDto> Register(ClubRegisterRequestDto request);
        Task<AuthTokenDto> Register(UserRegisterRequestDto request);
        Task<AuthTokenDto> Login(AuthUserLoginRequestDto request);
        Task<AuthTokenDto> OAuth(OAuthUserRequestDto request);
        Task Delete(AuthUserEntity authUser);
        Task<AuthTokenDto> RefreshAccessToken(AuthUserEntity user, string accessToken, RefreshTokenRequestDto request);
        Task RevokeRefreshToken(AuthUserEntity authUserEntity);
    }
}
