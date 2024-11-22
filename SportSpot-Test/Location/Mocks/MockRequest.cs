using SportSpot.V1.Request;
using System.Net;

namespace SportSpot_Test.Location.Mocks
{
    internal class MockRequest : IRequest
    {
        public string Content { get; set; } = string.Empty;
        public bool Success { get; set; } = true;

        public Task<HttpResponseMessage> Get(string url, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParameters = null, string accept = "application/json")
        {
            return Task.FromResult(new HttpResponseMessage
            {
                Content = new StringContent(Content),
                StatusCode = Success ? HttpStatusCode.OK : HttpStatusCode.InternalServerError
            });
        }

        public Task<HttpResponseMessage> Post(string url, string body, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParameters = null, string accept = "application/json", string contentType = "application/json")
        {
            return Task.FromResult(new HttpResponseMessage
            {
                Content = new StringContent(Content),
                StatusCode = Success ? HttpStatusCode.OK : HttpStatusCode.InternalServerError
            });
        }
    }
}
