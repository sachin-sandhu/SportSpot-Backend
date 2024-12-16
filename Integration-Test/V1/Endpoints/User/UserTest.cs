using Integration_Test.Extensions;
using Integration_Test.V1.Libs;
using System.Net;
using System.Text.Json.Nodes;

namespace Integration_Test.V1.Endpoints.User
{
    [TestClass]
    public class UserTest
    {

        private readonly string baseUri = "http://localhost:8080/api/v1/";
        private readonly UserLib _userLib;
        private readonly CleanUpLib _cleanUpLib;

        public UserTest()
        {
            _userLib = new(baseUri);
            _cleanUpLib = new(baseUri);
        }

        [TestInitialize]
        public async Task Setup()
        {
            await _cleanUpLib.CleanUp();
        }

        [TestMethod]
        public async Task TestUpdateUser_ShouldReturnOk()
        {
            // Arrange
            JsonObject authUserData = await _userLib.CreateDefaultUser();
            string accessToken = authUserData["accessToken"].Value<string>();
            Guid userId = authUserData["userId"].Value<Guid>();

            string email = "max2.musterman@gmail.com";
            string password = "password1.G.222";
            string firstname = "Nic";
            string lastname = "Cool";
            DateTime dateOfBirth = new(1994, 7, 5, 0, 0, 0, DateTimeKind.Utc);
            string biography = "I am cool";
            string avatar = UserLib.GetDefaultPictureAsBase64();

            // Act
            HttpResponseMessage response = await _userLib.UpdateUser(
                accessToken,
                email: email,
                newPassword: password,
                oldPassword: "password1.G.222",
                firstName: firstname,
                lastName: lastname,
                dateOfBirth: dateOfBirth,
                biography: biography,
                avaterAsBase64: avatar);
            response.EnsureSuccessStatusCode();

            // Assert
            JsonObject newUserData = await _userLib.GetUserById(userId, accessToken);
            Assert.AreEqual(email, newUserData["email"].Value<string>());
            Assert.AreEqual(firstname, newUserData["firstName"].Value<string>());
            Assert.AreEqual(lastname, newUserData["lastName"].Value<string>());
            Assert.AreEqual(dateOfBirth, newUserData["birthDate"].Value<DateTime>());
            Assert.AreEqual(biography, newUserData["biography"].Value<string>());
        }

        [TestMethod]
        public async Task TestUpdateUser_EmailNotUnique_ShouldReturnConflict()
        {
            // Arrange
            string duplicateEmail = "max.musterman@gmail.com";
            await _userLib.CreateDefaultUser();

            JsonObject secondUser = await _userLib.CreateDefaultUser(true);
            string secondAccessToken = secondUser["accessToken"].Value<string>();

            // Act
            HttpResponseMessage response = await _userLib.UpdateUser(
                secondAccessToken,
                email: duplicateEmail,
                oldPassword: "password1.G.222",
                newPassword: "NewPassword!123"
            );

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Expected conflict due to duplicate email.");
        }

        [TestMethod]
        public async Task TestUpdateUser_InvalidPassword_MissingUppercase_ShouldReturnBadRequest()
        {
            // Arrange
            JsonObject authUserData = await _userLib.CreateDefaultUser();
            string accessToken = authUserData["accessToken"].Value<string>();

            string invalidPassword = "password1!@"; // No uppercase letter

            // Act
            HttpResponseMessage response = await _userLib.UpdateUser(
                accessToken,
                oldPassword: "password1.G.222",
                newPassword: invalidPassword
            );

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Expected bad request due to invalid password.");
        }

        [TestMethod]
        public async Task TestUpdateUser_InvalidPassword_MissingSpecialCharacter_ShouldReturnBadRequest()
        {
            // Arrange
            JsonObject authUserData = await _userLib.CreateDefaultUser();
            string accessToken = authUserData["accessToken"].Value<string>();

            string invalidPassword = "Password123"; // No special character

            // Act
            HttpResponseMessage response = await _userLib.UpdateUser(
                accessToken,
                oldPassword: "password1.G.222",
                newPassword: invalidPassword
            );

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Expected bad request due to invalid password.");
        }

        [TestMethod]
        public async Task TestUpdateUser_InvalidPassword_MissingNumber_ShouldReturnBadRequest()
        {
            // Arrange
            JsonObject authUserData = await _userLib.CreateDefaultUser();
            string accessToken = authUserData["accessToken"].Value<string>();

            string invalidPassword = "Password!@"; // No number

            // Act
            HttpResponseMessage response = await _userLib.UpdateUser(
                accessToken,
                oldPassword: "password1.G.222",
                newPassword: invalidPassword
            );

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Expected bad request due to invalid password.");
        }

        [TestMethod]
        public async Task TestUpdateUser_InvalidPassword_MissingLowercase_ShouldReturnBadRequest()
        {
            // Arrange
            JsonObject authUserData = await _userLib.CreateDefaultUser();
            string accessToken = authUserData["accessToken"].Value<string>();

            string invalidPassword = "PASSWORD1!@"; // No lowercase letter

            // Act
            HttpResponseMessage response = await _userLib.UpdateUser(
                accessToken,
                oldPassword: "password1.G.222",
                newPassword: invalidPassword
            );

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Expected bad request due to invalid password.");
        }

        [TestMethod]
        public async Task TestUpdateUser_InvalidEmailFormat_ShouldReturnBadRequest()
        {
            // Arrange
            JsonObject authUserData = await _userLib.CreateDefaultUser();
            string accessToken = authUserData["accessToken"].Value<string>();

            string invalidEmail = "invalid-email"; // Missing domain

            // Act
            HttpResponseMessage response = await _userLib.UpdateUser(
                accessToken,
                email: invalidEmail,
                oldPassword: "password1.G.222",
                newPassword: "Password1!"
            );

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Expected bad request due to invalid email format.");
        }

        [TestMethod]
        public async Task TestUpdateUser_WrongOldPassword_ShouldReturnUnauthorized()
        {
            // Arrange
            JsonObject authUserData = await _userLib.CreateDefaultUser();
            string accessToken = authUserData["accessToken"].Value<string>();

            // Act
            HttpResponseMessage response = await _userLib.UpdateUser(
                accessToken,
                oldPassword: "wrongpassword!1",
                newPassword: "NewPassword1!"
            );

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Expected unauthorized due to wrong old password.");
        }
    }
}
