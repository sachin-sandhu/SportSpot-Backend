using Integration_Test.Extensions;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;

namespace Integration_Test.V1.Libs
{
    public class SessionLib
    {

        private readonly HttpClient _client;

        public SessionLib(string baseUrl)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        public async Task<HttpResponseMessage> CreateSessionAsync(string accessToken, string title = null, string sportType = null, string description = null, double? latitude = null, double? longitude = null, DateTime? date = null, int? minParticipants = null, int? maxParticipants = null, List<string> tags = null)
        {
            JsonObject createSession = [];
            createSession.AddIfNotNull("title", title);
            createSession.AddIfNotNull("sportType", sportType);
            createSession.AddIfNotNull("description", description);
            createSession.AddIfNotNull("latitude", latitude);
            createSession.AddIfNotNull("longitude", longitude);
            createSession.AddIfNotNull("date", date);
            createSession.AddIfNotNull("minParticipants", minParticipants);
            createSession.AddIfNotNull("maxParticipants", maxParticipants);
            createSession.AddIfNotNull("tags", tags != null ? new JsonArray([.. tags]) : null);

            StringContent content = new(createSession.ToJsonString(), Encoding.UTF8, "application/json");
            HttpRequestMessage requestMessage = new(HttpMethod.Post, "/api/v1/session")
            {
                Content = content
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await _client.SendAsync(requestMessage);
        }

    }
}
