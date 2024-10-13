namespace SportSpot.V1.User
{
    public interface IOAuthProvider
    {
        Task<OAuthUserDataDto> GetUserDataDto(string accessToken);
    }
}
