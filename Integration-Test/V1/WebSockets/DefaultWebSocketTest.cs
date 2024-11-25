using Integration_Test.Extensions;
using Integration_Test.V1.Libs;
using Rest_Emulator.Enums;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Nodes;

namespace Integration_Test.V1.WebSockets
{
    [TestClass]
    public class DefaultWebSocketTest
    {

        private readonly string baseUri = "http://localhost:8080/api/v1/";
        private readonly string webSocketUri = "ws://localhost:8080/ws";
        private readonly string emulatorUri = "http://localhost:8083/";
        private readonly UserLib _userLib;
        private readonly CleanUpLib _cleanUpLib;
        private readonly SessionLib _sessionLib;
        private readonly RestEmulatorLib _emulatorLib;

        public DefaultWebSocketTest()
        {
            _userLib = new(baseUri);
            _cleanUpLib = new(baseUri);
            _sessionLib = new(baseUri);
            _emulatorLib = new(emulatorUri);
        }

        [TestInitialize]
        public async Task Setup()
        {
            await _cleanUpLib.CleanUp();
        }

        [TestMethod]
        public async Task TestWebSocket()
        {
            await _emulatorLib.SetMode(ModeType.ReverseLocation, true, LocationLib.GetDefaultReverseResponse().ToJsonString());

            JsonObject user = await _userLib.CreateDefaultUser();
            string accessToken = user["accessToken"].Value<string>();

            JsonObject session = await _sessionLib.CreateDefaultSession(accessToken);
            Guid sessionId = session["id"].Value<Guid>();

            ClientWebSocket clientWebSocket = new();
            Uri uri = new(webSocketUri);
            clientWebSocket.Options.SetRequestHeader("Sec-WebSocket-Protocol", accessToken);
            await clientWebSocket.ConnectAsync(uri, CancellationToken.None);

            JsonObject requestMessage = [];

            requestMessage.Add("MessageType", "MessageSendRequest");
            requestMessage.Add("SessionId", sessionId);
            requestMessage.Add("Content", "Test Message");

            await clientWebSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(requestMessage.ToJsonString())), WebSocketMessageType.Text, true, CancellationToken.None);

            byte[] buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            byte[] slicedBuffer = buffer[..result.Count];

            string rawResponse = Encoding.UTF8.GetString(slicedBuffer);
            JsonObject responseMessage = JsonSerializer.Deserialize<JsonObject>(rawResponse);

            Assert.AreEqual("MessageSendResponse", responseMessage["MessageType"].Value<string>());
            JsonObject message = responseMessage["Message"].AsObject();
            Assert.AreEqual("Test Message", message["Content"].Value<string>());
            Assert.AreEqual(user["userId"].Value<string>(), message["CreatorId"].Value<string>());
            Assert.AreEqual(sessionId.ToString(), message["SessionId"].Value<string>());
            Assert.IsNotNull(message["CreatedAt"].Value<DateTime>());
            await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Close Test Connection", CancellationToken.None);
        }
    }
}
