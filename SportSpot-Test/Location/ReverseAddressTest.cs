using NSubstitute;
using SportSpot.V1.Exceptions.Location;
using SportSpot.V1.Location.Dtos;
using SportSpot.V1.Location.Services;
using SportSpot_Test.Location.Mocks;
using StackExchange.Redis;
using System.Text;

namespace SportSpot_Test.Location
{
    [TestClass]
    public class ReverseAddressTest
    {

        [TestMethod]
        public async Task TestGetAddress_APINotReachable()
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
                Success = false
            };

            MockDistributedCache mockDistributedCache = new();
            LocationCacheService locationCacheService = new(mockDistributedCache, Substitute.For<IConnectionMultiplexer>());
            LocationService locationService = new(mockRequest, locationCacheService, config);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<HttpRequestException>(() => locationService.GetAddress("DE-de", 51.903034, 7.842505));
        }

        [TestMethod]
        public async Task TestGetAddress_NoAddressFound()
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
            await Assert.ThrowsExceptionAsync<LocationNotFoundException>(() => locationService.GetAddress("DE-de", 51.903034, 7.842505));
        }

        [TestMethod]
        public async Task TestGetAddress_AddressFound()
        {
            LocationConfigDto config = new()
            {
                AzureMapsReverseLocationEndpoint = "https://test.com",
                AzureMapsSearchEndpoint = "https://test.com",
                AzureMapsSubscriptionKey = "test"
            };

            using MemoryStream ms = new();
            ResourceHelper.GetEmbeddedFileAsStream("SportSpot_Test.Location.Files.01-ReverseAddressResponse.json").CopyTo(ms);
            MockRequest mockRequest = new()
            {
                Content = Encoding.UTF8.GetString(ms.ToArray()),
                Success = true
            };

            MockDistributedCache mockDistributedCache = new();
            LocationCacheService locationCacheService = new(mockDistributedCache, Substitute.For<IConnectionMultiplexer>());
            LocationService locationService = new(mockRequest, locationCacheService, config);

            // Act
            AzureAddressDto result = await locationService.GetAddress("DE-de", 51.903034, 7.842505);

            // Assert
            Assert.AreEqual("15", result.StreetNumber, "Expected StreetNumber: '15'");
            Assert.AreEqual("Schuter", result.StreetName, "Expected StreetName: 'Schuter'");
            Assert.AreEqual("Everswinkel", result.Municipality, "Expected Municipality: 'Everswinkel'");
            Assert.AreEqual("Warendorf", result.CountrySecondarySubdivision, "Expected CountrySecondarySubdivision: 'Warendorf'");
            Assert.AreEqual("Nordrhein-Westfalen", result.CountrySubdivision, "Expected CountrySubdivision: 'Nordrhein-Westfalen'");
            Assert.AreEqual("Nordrhein-Westfalen", result.CountrySubdivisionName, "Expected CountrySubdivisionName: 'Nordrhein-Westfalen'");
            Assert.AreEqual("48351", result.PostalCode, "Expected PostalCode: '48351'");
            Assert.AreEqual("DE", result.CountryCode, "Expected CountryCode: 'DE'");
            Assert.AreEqual("Deutschland", result.Country, "Expected Country: 'Deutschland'");
            Assert.AreEqual("DEU", result.CountryCodeISO3, "Expected CountryCodeISO3: 'DEU'");
            Assert.AreEqual("Schuter 15, 48351 Everswinkel", result.FreeformAddress, "Expected FreeformAddress: 'Schuter 15, 48351 Everswinkel'");
        }

        [TestMethod]
        public async Task TestGetAddress_AddressFound_UseCache()
        {
            // Assert
            LocationConfigDto config = new()
            {
                AzureMapsReverseLocationEndpoint = "https://test.com",
                AzureMapsSearchEndpoint = "https://test.com",
                AzureMapsSubscriptionKey = "test"
            };

            using MemoryStream ms = new();
            ResourceHelper.GetEmbeddedFileAsStream("SportSpot_Test.Location.Files.01-ReverseAddressResponse.json").CopyTo(ms);
            MockRequest mockRequest = new()
            {
                Content = Encoding.UTF8.GetString(ms.ToArray()),
                Success = true
            };

            MockDistributedCache mockDistributedCache = new();
            LocationCacheService locationCacheService = new(mockDistributedCache, Substitute.For<IConnectionMultiplexer>());
            LocationService locationService = new(mockRequest, locationCacheService, config);

            // Act
            AzureAddressDto result = await locationService.GetAddress("DE-de", 51.903034, 7.842505);

            // Assert
            Assert.AreEqual(0, mockDistributedCache.Counter);
            Assert.AreEqual(1, mockDistributedCache.Count());

            // Act
            AzureAddressDto cachedResult = await locationService.GetAddress("DE-de", 51.903034, 7.842505);

            // Assert
            Assert.AreEqual(1, mockDistributedCache.Counter);

            // Assert
            Assert.AreEqual(result.StreetNumber, cachedResult.StreetNumber, "result.StreetNumber, cachedResult.StreetNumber");
            Assert.AreEqual(result.StreetName, cachedResult.StreetName, "result.StreetName, cachedResult.StreetName");
            Assert.AreEqual(result.Municipality, cachedResult.Municipality, "result.Municipality, cachedResult.Municipality");
            Assert.AreEqual(result.CountrySecondarySubdivision, cachedResult.CountrySecondarySubdivision, "result.CountrySecondarySubdivision, cachedResult.CountrySecondarySubdivision");
            Assert.AreEqual(result.CountrySubdivision, cachedResult.CountrySubdivision, "result.CountrySubdivision, cachedResult.CountrySubdivision");
            Assert.AreEqual(result.CountrySubdivisionName, cachedResult.CountrySubdivisionName, "result.CountrySubdivisionName, cachedResult.CountrySubdivisionName");
            Assert.AreEqual(result.PostalCode, cachedResult.PostalCode, "result.PostalCode, cachedResult.PostalCode");
            Assert.AreEqual(result.CountryCode, cachedResult.CountryCode, "result.CountryCode, cachedResult.CountryCode");
            Assert.AreEqual(result.Country, cachedResult.Country, "result.Country, cachedResult.Country");
            Assert.AreEqual(result.CountryCodeISO3, cachedResult.CountryCodeISO3, "result.CountryCodeISO3, cachedResult.CountryCodeISO3");
            Assert.AreEqual(result.FreeformAddress, cachedResult.FreeformAddress, "result.FreeformAddress, cachedResult.FreeformAddress");
        }

    }
}
