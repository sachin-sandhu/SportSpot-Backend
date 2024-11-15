namespace Integration_Test.V1.Libs
{
    public class CleanUpLib
    {

        private readonly HttpClient _client;

        public CleanUpLib(string baseUrl)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        public async Task CleanUp()
        {
            HttpResponseMessage response = await _client.DeleteAsync("admin/seed-data");
            response.EnsureSuccessStatusCode();
        }
    }
}
