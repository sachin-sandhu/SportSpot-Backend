using Integration_Test.Extensions;
using Integration_Test.V1.Libs;
using Rest_Emulator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace Integration_Test.V1.Endpoints.Location
{
    [TestClass]
    public class LocationTest
    {

        private readonly string baseUri = "http://localhost:8080/api/v1/";
        private readonly string emulatorUri = "http://localhost:8083/";
        private readonly UserLib _userLib;
        private readonly CleanUpLib _cleanUpLib;
        private readonly RestEmulatorLib _emulatorLib;
        private readonly LocationLib _locationLib;

        public LocationTest()
        {
            _userLib = new UserLib(baseUri);
            _cleanUpLib = new CleanUpLib(baseUri);
            _locationLib = new LocationLib(baseUri);
            _emulatorLib = new RestEmulatorLib(emulatorUri);
        }

        [TestInitialize]
        public async Task Setup()
        {
            await _cleanUpLib.CleanUp();
        }

        [TestMethod]
        public async Task TestSearchEmptyLocation()
        {
            // Arrange
            await _emulatorLib.SetMode(ModeType.SearchLocation, true, "{}");

            JsonObject user = await _userLib.CreateDefaultUser();
            string accessToken = user["accessToken"].Value<string>();

            // Act
            HttpResponseMessage responseMessage = await _locationLib.SearchLocationsWithResponse(accessToken, "test");
            string errorInformation = await responseMessage.Content.ReadAsStringAsync();
            JsonArray error = JsonSerializer.Deserialize<JsonArray>(errorInformation);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, responseMessage.StatusCode);
            Assert.AreEqual("Location.NotFound", error[0].AsObject()["Code"].Value<string>());
        }

        [TestMethod]
        public async Task TestSearchLocation()
        {
            // Arrange
            await _emulatorLib.SetMode(ModeType.SearchLocation, true, LocationLib.GetDefaultSearchResult().ToJsonString());

            JsonObject user = await _userLib.CreateDefaultUser();
            string accessToken = user["accessToken"].Value<string>();

            // Act
            JsonArray result = await _locationLib.SearchLocations(accessToken, "test");

            // Assert
            Assert.AreEqual(1, result.Count);

            JsonObject location = result[0].AsObject();

            ValidateAddress(location["address"].AsObject());
            Assert.AreEqual(51.90303, location["position"].AsObject()["lat"].Value<double>(), "Expected Latitude: 51.90303");
            Assert.AreEqual(7.84269, location["position"].AsObject()["lon"].Value<double>(), "Expected Longitude: 7.84269");
        }

        [TestMethod]
        public async Task TestSearchLocationAzureError()
        {
            // Arrange
            await _emulatorLib.SetMode(ModeType.SearchLocation, false, LocationLib.GetDefaultSearchResult().ToJsonString());

            JsonObject user = await _userLib.CreateDefaultUser();
            string accessToken = user["accessToken"].Value<string>();

            // Act
            HttpResponseMessage responseMessage = await _locationLib.SearchLocationsWithResponse(accessToken, "test");

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, responseMessage.StatusCode);
        }

        [TestMethod]
        public async Task TestReverseLocation()
        {
            // Arrange
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, LocationLib.GetDefaultReverseResponse().ToJsonString());

            JsonObject user = await _userLib.CreateDefaultUser();
            string accessToken = user["accessToken"].Value<string>();

            // Act
            JsonObject address = await _locationLib.ReverseLocation(accessToken, latitude: 51.924470285085526, longitude: 7.846992772627526);


            // Assert
            Assert.IsNotNull(address);
            ValidateAddress(address);
        }

        [TestMethod]
        public async Task TestReverseLocationAzureError()
        {
            // Arrange
            await _emulatorLib.SetMode(ModeType.ReverseLocation, false, LocationLib.GetDefaultSearchResult().ToJsonString());

            JsonObject user = await _userLib.CreateDefaultUser();
            string accessToken = user["accessToken"].Value<string>();

            // Act
            HttpResponseMessage responseMessage = await _locationLib.ReverseLocationWithResponse(accessToken, latitude: 51.924470285085526, longitude: 7.846992772627526);

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, responseMessage.StatusCode);
        }

        [TestMethod]
        [DataRow(9999, 7.846992)]
        [DataRow(51.924470, -9999)]
        [DataRow(-91, 0)]
        [DataRow(0, 181)]
        public async Task TestReverseInvalidLocation(double latitude, double longitude)
        {
            // Arrange
            JsonObject user = await _userLib.CreateDefaultUser();
            string token = user["accessToken"].Value<string>();

            // Act
            HttpResponseMessage responseMessage = await _locationLib.ReverseLocationWithResponse(token, latitude, longitude);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, responseMessage.StatusCode);
        }

        private static void ValidateAddress(JsonObject address)
        {
            Assert.AreEqual("15", address["streetNumber"].Value<string>(), "Expected StreetNumber: '15'");
            Assert.AreEqual("Schuter", address["streetName"].Value<string>(), "Expected StreetName: 'Schuter'");
            Assert.AreEqual("Everswinkel", address["municipality"].Value<string>(), "Expected Municipality: 'Everswinkel'");
            Assert.AreEqual("Warendorf", address["countrySecondarySubdivision"].Value<string>(), "Expected CountrySecondarySubdivision: 'Warendorf'");
            Assert.AreEqual("Nordrhein-Westfalen", address["countrySubdivision"].Value<string>(), "Expected CountrySubdivision: 'Nordrhein-Westfalen'");
            Assert.AreEqual("Nordrhein-Westfalen", address["countrySubdivisionName"].Value<string>(), "Expected CountrySubdivisionName: 'Nordrhein-Westfalen'");
            Assert.AreEqual("48351", address["postalCode"].Value<string>(), "Expected PostalCode: '48351'");
            Assert.AreEqual("DE", address["countryCode"].Value<string>(), "Expected CountryCode: 'DE'");
            Assert.AreEqual("Deutschland", address["country"].Value<string>(), "Expected Country: 'Deutschland'");
            Assert.AreEqual("DEU", address["countryCodeISO3"].Value<string>(), "Expected CountryCodeISO3: 'DEU'");
            Assert.AreEqual("Schuter 15, 48351 Everswinkel", address["freeformAddress"].Value<string>(), "Expected FreeformAddress: 'Schuter 15, 48351 Everswinkel'");

        }
    }
}
