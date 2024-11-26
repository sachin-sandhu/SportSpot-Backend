using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace SportSpot.V1.WebSockets.Services
{
    public class ConnectionService : IConnectionService
    {

        private readonly ConcurrentDictionary<Guid, WebSocket> _sockets = [];

        public bool AddWebSocket(WebSocket webSocket, Guid userId)
        {
            return _sockets.TryAdd(userId, webSocket);
        }

        public ConcurrentDictionary<Guid, WebSocket> GetAllConnections()
        {
            return _sockets;
        }

        public List<Guid> GetAllUser()
        {
            return [.. _sockets.Keys];
        }

        public Guid? GetUser(WebSocket webSocket)
        {
            Guid userId = _sockets.FirstOrDefault(x => x.Value == webSocket).Key;
            if (userId == Guid.Empty)
                return null;
            return userId;
        }

        public WebSocket? GetWebSocket(Guid userId)
        {
            return _sockets.TryGetValue(userId, out WebSocket? webSocket) ? webSocket : null;
        }

        public List<WebSocket> GetWebSockets()
        {
            return [.. _sockets.Values];
        }

        public async Task RemoveWebSocket(WebSocket webSocket, string? reason)
        {
            KeyValuePair<Guid, WebSocket> registeredWebsocket = _sockets.FirstOrDefault(x => x.Value == webSocket);

            if (registeredWebsocket.Key != Guid.Empty)
                _sockets.TryRemove(registeredWebsocket.Key, out _);

            if (webSocket.State != WebSocketState.Aborted)
            {
                await webSocket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure, statusDescription: reason, cancellationToken: CancellationToken.None);
            }
        }
    }
}
