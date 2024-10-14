namespace SportSpot.V1.Request
{
    public interface IRequest
    {
        Task<HttpResponseMessage> Get(string url, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParameters = null, string accept = "application/json");
        Task<HttpResponseMessage> Post(string url, string body, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParameters = null, string accept = "application/json", string contentType = "application/json");
    }
}
