using Integration_Test.Extensions;
using Integration_Test.V1.Exceptions;
using System.Net;
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

        public async Task<JsonObject> CreateDefaultSession(string accessToken)
        {
            string title = "Session Title";
            string sportType = "Basketball";
            string description = "Session Description";
            double latitude = 51.924470285085526;
            double longitude = 7.846992772627526;
            DateTime date = DateTime.Now.AddDays(1);
            int minParticipants = 5;
            int maxParticipants = 10;
            List<string> tags = ["tag1", "tag2"];

            HttpResponseMessage response = await CreateSessionAsync(accessToken: accessToken, title: title,
            sportType: sportType, description: description,
            latitude: latitude, longitude: longitude,
            date: date, minParticipants: minParticipants, maxParticipants:
            maxParticipants, tags: tags);

            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonObject>(responseString);
        }

        public async Task<JsonObject> GetSession(Guid sessionId, string accessToken)
        {
            HttpRequestMessage requestMessage = new(HttpMethod.Get, $"/api/v1/session/{sessionId}");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await _client.SendAsync(requestMessage);
            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new NotFoundException();
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonObject>(responseString);
        }

        public async Task<HttpResponseMessage> DeleteSession(Guid sessionId, string accessToken)
        {
            HttpRequestMessage requestMessage = new(HttpMethod.Delete, $"/api/v1/session/{sessionId}");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await _client.SendAsync(requestMessage);
        }

        public async Task<HttpResponseMessage> JoinSession(Guid sessionId, string accessToken)
        {
            HttpRequestMessage requestMessage = new(HttpMethod.Put, $"/api/v1/session/{sessionId}/join");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await _client.SendAsync(requestMessage);
        }

        public async Task<HttpResponseMessage> LeaveSession(Guid sessionId, string accessToken)
        {
            HttpRequestMessage requestMessage = new(HttpMethod.Put, $"/api/v1/session/{sessionId}/leave");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await _client.SendAsync(requestMessage);
        }

        public async Task<HttpResponseMessage> KickUserFromSession(Guid sessionId, Guid userID, string accessToken)
        {
            HttpRequestMessage requestMessage = new(HttpMethod.Delete, $"/api/v1/session/{sessionId}/{userID}");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await _client.SendAsync(requestMessage);
        }
    }
}
