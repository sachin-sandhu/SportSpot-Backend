using Integration_Test.Properties;
using Integration_Test.V1.Exceptions;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
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

        public async Task<HttpResponseMessage> OAuthGoogle(string accesstoken)
        {
            JsonObject loginRequest = new()
            {
                { "accessToken", accesstoken },
                { "provider", "GOOGLE" }
            };

            StringContent content = new(loginRequest.ToJsonString(), MediaTypeHeaderValue.Parse("application/json"));
            HttpResponseMessage response = await _client.PostAsync("auth/oauth", content);
            return response;
        }

        public async Task<JsonObject> GetUserById(Guid id, string accessToken)
        {
            HttpRequestMessage httpRequestMessage = new(HttpMethod.Get, $"user/{id}");
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new NotFoundException();
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedException();
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonObject>(json);
        }

        public async Task DeleteUser(string accessToken)
        {
            HttpRequestMessage httpRequestMessage = new(HttpMethod.Delete, "auth");
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
            response.EnsureSuccessStatusCode();
        }

        public async Task<HttpResponseMessage> RefreshAccessToken(string accessToken, string refreshToken)
        {
            JsonObject requestObj = new()
            {
                { "refreshToken", refreshToken }
            };
            HttpRequestMessage httpRequestMessage = new(HttpMethod.Post, "auth/token");
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpRequestMessage.Content = new StringContent(requestObj.ToJsonString(), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
            return response;
        }

        public async Task RevokeAccessToken(string accessToken)
        {
            HttpRequestMessage httpRequestMessage = new(HttpMethod.Delete, "auth/token");
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
            response.EnsureSuccessStatusCode();
        }

        public static string GetDefaultPictureAsBase64()
        {
            return Resources.TestImage;
        }

        public static byte[] GetDefaultPicture()
        {
            return Convert.FromBase64String(Resources.TestImage);
        }

        public async Task<JsonObject> CreateDefaultUser(bool jitter = false)
        {
            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";

            if (jitter)
            {
                username += Guid.NewGuid().ToString();
                email = email.Replace("@", Guid.NewGuid().ToString() + "@");
            }

            HttpResponseMessage response = await RegisterUser(username, email, password, firstname, lastname);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonObject>(json);
        }

    }
}