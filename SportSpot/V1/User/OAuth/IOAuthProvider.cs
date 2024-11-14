using SportSpot.V1.User.Dtos.Auth;

namespace SportSpot.V1.User.OAuth
{
    public interface IOAuthProvider
    {
        Task<OAuthUserDataDto> GetUserDataDto(string accessToken);
    }
}
