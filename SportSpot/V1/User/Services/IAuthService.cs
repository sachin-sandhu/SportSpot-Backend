namespace SportSpot.V1.User
{
    public interface IAuthService
    {
        Task<AuthTokenDto> Register(ClubRegisterRequestDto request);
        Task<AuthTokenDto> Register(UserRegisterRequestDto request);
        Task<AuthTokenDto> Login(AuthUserLoginRequestDto request);
        Task<AuthTokenDto> RefreshAccessToken(AuthUserEntity user, RefreshTokenRequestDto request);
        Task RevokeRefreshToken(AuthUserEntity authUserEntity);
    }
}
