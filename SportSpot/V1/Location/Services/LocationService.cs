using SportSpot.V1.Exceptions.Location;
using SportSpot.V1.Location.Dtos;
using SportSpot.V1.Location.Enums;
using SportSpot.V1.Request;

namespace SportSpot.V1.Location.Services
{
    public class LocationService(IRequest _request, ILocationCacheService _cache, LocationConfigDto _config) : ILocationService
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<AzureAddressDto> GetAddress(string language, double lat, double lng)
        {
            AzureAddressDto? cacheAddress = await _cache.FindAddress(language, lat, lng);
            if (cacheAddress is not null)
                return cacheAddress;

            Dictionary<string, string> queryParams = GetDefaultQueryParams(language, $"{lat},{lng}");
            HttpResponseMessage response = await _request.Get(_config.AzureMapsReverseLocationEndpoint, queryParameters: queryParams);
            response.EnsureSuccessStatusCode();

            AzureReverseResponseDto reverseResponse = JsonSerializer.Deserialize<AzureReverseResponseDto>(await response.Content.ReadAsStringAsync(), _options) ?? throw new LocationNotFoundException();

            if (reverseResponse.Addresses.Count == 0)
                throw new LocationNotFoundException();

            AzureAddressDto address = reverseResponse.Addresses[0].Address;

            await _cache.SaveAddress(language, lat, lng, address);
            return address;
        }

        public async Task<List<LocationDto>> GetLocations(string searchText, string country, string language, AzureGeographicEntityType entityType)
        {
            List<LocationDto>? cacheLocations = await _cache.FindLocations(searchText, country, language, entityType);
            if (cacheLocations is not null)
                return cacheLocations;

            Dictionary<string, string> queryParams = GetDefaultQueryParams(language, searchText);
            queryParams.Add("typeahead", "true");
            if (entityType != AzureGeographicEntityType.All)
                queryParams["typeahead"] = entityType.ToString();
            queryParams.Add("countrySet", country);

            HttpResponseMessage response = await _request.Get(_config.AzureMapsSearchEndpoint, queryParameters: queryParams);
            response.EnsureSuccessStatusCode();

            AzureSearchResponseDto searcheResponse = JsonSerializer.Deserialize<AzureSearchResponseDto>(await response.Content.ReadAsStringAsync(), _options) ?? throw new LocationNotFoundException();
            if (searcheResponse.Results.Count == 0)
                throw new LocationNotFoundException();

            List<LocationDto> locationList = searcheResponse.Results.Select(location =>
                new LocationDto
                {
                    Address = location.Address ?? throw new LocationNotFoundException(),
                    Position = location.Position ?? throw new LocationNotFoundException()
                }).ToList();

            await _cache.SaveLocations(searchText, country, language, entityType, locationList);
            return locationList;
        }

        private Dictionary<string, string> GetDefaultQueryParams(string language, string query)
        {
            return new Dictionary<string, string>
            {
                { "subscription-key", _config.AzureMapsSubscriptionKey },
                { "api-version", "1.0" },
                { "language", language },
                { "query", query }
            };
        }

    }
}
