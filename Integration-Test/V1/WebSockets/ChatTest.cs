using Integration_Test.Extensions;
using Integration_Test.V1.Libs;
using Rest_Emulator.Enums;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Nodes;

namespace Integration_Test.V1.WebSockets
{
    [TestClass]
    public class ChatTest
    {

        private readonly string baseUri = "http://localhost:8080/api/v1/";
        private readonly string webSocketUri = "ws://localhost:8080/ws";
        private readonly string emulatorUri = "http://localhost:8083/";
        private readonly UserLib _userLib;
        private readonly CleanUpLib _cleanUpLib;
        private readonly SessionLib _sessionLib;
        private readonly ChatLib _chatLib;
        private readonly RestEmulatorLib _emulatorLib;


        public ChatTest()
        {
            _userLib = new(baseUri);
            _cleanUpLib = new(baseUri);
            _sessionLib = new(baseUri);
            _chatLib = new(baseUri);
            _emulatorLib = new(emulatorUri);
        }

        [TestInitialize]
        public async Task Setup()
        {
            await _cleanUpLib.CleanUp();
        }

        [TestMethod]
        public async Task TestWebSocket_SendAndReceive()
        {
            // Arrange: Set emulator mode and create user, session
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, LocationLib.GetDefaultReverseResponse().ToJsonString());
            JsonObject user = await _userLib.CreateDefaultUser();
            string accessToken = user["accessToken"].Value<string>();
            JsonObject session = await _sessionLib.CreateDefaultSession(accessToken);
            Guid sessionId = session["id"].Value<Guid>();

            // Arrange: Setup WebSocket client and connect
            ClientWebSocket clientWebSocket = new();
            Uri uri = new(webSocketUri);
            clientWebSocket.Options.SetRequestHeader("Sec-WebSocket-Protocol", accessToken);
            await clientWebSocket.ConnectAsync(uri, CancellationToken.None);

            // Arrange: Prepare message payload
            JsonObject requestMessage = new()
            {
                { "MessageType", "MessageSendRequest" },
                { "SessionId", sessionId },
                { "Content", "Test Message" }
            };

            // Act: Send the message multiple times
            string messagePayload = requestMessage.ToJsonString();
            for (int i = 0; i < 4; i++)
            {
                await clientWebSocket.SendAsync(
                    new ArraySegment<byte>(Encoding.UTF8.GetBytes(messagePayload)),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }

            // Act: Receive the WebSocket response
            byte[] buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            // Act: Extract and deserialize the response message
            byte[] slicedBuffer = buffer[..result.Count];
            string rawResponse = Encoding.UTF8.GetString(slicedBuffer);
            JsonObject responseMessage = JsonSerializer.Deserialize<JsonObject>(rawResponse);

            // Assert: Validate WebSocket response
            Assert.AreEqual("MessageSendResponse", responseMessage["MessageType"].Value<string>());
            JsonObject message = responseMessage["Message"].AsObject();
            Assert.AreEqual("Test Message", message["Content"].Value<string>());
            Assert.AreEqual(user["userId"].Value<string>(), message["CreatorId"].Value<string>());
            Assert.AreEqual(sessionId.ToString(), message["SessionId"].Value<string>());
            Assert.IsNotNull(message["CreatedAt"].Value<DateTime>());

            // Act: Close the WebSocket connection
            await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Close Test Connection", CancellationToken.None);

            // Act: Retrieve messages via REST API
            HttpResponseMessage messageGetResponse = await _chatLib.GetMessages(accessToken, sessionId, size: 10, page: 0);
            messageGetResponse.EnsureSuccessStatusCode();

            // Assert: Validate retrieved messages
            string rawResponseMessages = await messageGetResponse.Content.ReadAsStringAsync();
            JsonArray messages = JsonSerializer.Deserialize<JsonArray>(rawResponseMessages);
            Assert.AreEqual(4, messages.Count);

            message = messages[0].AsObject();
            Assert.AreEqual("Test Message", message["content"].Value<string>());
            Assert.AreEqual(user["userId"].Value<string>(), message["creatorId"].Value<string>());
            Assert.AreEqual(sessionId.ToString(), message["sessionId"].Value<string>());
            Assert.IsNotNull(message["createdAt"].Value<DateTime>());
        }


