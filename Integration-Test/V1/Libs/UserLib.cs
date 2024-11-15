using System.Net.Http.Headers;
using System.Text.Json.Nodes;

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

        public async Task<HttpResponseMessage> RegisterUser(string username, string email, string password, string firstname, string lastname, byte[] avatar = null)
        {
            JsonObject registerRequest = new()
            {
                { "username", username },
                { "email", email },
                { "password", password },
                { "firstname", firstname },
                { "lastname", lastname }
            };
            if (avatar != null)
            {
                string avatarAsBase64 = Convert.ToBase64String(avatar);
                registerRequest.Add("avatar", avatarAsBase64);
            }

            StringContent content = new(registerRequest.ToJsonString(), MediaTypeHeaderValue.Parse("application/json"));
            HttpResponseMessage response = await _client.PostAsync("auth/register", content);
            return response;
        }

    }
}