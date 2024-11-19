using Integration_Test.Extensions;
using Integration_Test.V1.Libs;
using Rest_Emulator.Enums;
using System.Text.Json.Nodes;

namespace Integration_Test.V1.Endpoints.Session
{
    [TestClass]
    public class SessionTest
    {
        private readonly string baseUri = "http://localhost:8080/api/v1/";
        private readonly string emulatorUri = "http://localhost:8083/";
        private readonly UserLib _userLib;
        private readonly SessionLib _sessionLib;
        private readonly CleanUpLib _cleanUpLib;
        private readonly RestEmulatorLib _emulatorLib;

        public SessionTest()
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
        public async Task CreateDefaultSession()
        {
            if (!RunLocationTest())
            {
                Assert.Inconclusive($"Emulator is not running");
            }

            // Arrange
            JsonObject user = await _userLib.CreateDefaultUser();
            string token = user["accessToken"].Value<string>();
            Guid userId = user["userId"].Value<Guid>();

            string title = "Session Title";
            string sportType = "Basketball";
            string description = "Session Description";
            double latitude = 51.924470285085526;
            double longitude = 7.846992772627526;
            DateTime date = DateTime.Now.AddDays(1);
            int minParticipants = 5;
            int maxParticipants = 10;
            List<string> tags = ["tag1", "tag2"];

            JsonObject defaultReverseAddress = LocationLib.GetDefaultReverseResponse();
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, defaultReverseAddress.ToJsonString());

            // Act
            HttpResponseMessage response = await _sessionLib.CreateSessionAsync(accessToken: token, title: title,
                sportType: sportType, description: description,
                latitude: latitude, longitude: longitude,
                date: date, minParticipants: minParticipants, maxParticipants:
                maxParticipants, tags: tags);

            response.EnsureSuccessStatusCode();

            // Assert
            string rawResponse = await response.Content.ReadAsStringAsync();
            JsonObject session = JsonSerializer.Deserialize<JsonObject>(rawResponse);

            Assert.IsTrue(session.ContainsKey("id"));
            Assert.AreEqual(sportType, session["sportType"].Value<string>());
            Assert.AreEqual(userId.ToString(), session["creatorId"].Value<Guid>().ToString());
            Assert.AreEqual(userId.ToString(), session["participants"].AsArray()[0].ToString());

            Assert.AreEqual(title, session["title"].Value<string>());
            Assert.AreEqual(description, session["description"].Value<string>());
            Assert.AreEqual("Everswinkel", session["location"].AsObject()["city"].Value<string>());
            Assert.AreEqual("48351", session["location"].AsObject()["zipCode"].Value<string>());
            Assert.AreEqual(date, session["date"].Value<DateTime>());
            Assert.AreEqual(minParticipants, session["minParticipants"].Value<int>());
            Assert.AreEqual(maxParticipants, session["maxParticipants"].Value<int>());
            Assert.AreEqual(string.Join(' ', tags), string.Join(' ', session["tags"].AsArray().Select(x => x.Value<string>()).ToList()));
            Assert.AreEqual(1, session["participants"].AsArray().Count);
        }

        public static bool RunLocationTest() => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RUN_LOCATION_TEST"));
    }
}
