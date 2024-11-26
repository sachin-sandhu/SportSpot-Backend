using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace SportSpot.V1.WebSockets.Services
{
    public interface IConnectionService
    {
        ConcurrentDictionary<Guid, WebSocket> GetAllConnections();
        WebSocket? GetWebSocket(Guid userId);
        List<WebSocket> GetWebSockets();
        List<Guid> GetAllUser();
        bool AddWebSocket(WebSocket webSocket, Guid userId);
        Task RemoveWebSocket(WebSocket webSocket, string? reason);
        Guid? GetUser(WebSocket webSocket);
    }
}
