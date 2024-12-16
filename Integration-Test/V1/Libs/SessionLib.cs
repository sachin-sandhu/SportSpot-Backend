using Integration_Test.Extensions;
using Integration_Test.V1.Exceptions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using System.Web;

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

        public async Task<JsonObject> CreateDefaultSession(string accessToken, DateTime? dateTime = null, string sportType = null)
        {
            string title = "Session Title";
            sportType ??= "Basketball";
            string description = "Session Description";
            double latitude = 51.924470285085526;
            double longitude = 7.846992772627526;
            dateTime ??= DateTime.UtcNow.AddDays(1);
            int minParticipants = 5;
            int maxParticipants = 10;
            List<string> tags = ["tag1", "tag2"];

            HttpResponseMessage response = await CreateSessionAsync(accessToken: accessToken, title: title,
            sportType: sportType, description: description,
            latitude: latitude, longitude: longitude,
            date: dateTime, minParticipants: minParticipants, maxParticipants:
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

        public async Task<(JsonArray, bool)> SearchSessions(string accessToken, double latitude, double longitude, double distance, int page, int size, string sportType = null)
        {
            UriBuilder uriBuilder = new($"{_client.BaseAddress}/api/v1/session/search");
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query.Add("latitude", latitude.ToString());
            query.Add("longitude", longitude.ToString());
            query.Add("distance", distance.ToString());
            query.Add("page", page.ToString());
            query.Add("size", size.ToString());
            if (sportType != null)
                query.Add("sportType", sportType);

            uriBuilder.Query = query.ToString();
            string uri = uriBuilder.ToString()[(_client.BaseAddress.ToString().Length)..];
            HttpRequestMessage requestMessage = new(HttpMethod.Get, uri);

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await _client.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            JsonArray sessions = JsonSerializer.Deserialize<JsonArray>(responseString);
            bool hasMoreEntries = false;
            if (response.Headers.TryGetValues("X-Has-More-Entries", out IEnumerable<string> values))
                hasMoreEntries = bool.Parse(values.First());
            return (sessions, hasMoreEntries);
        }

        public async Task<JsonArray> GetSesssionFromUser(string accessToken, bool withExpired = false, int page = 0, int size = 10)
        {
            UriBuilder uriBuilder = new($"{_client.BaseAddress}/api/v1/session");
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query.Add("withExpired", withExpired.ToString());
            query.Add("page", page.ToString());
            query.Add("size", size.ToString());

            uriBuilder.Query = query.ToString();
            string uri = uriBuilder.ToString()[(_client.BaseAddress.ToString().Length)..];
            HttpRequestMessage requestMessage = new(HttpMethod.Get, uri);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await _client.SendAsync(requestMessage);
            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new NotFoundException();
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonArray>(responseString);
        }

        public static void ValidateDefaultSession(JsonObject session, Guid creatorId)
        {
            Assert.IsTrue(session.ContainsKey("id"));
            Assert.AreEqual("Basketball", session["sportType"].Value<string>());
            Assert.AreEqual(creatorId.ToString(), session["creatorId"].Value<Guid>().ToString());

            Assert.AreEqual("Session Title", session["title"].Value<string>());
            Assert.AreEqual("Session Description", session["description"].Value<string>());
            Assert.AreEqual("Everswinkel", session["location"].AsObject()["city"].Value<string>());
            Assert.AreEqual("48351", session["location"].AsObject()["zipCode"].Value<string>());
            Assert.AreEqual(5, session["minParticipants"].Value<int>());
            Assert.AreEqual(10, session["maxParticipants"].Value<int>());
            Assert.AreEqual(string.Join(' ', ["tag1", "tag2"]), string.Join(' ', session["tags"].AsArray().Select(x => x.Value<string>()).ToList()));
        }
    }
}
