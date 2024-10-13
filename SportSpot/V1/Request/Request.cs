using System.Text;
using System.Web;

namespace SportSpot.V1.Request
{
    public class Request(HttpClient _client) : IRequest
    {
        public async Task<HttpResponseMessage> Get(string url, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParameters = null, string accept = "application/json")
        {
            using HttpRequestMessage requestMessage = BuildRequestMessage(url, HttpMethod.Get, headers, queryParameters, accept);
            return await _client.SendAsync(requestMessage);
        }

        public async Task<HttpResponseMessage> Post(string url, string body, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParameters = null, string accept = "application/json", string contentType = "application/json")
        {
            using HttpRequestMessage requestMessage = BuildRequestMessage(url, HttpMethod.Post, headers, queryParameters, accept);
            requestMessage.Content = new StringContent(body, Encoding.UTF8, contentType);
            return await _client.PostAsync(url, new StringContent(body));
        }

        private static HttpRequestMessage BuildRequestMessage(string url, HttpMethod method, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParameters = null, string accept = "application/json")
        {
            UriBuilder uriBuilder = new(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            if (queryParameters is not null)
            {
                foreach (var queryParameter in queryParameters)
                {
                    query.Add(queryParameter.Key, queryParameter.Value);
                }
            }
            uriBuilder.Query = query.ToString();

            HttpRequestMessage requestMessage = new(HttpMethod.Get, uriBuilder.ToString());

            if (headers is not null)
            {
                foreach (var header in headers)
                {
                    if (requestMessage.Headers.Contains(header.Key))
                        requestMessage.Headers.Remove(header.Key);
                    requestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            if (requestMessage.Headers.Contains("Accept"))
                requestMessage.Headers.Remove("Accept");
            requestMessage.Headers.Add("Accept", accept);

            requestMessage.Method = method;
            return requestMessage;
        }
    }
}
