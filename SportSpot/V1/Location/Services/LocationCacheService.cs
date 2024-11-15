using Microsoft.Extensions.Caching.Distributed;
using SportSpot.V1.Location.Dtos;
using SportSpot.V1.Location.Enums;
using SportSpot.V1.Location.Services;
using StackExchange.Redis;

namespace SportSpot.V1.Location.Services
{
    public class LocationCacheService(IDistributedCache _distributedCache, IConnectionMultiplexer _connectionMultiplexer) : ILocationCacheService
    {
        public async Task<AzureAddressDto?> FindAddress(string language, double lat, double lng)
        {
            string? addressRaw = await _distributedCache.GetStringAsync(BuildReverseKey(language, lat, lng));
            if (addressRaw == null)
                return null;
            return JsonSerializer.Deserialize<AzureAddressDto>(addressRaw);
        }

        public async Task<List<LocationDto>?> FindLocations(string searchText, string country, string language, AzureGeographicEntityType entityType)
        {
            string? locationRaw = await _distributedCache.GetStringAsync(BuildLocationKey(searchText, country, language, entityType));
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

        public void FlushCache()
        {
            IServer cacheServer = _connectionMultiplexer.GetServers().First();
            cacheServer.Keys(pattern: "*").ToList().ForEach(key =>
            {
                _distributedCache.Remove(key.ToString());
            });
        }

        private static string BuildReverseKey(string language, double lat, double lng) => $"reverse_{language}_{lat}_{lng}";

        private static string BuildLocationKey(string searchText, string country, string language, AzureGeographicEntityType entityType) => $"location_{country}_{language}_{searchText.ToLower()}_{entityType}";
    }
}
