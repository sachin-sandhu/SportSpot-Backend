using NSubstitute;
using SportSpot.V1.Exceptions.Location;
using SportSpot.V1.Location.Dtos;
using SportSpot.V1.Location.Enums;
using SportSpot.V1.Location.Services;
using SportSpot_Test.Location.Mocks;
using StackExchange.Redis;
using System.Text;

namespace SportSpot_Test.Location
{
    [TestClass]
    public class LocationTest
    {

        [TestMethod]
        public async Task TestGetLocation_APINotReachable()
        {
            // Arrange
            LocationConfigDto config = new ()
            {
                AzureMapsReverseLocationEndpoint = "https://test.com",
                AzureMapsSearchEndpoint = "https://test.com",
                AzureMapsSubscriptionKey = "test"
            };

            MockRequest mockRequest = new()
            {
                Content = "{}",
                Success = false
            };

            MockDistributedCache mockDistributedCache = new();
            LocationCacheService locationCacheService = new(mockDistributedCache, Substitute.For<IConnectionMultiplexer>());
            LocationService locationService = new(mockRequest, locationCacheService, config);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<HttpRequestException>(async() => await locationService.GetLocations("nic", "DE", "de-DE", AzureGeographicEntityType.All));
        }

        [TestMethod]
        public async Task TestGetLocation_NoLocationFound()
        {
            // Arrange
            LocationConfigDto config = new()
            {
                AzureMapsReverseLocationEndpoint = "https://test.com",
                AzureMapsSearchEndpoint = "https://test.com",
                AzureMapsSubscriptionKey = "test"
            };

            MockRequest mockRequest = new()
            {
                Content = "{}",
                Success = true
            };

            MockDistributedCache mockDistributedCache = new();
            LocationCacheService locationCacheService = new(mockDistributedCache, Substitute.For<IConnectionMultiplexer>());
            LocationService locationService = new(mockRequest, locationCacheService, config);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<LocationNotFoundException>(async () => await locationService.GetLocations("nic", "DE", "de-DE", AzureGeographicEntityType.All));
        }

        [TestMethod]
        public async Task TestGetLocation_LocationFound()
        {
            // Arrange
            LocationConfigDto config = new()
            {
                AzureMapsReverseLocationEndpoint = "https://test.com",
                AzureMapsSearchEndpoint = "https://test.com",
                AzureMapsSubscriptionKey = "test"
            };

            using MemoryStream ms = new();
            ResourceHelper.GetEmbeddedFileAsStream("SportSpot_Test.Location.Files.01-LocationResponse.json").CopyTo(ms);
            MockRequest mockRequest = new()
            {
                Content = Encoding.UTF8.GetString(ms.ToArray()),
                Success = true
            };

            MockDistributedCache mockDistributedCache = new();
            LocationCacheService locationCacheService = new(mockDistributedCache, Substitute.For<IConnectionMultiplexer>());
            LocationService locationService = new(mockRequest, locationCacheService, config);

            // Act
            List<LocationDto> result = await locationService.GetLocations("nic", "DE", "de-DE", AzureGeographicEntityType.All);

            // Assert
            Assert.AreEqual(1, result.Count);

            LocationDto location = result[0];

            Assert.AreEqual("15", location.Address.StreetNumber, "Expected StreetNumber: '15'");
            Assert.AreEqual("Schuter", location.Address.StreetName, "Expected StreetName: 'Schuter'");
            Assert.AreEqual("Everswinkel", location.Address.Municipality, "Expected Municipality: 'Everswinkel'");
            Assert.AreEqual("Warendorf", location.Address.CountrySecondarySubdivision, "Expected CountrySecondarySubdivision: 'Warendorf'");
            Assert.AreEqual("Nordrhein-Westfalen", location.Address.CountrySubdivision, "Expected CountrySubdivision: 'Nordrhein-Westfalen'");
            Assert.AreEqual("Nordrhein-Westfalen", location.Address.CountrySubdivisionName, "Expected CountrySubdivisionName: 'Nordrhein-Westfalen'");
            Assert.AreEqual("48351", location.Address.PostalCode, "Expected PostalCode: '48351'");
            Assert.AreEqual("DE", location.Address.CountryCode, "Expected CountryCode: 'DE'");
            Assert.AreEqual("Deutschland", location.Address.Country, "Expected Country: 'Deutschland'");
            Assert.AreEqual("DEU", location.Address.CountryCodeISO3, "Expected CountryCodeISO3: 'DEU'");
            Assert.AreEqual("Schuter 15, 48351 Everswinkel", location.Address.FreeformAddress, "Expected FreeformAddress: 'Schuter 15, 48351 Everswinkel'");

            Assert.AreEqual(51.90303, location.Position.Lat, "Expected Latitude: 51.90303");
            Assert.AreEqual(7.84269, location.Position.Lon, "Expected Longitude: 7.84269");
        }

