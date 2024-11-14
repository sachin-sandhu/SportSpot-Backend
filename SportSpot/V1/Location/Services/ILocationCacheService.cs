using SportSpot.V1.Location.Dtos;
using SportSpot.V1.Location.Enums;

namespace SportSpot.V1.Location.Services
{
    public interface ILocationCacheService
    {
        Task<List<LocationDto>?> FindLocations(string searchText, string country, string language, AzureGeographicEntityType entityType);
        Task<AzureAddressDto?> FindAddress(string language, double lat, double lng);
        Task SaveAddress(string language, double lat, double lng, AzureAddressDto address);
        Task SaveLocations(string searchText, string country, string language, AzureGeographicEntityType entityType, List<LocationDto> locations);
    }
}
