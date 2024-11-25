using System.Web;

namespace Integration_Test.V1.Libs
{
    public class ChatLib
    {

        private readonly HttpClient _client;

        public ChatLib(string baseUri)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(baseUri)
            };
        }

        public async Task<HttpResponseMessage> GetMessages(string accessToken, Guid sessionId, int size = 10, int page = 0, Guid? senderId = null, string content = null, DateTime? startTime = null, DateTime? endTime = null)
        {
            UriBuilder uriBuilder = new($"{_client.BaseAddress}/api/v1/Session/{sessionId}/Chat/messages");
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query.Add("page", page.ToString());
            query.Add("size", size.ToString());
            if (senderId.HasValue)
                query.Add("senderId", senderId.ToString());
            if (!string.IsNullOrEmpty(content))
                query.Add("content", content);
            if (startTime.HasValue)
                query.Add("startTime", startTime.Value.ToString());
            if (endTime.HasValue)
                query.Add("endTime", endTime.Value.ToString());
            uriBuilder.Query = query.ToString();
            string uri = uriBuilder.ToString()[(_client.BaseAddress.ToString().Length)..];
            HttpRequestMessage requestMessage = new(HttpMethod.Get, uri);
            requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
            HttpResponseMessage response = await _client.SendAsync(requestMessage);
            return response;
        }
    }
}