        [TestMethod]
        public async Task TestGetLocation_LocationFound_UseCache()
        {
            // Arrange
            LocationConfigDto config = new()
            {
                AzureMapsReverseLocationEndpoint = "https://test.com",
                AzureMapsSearchEndpoint = "https://test.com",
                AzureMapsSubscriptionKey = "test"
            };

            using MemoryStream ms = new();
            ResourceHelper.GetEmbeddedFileAsStream("SportSpot_Test.Location.Files.01-LocationResponse.json").CopyTo(ms);
            MockRequest mockRequest = new()
            {
                Content = Encoding.UTF8.GetString(ms.ToArray()),
                Success = true
            };

            MockDistributedCache mockDistributedCache = new();
            LocationCacheService locationCacheService = new(mockDistributedCache, Substitute.For<IConnectionMultiplexer>());
            LocationService locationService = new(mockRequest, locationCacheService, config);

            // Act
            List<LocationDto> result = await locationService.GetLocations("nic", "DE", "de-DE", AzureGeographicEntityType.All);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(0, mockDistributedCache.Counter);
            Assert.AreEqual(1, mockDistributedCache.Count());

            List<LocationDto> cachedResult = await locationService.GetLocations("nic", "DE", "de-DE", AzureGeographicEntityType.All);
            Assert.AreEqual(1, mockDistributedCache.Counter);

            Assert.AreEqual(result.Count, cachedResult.Count, "result.Lenght, cachedResult.Length");
            Assert.AreEqual(result[0].Address.StreetNumber, cachedResult[0].Address.StreetNumber, "result[0].Address.StreetNumber, cachedResult[0].Address.StreetNumber");
            Assert.AreEqual(result[0].Address.StreetName, cachedResult[0].Address.StreetName, "result[0].Address.StreetName, cachedResult[0].Address.StreetName");
            Assert.AreEqual(result[0].Address.Municipality, cachedResult[0].Address.Municipality, "result[0].Address.Municipality, cachedResult[0].Address.Municipality");
            Assert.AreEqual(result[0].Address.CountrySecondarySubdivision, cachedResult[0].Address.CountrySecondarySubdivision, "result[0].Address.CountrySecondarySubdivision, cachedResult[0].Address.CountrySecondarySubdivision");
            Assert.AreEqual(result[0].Address.CountrySubdivision, cachedResult[0].Address.CountrySubdivision, "result[0].Address.CountrySubdivision, cachedResult[0].Address.CountrySubdivision");
            Assert.AreEqual(result[0].Address.CountrySubdivisionName, cachedResult[0].Address.CountrySubdivisionName, "result[0].Address.CountrySubdivisionName, cachedResult[0].Address.CountrySubdivisionName");
            Assert.AreEqual(result[0].Address.PostalCode, cachedResult[0].Address.PostalCode, "result[0].Address.PostalCode, cachedResult[0].Address.PostalCode");
            Assert.AreEqual(result[0].Address.CountryCode, cachedResult[0].Address.CountryCode, "result[0].Address.CountryCode, cachedResult[0].Address.CountryCode");
            Assert.AreEqual(result[0].Address.Country, cachedResult[0].Address.Country, "result[0].Address.Country, cachedResult[0].Address.Country");
            Assert.AreEqual(result[0].Address.CountryCodeISO3, cachedResult[0].Address.CountryCodeISO3, "result[0].Address.CountryCodeISO3, cachedResult[0].Address.CountryCodeISO3");
            Assert.AreEqual(result[0].Address.FreeformAddress, cachedResult[0].Address.FreeformAddress, "result[0].Address.FreeformAddress, cachedResult[0].Address.FreeformAddress");

            Assert.AreEqual(result[0].Position.Lat, cachedResult[0].Position.Lat, "result[0].Position.Lat, cachedResult[0].Position.Lat");
            Assert.AreEqual(result[0].Position.Lon, cachedResult[0].Position.Lon, "result[0].Position.Lon, cachedResult[0].Position.Lon");
        }
    }
}