        [TestMethod]
        public async Task TestWebSocket_ReceiveMessagesAndVerifyGetMessages()
        {
            // Arrange: Set emulator mode and create user, session
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, LocationLib.GetDefaultReverseResponse().ToJsonString());
            JsonObject user = await _userLib.CreateDefaultUser();
            string accessToken = user["accessToken"].Value<string>();
            JsonObject session = await _sessionLib.CreateDefaultSession(accessToken);
            Guid sessionId = session["id"].Value<Guid>();

            // Arrange: Setup WebSocket client and connect
            ClientWebSocket clientWebSocket = new();
            Uri uri = new(webSocketUri);
            clientWebSocket.Options.SetRequestHeader("Sec-WebSocket-Protocol", accessToken);
            await clientWebSocket.ConnectAsync(uri, CancellationToken.None);

            // Act: Send multiple messages via WebSocket
            for (int i = 1; i <= 3; i++)
            {
                JsonObject requestMessage = new()
                {
                    { "MessageType", "MessageSendRequest" },
                    { "SessionId", sessionId },
                    { "Content", $"Test Message {i}" }
                };

                await clientWebSocket.SendAsync(
                    new ArraySegment<byte>(Encoding.UTF8.GetBytes(requestMessage.ToJsonString())),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }

            // Act: Receive messages and validate WebSocket responses
            int receivedCount = 0;
            while (receivedCount < 3)
            {
                byte[] buffer = new byte[1024 * 4];
                WebSocketReceiveResult result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                string rawResponse = Encoding.UTF8.GetString(buffer, 0, result.Count);
                JsonObject responseMessage = JsonSerializer.Deserialize<JsonObject>(rawResponse);

                // Assert: Validate each received message
                Assert.AreEqual("MessageSendResponse", responseMessage["MessageType"].Value<string>());
                JsonObject message = responseMessage["Message"].AsObject();
                Assert.IsTrue(message["Content"].Value<string>().StartsWith("Test Message"));
                receivedCount++;
            }

            // Act: Close WebSocket connection
            await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);

            // Act: Retrieve messages via REST API
            HttpResponseMessage getMessagesResponse = await _chatLib.GetMessages(accessToken, sessionId, size: 10, page: 0);
            getMessagesResponse.EnsureSuccessStatusCode();

            string rawGetMessagesResponse = await getMessagesResponse.Content.ReadAsStringAsync();
            JsonArray messages = JsonSerializer.Deserialize<JsonArray>(rawGetMessagesResponse);

            // Assert: Validate the retrieved messages
            Assert.AreEqual(3, messages.Count);
            for (int i = 0; i < 3; i++)
            {
                Assert.IsTrue(messages[i].AsObject()["content"].Value<string>().StartsWith("Test Message"));
            }
        }


        [TestMethod]
        public async Task TestWebSocket_FilterMessagesByTimeRange()
        {
            // Arrange: Set emulator mode and create user, session
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, LocationLib.GetDefaultReverseResponse().ToJsonString());
            JsonObject user = await _userLib.CreateDefaultUser();
            string accessToken = user["accessToken"].Value<string>();
            JsonObject session = await _sessionLib.CreateDefaultSession(accessToken);
            Guid sessionId = session["id"].Value<Guid>();

            // Arrange: Setup WebSocket client and connect
            ClientWebSocket clientWebSocket = new();
            Uri uri = new(webSocketUri);
            clientWebSocket.Options.SetRequestHeader("Sec-WebSocket-Protocol", accessToken);
            await clientWebSocket.ConnectAsync(uri, CancellationToken.None);

