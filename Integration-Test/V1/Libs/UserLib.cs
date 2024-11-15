using Integration_Test.Properties;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using static System.Net.Mime.MediaTypeNames;

namespace Integration_Test.V1.Libs
{
    public class UserLib
    {
        private readonly HttpClient _client;

        public UserLib(string baseUri)
        {
            _client = new()
            {
                BaseAddress = new Uri(baseUri)
            };
        }

        public async Task<HttpResponseMessage> RegisterUser(string username, string email, string password, string firstname, string lastname, string avatarAsBase64 = null)
        {
            JsonObject registerRequest = new()
            {
                { "username", username },
                { "email", email },
                { "password", password },
                { "firstname", firstname },
                { "lastname", lastname }
            };
            if (avatarAsBase64 != null)
            {
                registerRequest.Add("avatarAsBase64", avatarAsBase64);
            }

            StringContent content = new(registerRequest.ToJsonString(), MediaTypeHeaderValue.Parse("application/json"));
            HttpResponseMessage response = await _client.PostAsync("auth/register", content);
            return response;
        }

        public async Task<HttpResponseMessage> LoginUser(string usernameOrMail, string password)
        {
            JsonObject loginRequest = new()
            {
                { "email", usernameOrMail },
                { "password", password }
            };

            StringContent content = new(loginRequest.ToJsonString(), MediaTypeHeaderValue.Parse("application/json"));
            HttpResponseMessage response = await _client.PostAsync("auth/login", content);
            return response;
        }


        public string GetDefaultPictureAsBase64()
        {
            return Resources.TestImage;
        }

        public byte[] GetDefaultPicture()
        {
            return Convert.FromBase64String(Resources.TestImage);
        }

    }
}