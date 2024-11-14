using SportSpot.V1.Exceptions.User;
using SportSpot.V1.Request;
using SportSpot.V1.User.Dtos.Auth;
using SportSpot.V1.User.Dtos.Auth.OAuth;
using SportSpot.V1.User.OAuth;

namespace SportSpot.V1.User.OAuth.Types
{
    public class GoogleOAuthProvider(OAuthConfigurationDto _config, IRequest _request) : IOAuthProvider
    {
        public async Task<OAuthUserDataDto> GetUserDataDto(string accessToken)
        {
            try
            {
                HttpResponseMessage response = await _request.Get(_config.GoogleUserInformationEndpoint, headers: new Dictionary<string, string>() { { "Authorization", $"Bearer {accessToken}" } });
                response.EnsureSuccessStatusCode();

                GoogleOAuthUserDataDto userData = await response.Content.ReadFromJsonAsync<GoogleOAuthUserDataDto>() ?? throw new InvalidOAuthAccessTokenException();

                return new OAuthUserDataDto
                {
                    Email = userData.Email,
                    Name = userData.Name,
                    Picture = await DownloadPicture(userData.Picture),
                    FirstName = userData.GivenName,
                    LastName = userData.FamilyName
                };
            }
            catch (InvalidOAuthAccessTokenException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new InvalidOAuthAccessTokenException();
            }
        }

        private async Task<byte[]?> DownloadPicture(string url)
        {
            try
            {
                HttpResponseMessage response = await _request.Get(url, accept: "image/png");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
