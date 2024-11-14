using SportSpot.V1.Location.Dtos;
using SportSpot.V1.Location.Enums;

namespace SportSpot.V1.Location.Services
{
    public interface ILocationService
    {
        Task<List<LocationDto>> GetLocations(string searchText, string country, string language, AzureGeographicEntityType entityType);
        Task<AzureAddressDto> GetAddress(string language, double lat, double lng);
    }
}
