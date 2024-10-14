using Microsoft.Extensions.Caching.Distributed;

namespace SportSpot.V1.Location
{
    public class LocationCacheService(IDistributedCache _distributedCache) : ILocationCacheService
    {
        public async Task<AzureAddressDto?> FindAddress(string language, double lat, double lng)
        {
            var addressRaw = await _distributedCache.GetStringAsync(BuildReverseKey(language, lat, lng));
            if (addressRaw == null)
                return null;
            return JsonSerializer.Deserialize<AzureAddressDto>(addressRaw);
        }

        public async Task<List<LocationDto>?> FindLocations(string searchText, string country, string language, AzureGeographicEntityType entityType)
        {
            var locationRaw = await _distributedCache.GetStringAsync(BuildLocationKey(searchText, country, language, entityType));
            if (locationRaw == null)
                return null;
            return JsonSerializer.Deserialize<List<LocationDto>?>(locationRaw);
        }

        public async Task SaveAddress(string language, double lat, double lng, AzureAddressDto address)
        {
            await _distributedCache.SetStringAsync(BuildReverseKey(language, lat, lng), JsonSerializer.Serialize(address));
        }

        public async Task SaveLocations(string searchText, string country, string language, AzureGeographicEntityType entityType, List<LocationDto> locations)
        {
            await _distributedCache.SetStringAsync(BuildLocationKey(searchText, country, language, entityType), JsonSerializer.Serialize(locations));
        }

        private static string BuildReverseKey(string language, double lat, double lng) => $"reverse_{language}_{lat}_{lng}";

        private static string BuildLocationKey(string searchText, string country, string language, AzureGeographicEntityType entityType) => $"location_{country}_{language}_{searchText.ToLower()}_{entityType}";
    }
}
