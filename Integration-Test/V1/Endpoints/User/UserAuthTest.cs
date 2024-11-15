using Integration_Test.Extensions;
using Integration_Test.V1.Exceptions;
using Integration_Test.V1.Libs;
using Rest_Emulator.Enums;
using System.Net;
using System.Text.Json.Nodes;

namespace Integration_Test.V1.Endpoints.User
{
    [TestClass]
    public class UserAuthTest
    {

        private readonly string baseUri = "http://localhost:8080/api/v1/";
        private readonly string emulatorUri = "http://localhost:8083/";
        private readonly UserLib _userLib;
        private readonly CleanUpLib _cleanUpLib;
        private readonly RestEmulatorLib _emulatorLib;

        public UserAuthTest()
        {
            _userLib = new(baseUri);
            _cleanUpLib = new(baseUri);
            _emulatorLib = new(emulatorUri);
        }


        [TestInitialize]
        public async Task Setup()
        {
            await _cleanUpLib.CleanUp();
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
            await ValidateToken(authToken);
        }

        [TestMethod]
        public async Task RegisterUserWithImage()
        {
            // Arrange
            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";

            string avatar = UserLib.GetDefaultPictureAsBase64();

            // Act
            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname, avatar);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            JsonObject authToken = JsonSerializer.Deserialize<JsonObject>(responseContent);

            // Assert
            await ValidateToken(authToken);
        }

        [TestMethod]
        public async Task RegisterUserWithInvalidImageWithRetry()
        {
            // Arrange
            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";

            string avatar = "ssasaa1sas";

            // Act
            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname, avatar);

            string responseContent = await response.Content.ReadAsStringAsync();
            JsonArray errorInformation = JsonSerializer.Deserialize<JsonArray>(responseContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("Invalid Base64 string", errorInformation[0].AsObject()["Message"].Value<string>());

            // Retry

            // Arrange
            avatar = UserLib.GetDefaultPictureAsBase64();

            // Act
            response = await _userLib.RegisterUser(username, email, password, firstname, lastname, avatar);

            responseContent = await response.Content.ReadAsStringAsync();
            JsonObject authToken = JsonSerializer.Deserialize<JsonObject>(responseContent);

            // Assert
            await ValidateToken(authToken);
        }

        [TestMethod]
        public async Task RegisterMultipleUser()
        {
            // Arrange

            //User One
            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";

            string avatar = UserLib.GetDefaultPictureAsBase64();

            //User Two
            string username2 = "TestUser2";
            string email2 = "max.musterman2@gmail.com";
            string password2 = "password1.G.222";
            string firstname2 = "Max";
            string lastname2 = "Musterman";

            string avatar2 = UserLib.GetDefaultPictureAsBase64();

            // Act

            //User One
            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname, avatar);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            JsonObject authToken = JsonSerializer.Deserialize<JsonObject>(responseContent);

            //User Two
            HttpResponseMessage response2 = await _userLib.RegisterUser(username2, email2, password2, firstname2, lastname2, avatar2);
            response2.EnsureSuccessStatusCode();

            string responseContent2 = await response2.Content.ReadAsStringAsync();
            JsonObject authToken2 = JsonSerializer.Deserialize<JsonObject>(responseContent2);

