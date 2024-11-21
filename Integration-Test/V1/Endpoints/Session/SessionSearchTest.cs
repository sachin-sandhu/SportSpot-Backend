using Integration_Test.Extensions;
using Integration_Test.V1.Libs;
using Rest_Emulator.Enums;
using System.Text.Json.Nodes;

namespace Integration_Test.V1.Endpoints.Session
{
    [TestClass]
    public class SessionSearchTest
    {
        private readonly string baseUri = "http://localhost:8080/api/v1/";
        private readonly string emulatorUri = "http://localhost:8083/";
        private readonly UserLib _userLib;
        private readonly SessionLib _sessionLib;
        private readonly CleanUpLib _cleanUpLib;
        private readonly RestEmulatorLib _emulatorLib;

        public SessionSearchTest()
        {
            _sessionLib = new SessionLib(baseUri);
            _userLib = new UserLib(baseUri);
            _cleanUpLib = new CleanUpLib(baseUri);
            _emulatorLib = new RestEmulatorLib(emulatorUri);
        }

        [TestInitialize]
        public async Task Setup()
        {
            await _cleanUpLib.CleanUp();
        }

        [TestMethod]
        public async Task TestSearchSessionInDistance()
        {
            // Arrange
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, LocationLib.GetDefaultReverseResponse().ToJsonString());

            JsonObject createUser = await _userLib.CreateDefaultUser();
            string createUserToken = createUser["accessToken"].Value<string>();

            _ = await _sessionLib.CreateDefaultSession(createUserToken);

            JsonObject getUser = await _userLib.CreateDefaultUser(true);
            string getUserToken = getUser["accessToken"].Value<string>();

            // Act
            JsonArray searchResults = await _sessionLib.SearchSessions(getUserToken,
                latitude: 51.514244,
                longitude: 7.468429, distance: 53, 0, 1);

            // Assert
            Assert.AreEqual(1, searchResults.Count);

            JsonObject getSession = searchResults[0].AsObject();
            SessionLib.ValidateDefaultSession(getSession, createUser["userId"].Value<Guid>());
        }

        [TestMethod]
        public async Task TestSearchSessionNotInDistance()
        {
            // Arrange
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, LocationLib.GetDefaultReverseResponse().ToJsonString());

            JsonObject createUser = await _userLib.CreateDefaultUser();
            string createUserToken = createUser["accessToken"].Value<string>();

            _ = await _sessionLib.CreateDefaultSession(createUserToken);

            JsonObject getUser = await _userLib.CreateDefaultUser(true);
            string getUserToken = getUser["accessToken"].Value<string>();

            // Act
            JsonArray searchResults = await _sessionLib.SearchSessions(getUserToken,
                latitude: 51.514244,
                longitude: 7.468429, distance: 1, 0, 1);

            // Assert
            Assert.AreEqual(0, searchResults.Count);
        }

        [TestMethod]
        public async Task TestSearchSessionInDistanceAndSportType()
        {
            // Arrange
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, LocationLib.GetDefaultReverseResponse().ToJsonString());

            JsonObject createUser = await _userLib.CreateDefaultUser();
            string createUserToken = createUser["accessToken"].Value<string>();

            await _sessionLib.CreateDefaultSession(createUserToken, sportType: "Basketball");
            await _sessionLib.CreateDefaultSession(createUserToken, sportType: "Basketball");
            await _sessionLib.CreateDefaultSession(createUserToken, sportType: "Volleyball");

            JsonObject getUser = await _userLib.CreateDefaultUser(true);
            string getUserToken = getUser["accessToken"].Value<string>();

            // Act
            JsonArray searchResultsBasketball = await _sessionLib.SearchSessions(getUserToken,
                latitude: 51.514244,
                longitude: 7.468429, distance: 100, 0, 10, sportType: "Basketball");
            JsonArray searchResultsVolleyball = await _sessionLib.SearchSessions(getUserToken,
                latitude: 51.514244,
                longitude: 7.468429, distance: 100, 0, 10, sportType: "Volleyball");

            // Assert
            Assert.AreEqual(2, searchResultsBasketball.Count);
            Assert.AreEqual(1, searchResultsVolleyball.Count);
        }

        [TestMethod]
        public async Task TestSearchSessionInDistanceButInPast()
        {
            // Arrange
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, LocationLib.GetDefaultReverseResponse().ToJsonString());

            JsonObject createUser = await _userLib.CreateDefaultUser();
            string createUserToken = createUser["accessToken"].Value<string>();

            _ = await _sessionLib.CreateDefaultSession(createUserToken, DateTime.Now.AddSeconds(5));

            JsonObject getUser = await _userLib.CreateDefaultUser(true);
            string getUserToken = getUser["accessToken"].Value<string>();

            await Task.Delay(5000);

            // Act
            JsonArray searchResults = await _sessionLib.SearchSessions(getUserToken,
                latitude: 51.514244,
                longitude: 7.468429, distance: 100, 0, 1);

            // Assert
            Assert.AreEqual(0, searchResults.Count);
        }

        [TestMethod]
        public async Task TestSearchSessionInDistanceFromOwner()
        {
            // Arrange
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, LocationLib.GetDefaultReverseResponse().ToJsonString());

            JsonObject createUser = await _userLib.CreateDefaultUser();
            string createUserToken = createUser["accessToken"].Value<string>();

            _ = await _sessionLib.CreateDefaultSession(createUserToken);

            // Act
            JsonArray searchResults = await _sessionLib.SearchSessions(createUserToken,
                latitude: 51.514244,
                longitude: 7.468429, distance: 53, 0, 1);

            // Assert
            Assert.AreEqual(0, searchResults.Count);
        }

    }
}
