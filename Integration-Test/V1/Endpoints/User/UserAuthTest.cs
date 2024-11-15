using Integration_Test.Extensions;
using Integration_Test.V1.Libs;
using System.Text.Json.Nodes;

namespace Integration_Test.V1.Endpoints.User
{
    [TestClass]
    public class UserAuthTest
    {

        private string baseUri = "http://localhost:8080/api/v1/auth";
        private readonly UserLib _userLib;

        public UserAuthTest()
        {
            _userLib = new(baseUri);
        }

        [TestMethod]
        public async Task RegisterUserWithoutImage()
        {
            // Arrange
            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";

            // Act
            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            JsonObject authToken = JsonSerializer.Deserialize<JsonObject>(responseContent);

            // Assert
            Assert.IsNotNull(authToken, "Authentication token should not be null.");
            Assert.IsFalse(string.IsNullOrEmpty(authToken["accessToken"].Value<string>()), "Access token should not be empty or null.");
            Assert.IsTrue(authToken["accessExpire"].Value<DateTime>() > DateTime.Now, "Access token should expire in the future.");
            Assert.IsFalse(string.IsNullOrEmpty(authToken["refreshToken"].Value<string>()), "Refresh token should not be empty or null.");
            Assert.IsTrue(authToken["refreshExpire"].Value<DateTime>() > DateTime.Now, "Refresh token should expire in the future.");
        }

    }
}
