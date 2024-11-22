using Integration_Test.Extensions;
using Integration_Test.V1.Libs;
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
        private readonly RestEmulatorLib _emulatorLib;

        public DefaultWebSocketTest()
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
        public async Task TestWebSocket()
        {
            JsonObject user = await _userLib.CreateDefaultUser();
            string accessToken = user["accessToken"].Value<string>();

            ClientWebSocket clientWebSocket = new();
            Uri uri = new(webSocketUri);
            clientWebSocket.Options.SetRequestHeader("Sec-WebSocket-Protocol", accessToken);
            await clientWebSocket.ConnectAsync(uri, CancellationToken.None);

            JsonObject requestMessage = [];
            requestMessage.Add("Type", "test");

            await clientWebSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(requestMessage.ToJsonString())), WebSocketMessageType.Text, true, CancellationToken.None);

            while (clientWebSocket.State != WebSocketState.Aborted && clientWebSocket.State != WebSocketState.Closed) ;
        }
    }
}