            // Assert
            await ValidateToken(authToken);
            await ValidateToken(authToken2);
        }

        [TestMethod]
        public async Task RegisterUserInvalidPassword()
        {
            // Arrange
            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "p";
            string firstname = "Max";
            string lastname = "Musterman";

            // Act
            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname);

            string responseContent = await response.Content.ReadAsStringAsync();
            JsonArray errorInformation = JsonSerializer.Deserialize<JsonArray>(responseContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("User.PasswordTooShort", errorInformation[0].AsObject()["Code"].Value<string>());
            Assert.AreEqual("User.PasswordRequiresNonAlphanumeric", errorInformation[1].AsObject()["Code"].Value<string>());
            Assert.AreEqual("User.PasswordRequiresDigit", errorInformation[2].AsObject()["Code"].Value<string>());
            Assert.AreEqual("User.PasswordRequiresUpper", errorInformation[3].AsObject()["Code"].Value<string>());
        }

        [TestMethod]
        public async Task RegisterUserInvalidMail()
        {
            // Arrange
            string username = "TestUser";
            string email = "max.mustermangmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";

            // Act
            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task RegisterUserEmptyRequest()
        {
            // Arrange
            string username = "";
            string email = "";
            string password = "";
            string firstname = "";
            string lastname = "";

            // Act
            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task RegisterMultipleUserSameMail()
        {
            // Arrange

            //User One
            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";

            string avatar = UserLib.GetDefaultPictureAsBase64();

            //User Two
            string username2 = "TestUser2";
            string email2 = "max.musterman@gmail.com";
            string password2 = "password1.G.222";
            string firstname2 = "Max";
            string lastname2 = "Musterman";

            string avatar2 = UserLib.GetDefaultPictureAsBase64();

            // Act
            //User One
            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname, avatar);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            JsonObject authToken = JsonSerializer.Deserialize<JsonObject>(responseContent);

            //User Two
            HttpResponseMessage response2 = await _userLib.RegisterUser(username2, email2, password2, firstname2, lastname2, avatar2);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response2.StatusCode);
            string responseContent2 = await response2.Content.ReadAsStringAsync();
            JsonArray errorInformation = JsonSerializer.Deserialize<JsonArray>(responseContent2);

            // Assert
            await ValidateToken(authToken);
            Assert.AreEqual("User.DuplicateEmail", errorInformation[0].AsObject()["Code"].Value<string>());
        }

        [TestMethod]
        public async Task RegisterMultipleUserSameUsername()
        {
            // Arrange

            //User One
            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";

            string avatar = UserLib.GetDefaultPictureAsBase64();

            //User Two
            string username2 = "TestUser";
            string email2 = "max.musterman2@gmail.com";
            string password2 = "password1.G.222";
            string firstname2 = "Max";
            string lastname2 = "Musterman";

            string avatar2 = UserLib.GetDefaultPictureAsBase64();

            // Act
            //User One
            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname, avatar);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            JsonObject authToken = JsonSerializer.Deserialize<JsonObject>(responseContent);

            //User Two
            HttpResponseMessage response2 = await _userLib.RegisterUser(username2, email2, password2, firstname2, lastname2, avatar2);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response2.StatusCode);
            string responseContent2 = await response2.Content.ReadAsStringAsync();
            JsonArray errorInformation = JsonSerializer.Deserialize<JsonArray>(responseContent2);

            // Assert
            await ValidateToken(authToken);
            Assert.AreEqual("User.DuplicateUserName", errorInformation[0].AsObject()["Code"].Value<string>());
        }

        [TestMethod]
        public async Task LoginUserWithUsernameAndMail()
        {
            // Arrange
            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";
            string avatar = UserLib.GetDefaultPictureAsBase64();

            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname, avatar);
            response.EnsureSuccessStatusCode();

            // Act Login with Username
            HttpResponseMessage loginResponse = await _userLib.LoginUser(username, password);

            loginResponse.EnsureSuccessStatusCode();

            string loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            JsonObject loginAuthToken = JsonSerializer.Deserialize<JsonObject>(loginResponseContent);

            // Assert
            await ValidateToken(loginAuthToken);

            // Act Login with Mail
            HttpResponseMessage loginResponse2 = await _userLib.LoginUser(email, password);

            loginResponse2.EnsureSuccessStatusCode();

            string loginResponseContent2 = await loginResponse2.Content.ReadAsStringAsync();
            JsonObject loginAuthToken2 = JsonSerializer.Deserialize<JsonObject>(loginResponseContent2);

            // Assert
            await ValidateToken(loginAuthToken2);
        }

        [TestMethod]
        public async Task LoginUserInvalidPassword()
        {
            // Arrange User
            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";
            string avatar = UserLib.GetDefaultPictureAsBase64();

            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname, avatar);
            response.EnsureSuccessStatusCode();

            // Act Login with wrong Password
            HttpResponseMessage loginResponse = await _userLib.LoginUser(username, password + "fake");

            string loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            JsonArray errorInformation = JsonSerializer.Deserialize<JsonArray>(loginResponseContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
            Assert.AreEqual("User.Login", errorInformation[0].AsObject()["Code"].Value<string>());
        }

        [TestMethod]
        public async Task LoginUserInvalidUsername()
        {
            // Arrange User
            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";
            string avatar = UserLib.GetDefaultPictureAsBase64();

            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname, avatar);
            response.EnsureSuccessStatusCode();

            // Act Login with wrong Password
            HttpResponseMessage loginResponse = await _userLib.LoginUser(username + "fake", password);

            string loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            JsonArray errorInformation = JsonSerializer.Deserialize<JsonArray>(loginResponseContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
            Assert.AreEqual("User.Login", errorInformation[0].AsObject()["Code"].Value<string>());
        }

        [TestMethod]
        public async Task LoginAndRegisterViaOAuth()
        {
            //Only run if the emulator is running
            if (!RunOAuthTest())
            {
                Assert.Inconclusive($"Emulator is not running");
            }

            //Register User

            //Arrange
            JsonObject googleResponse = new()
            {
                { "sub", "1234567890" },
                { "name", "Max Musterman" },
                { "given_name", "Max" },
                { "family_name", "Musterman" },
                { "picture", "https://lh3.googleusercontent.com/a/ACg8ocIPR9CpoHOlpda4UqxIyEjg0qXJu__I83NwFqIEga0Y-5si6ik=s96-c" },
                { "email", "max.musterman@gmail.com" },
                { "email_verified", true }
            };
            await _emulatorLib.SetMode(ModeType.GoogleOAuth, true, googleResponse.ToJsonString());

            //Act
            HttpResponseMessage response = await _userLib.OAuthGoogle("1234567890");
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            JsonObject authToken = JsonSerializer.Deserialize<JsonObject>(responseContent);

            // Assert
            await ValidateToken(authToken);

            //Login User

            //Act
            HttpResponseMessage response2 = await _userLib.OAuthGoogle("1234567890");
            response2.EnsureSuccessStatusCode();

            string responseContent2 = await response.Content.ReadAsStringAsync();
            JsonObject authToken2 = JsonSerializer.Deserialize<JsonObject>(responseContent2);

            // Assert
            await ValidateToken(authToken2);
        }


        [TestMethod]
        public async Task RegisterViaOAuthAndLoginWithPasswordAndRegisterWithSameName()
        {
            //Only run if the emulator is running
            if (!RunOAuthTest())
            {
                Assert.Inconclusive($"Emulator is not running");
            }

            //Register User

            //Arrange

            string mail = "max.musterman@gmail.com";

            JsonObject googleResponse = new()
            {
                { "sub", "1234567890" },
                { "name", "Max Musterman" },
                { "given_name", "Max" },
                { "family_name", "Musterman" },
                { "picture", "https://lh3.googleusercontent.com/a/ACg8ocIPR9CpoHOlpda4UqxIyEjg0qXJu__I83NwFqIEga0Y-5si6ik=s96-c" },
                { "email", mail },
                { "email_verified", true }
            };
            await _emulatorLib.SetMode(ModeType.GoogleOAuth, true, googleResponse.ToJsonString());

            //Act
            HttpResponseMessage response = await _userLib.OAuthGoogle("1234567890");
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            JsonObject authToken = JsonSerializer.Deserialize<JsonObject>(responseContent);

            // Assert
            await ValidateToken(authToken);

            //Login User

            //Act
            HttpResponseMessage loginRepsonse = await _userLib.LoginUser(mail, "FakePasswod");
            string loginResponseContent = await loginRepsonse.Content.ReadAsStringAsync();
            JsonArray errorInformationLogin = JsonSerializer.Deserialize<JsonArray>(loginResponseContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, loginRepsonse.StatusCode);
            Assert.AreEqual("User.Login", errorInformationLogin[0].AsObject()["Code"].Value<string>());

            //Register User with same Mail

            //Act
            HttpResponseMessage registerResponse = await _userLib.RegisterUser("TestUser", mail, "password1.G.222", "Max", "Musterman");
            string registerResponseContent = await registerResponse.Content.ReadAsStringAsync();
            JsonArray errorInformationRegister = JsonSerializer.Deserialize<JsonArray>(registerResponseContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, registerResponse.StatusCode);
            Assert.AreEqual("User.DuplicateEmail", errorInformationRegister[0].AsObject()["Code"].Value<string>());
        }

        [TestMethod]
        public async Task RegisterNormalThenLoginViaOAuth()
        {
            //Only run if the emulator is running
            if (!RunOAuthTest())
            {
                Assert.Inconclusive($"Emulator is not running");
            }
            //Register User

            // Arrange

            // User

            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";

            HttpResponseMessage registerResponse = await _userLib.RegisterUser(username, email, password, firstname, lastname);
            registerResponse.EnsureSuccessStatusCode();

            //OAuth

            JsonObject googleResponse = new()
            {
                { "sub", "1234567890" },
                { "name", $"{firstname} {lastname}" },
                { "given_name", firstname },
                { "family_name", lastname },
                { "picture", "https://lh3.googleusercontent.com/a/ACg8ocIPR9CpoHOlpda4UqxIyEjg0qXJu__I83NwFqIEga0Y-5si6ik=s96-c" },
                { "email", email },
                { "email_verified", true }
            };
            await _emulatorLib.SetMode(ModeType.GoogleOAuth, true, googleResponse.ToJsonString());

            //Act
            HttpResponseMessage oauthResponse = await _userLib.OAuthGoogle("1234567890");
            string oauthContent = await oauthResponse.Content.ReadAsStringAsync();
            JsonArray errorInformation = JsonSerializer.Deserialize<JsonArray>(oauthContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, oauthResponse.StatusCode);
            Assert.AreEqual("User.Login", errorInformation[0].AsObject()["Code"].Value<string>());
        }

        [TestMethod]
        public async Task DeleteRegisterdUser()
        {
            // Arrange
            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";

            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            JsonObject authToken = JsonSerializer.Deserialize<JsonObject>(responseContent);

            string accessToken = authToken["accessToken"].Value<string>();

            // Act
            await _userLib.DeleteUser(accessToken);

            // Assert
            HttpResponseMessage response2 = await _userLib.RegisterUser(username, email, password, firstname, lastname);

            Assert.AreEqual(HttpStatusCode.Created, response2.StatusCode);
        }

        [TestMethod]
        public async Task RefreshAccessToken()
        {
            // Arrange
            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";

            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            JsonObject authToken = JsonSerializer.Deserialize<JsonObject>(responseContent);

            string accessToken = authToken["accessToken"].Value<string>();
            DateTime accessTokenExpire = authToken["accessExpire"].Value<DateTime>();
            string refreshToken = authToken["refreshToken"].Value<string>();
            DateTime refreshExpire = authToken["refreshExpire"].Value<DateTime>();

            // Act
            HttpResponseMessage refreshResponse = await _userLib.RefreshAccessToken(accessToken, refreshToken);
            refreshResponse.EnsureSuccessStatusCode();

            string refreshResponseContent = await refreshResponse.Content.ReadAsStringAsync();
            JsonObject newAuthToken = JsonSerializer.Deserialize<JsonObject>(refreshResponseContent);

            Assert.IsTrue(accessTokenExpire < newAuthToken["accessExpire"].Value<DateTime>(), "The new Access-Expire Date must be greater!");
            Assert.IsTrue(refreshExpire < newAuthToken["refreshExpire"].Value<DateTime>(), "The new Refresh-Expire Date must be greater!");
            Assert.AreNotEqual(accessToken, newAuthToken["accessToken"].Value<string>(), "The new Access-Token must be different!");
            Assert.AreNotEqual(refreshToken, newAuthToken["refreshToken"].Value<string>(), "The new Refresh-Token must be different!");
        }

        [TestMethod]
        public async Task RevokeRefreshToken()
        {
            // Arrange
            string username = "TestUser";
            string email = "max.musterman@gmail.com";
            string password = "password1.G.222";
            string firstname = "Max";
            string lastname = "Musterman";

            HttpResponseMessage response = await _userLib.RegisterUser(username, email, password, firstname, lastname);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            JsonObject authToken = JsonSerializer.Deserialize<JsonObject>(responseContent);

            string accessToken = authToken["accessToken"].Value<string>();
            string refreshToken = authToken["refreshToken"].Value<string>();

            // Act
            await _userLib.RevokeAccessToken(accessToken);

            // Assert
            HttpResponseMessage refreshResponse = await _userLib.RefreshAccessToken(accessToken, refreshToken);

            string refreshResponseContent = await refreshResponse.Content.ReadAsStringAsync();
            JsonArray errorInformation = JsonSerializer.Deserialize<JsonArray>(refreshResponseContent);

            Assert.AreEqual(HttpStatusCode.Unauthorized, refreshResponse.StatusCode);
            Assert.AreEqual("User.InvalidToken", errorInformation[0].AsObject()["Code"].Value<string>());
        }

        private async Task ValidateToken(JsonObject authToken)
        {
            Assert.IsNotNull(authToken, "Authentication token should not be null.");
            Assert.IsFalse(string.IsNullOrEmpty(authToken["accessToken"].Value<string>()), "Access token should not be empty or null.");
            Assert.IsTrue(authToken["accessExpire"].Value<DateTime>() > DateTime.Now, "Access token should expire in the future.");
            Assert.IsFalse(string.IsNullOrEmpty(authToken["refreshToken"].Value<string>()), "Refresh token should not be empty or null.");
            Assert.IsTrue(authToken["refreshExpire"].Value<DateTime>() > DateTime.Now, "Refresh token should expire in the future.");

            await Assert.ThrowsExceptionAsync<NotFoundException>(async () => await _userLib.GetUserById(Guid.NewGuid(), authToken["accessToken"].Value<string>()));
        }

        public static bool RunOAuthTest() => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RUN_OAUTH_TEST"));
    }
}