            // Act: Send the first message
            JsonObject firstMessage = new()
            {
                { "MessageType", "MessageSendRequest" },
                { "SessionId", sessionId },
                { "Content", "Message 1" }
            };
            await clientWebSocket.SendAsync(
                new ArraySegment<byte>(Encoding.UTF8.GetBytes(firstMessage.ToJsonString())),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);

            // Act: Wait to ensure timestamp difference
            await Task.Delay(5000);

            // Act: Send the second message
            JsonObject secondMessage = new()
            {
                { "MessageType", "MessageSendRequest" },
                { "SessionId", sessionId },
                { "Content", "Message 2" }
            };
            await clientWebSocket.SendAsync(
                new ArraySegment<byte>(Encoding.UTF8.GetBytes(secondMessage.ToJsonString())),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);

            // Act: Close the WebSocket connection
            await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);

            // Arrange: Define the start time for filtering messages
            DateTime startTime = DateTime.UtcNow.AddSeconds(-2);

            // Act: Retrieve messages from the server filtered by the time range
            HttpResponseMessage response = await _chatLib.GetMessages(accessToken, sessionId, startTime: startTime);
            response.EnsureSuccessStatusCode();
            string rawResponse = await response.Content.ReadAsStringAsync();
            JsonArray messages = JsonSerializer.Deserialize<JsonArray>(rawResponse);

            // Assert: Validate the filtered messages
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual("Message 2", messages[0].AsObject()["content"].Value<string>());
        }


        [TestMethod]
        public async Task TestWebSocket_FilterMessagesBySender()
        {
            // Arrange: Set emulator mode and create two users
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, LocationLib.GetDefaultReverseResponse().ToJsonString());
            JsonObject user1 = await _userLib.CreateDefaultUser();
            string accessToken1 = user1["accessToken"].Value<string>();

            JsonObject user2 = await _userLib.CreateDefaultUser(true);
            string accessToken2 = user2["accessToken"].Value<string>();

            // Arrange: Create a session and have user2 join the session
            JsonObject session = await _sessionLib.CreateDefaultSession(accessToken1);
            Guid sessionId = session["id"].Value<Guid>();
            await _sessionLib.JoinSession(sessionId, accessToken2);

            // Arrange: Setup WebSocket clients for both users
            ClientWebSocket wsUser1 = new();
            wsUser1.Options.SetRequestHeader("Sec-WebSocket-Protocol", accessToken1);
            await wsUser1.ConnectAsync(new Uri(webSocketUri), CancellationToken.None);

            ClientWebSocket wsUser2 = new();
            wsUser2.Options.SetRequestHeader("Sec-WebSocket-Protocol", accessToken2);
            await wsUser2.ConnectAsync(new Uri(webSocketUri), CancellationToken.None);

            // Act: User 1 sends a message
            JsonObject message1 = new()
            {
                { "MessageType", "MessageSendRequest" },
                { "SessionId", sessionId },
                { "Content", "Message from User 1" }
            };
            await wsUser1.SendAsync(
                new ArraySegment<byte>(Encoding.UTF8.GetBytes(message1.ToJsonString())),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);

            // Act: User 2 sends a message
            JsonObject message2 = new()
            {
                { "MessageType", "MessageSendRequest" },
                { "SessionId", sessionId },
                { "Content", "Message from User 2" }
            };
            await wsUser2.SendAsync(
                new ArraySegment<byte>(Encoding.UTF8.GetBytes(message2.ToJsonString())),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);

            // Act: Close WebSocket connections for both users
            await wsUser1.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
            await wsUser2.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);

            // Act: Retrieve messages sent by User 1
            HttpResponseMessage response = await _chatLib.GetMessages(accessToken1, sessionId, senderId: user1["userId"].Value<Guid>());
            response.EnsureSuccessStatusCode();
            string rawResponse = await response.Content.ReadAsStringAsync();
            JsonArray messages = JsonSerializer.Deserialize<JsonArray>(rawResponse);

            // Assert: Verify that only messages from User 1 are returned
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual("Message from User 1", messages[0].AsObject()["content"].Value<string>());
        }
    }
}
