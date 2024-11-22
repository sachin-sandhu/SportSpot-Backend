using Microsoft.Extensions.Primitives;
using SportSpot.V1.Exceptions;
using SportSpot.V1.WebSockets.Services;
using System.Net.WebSockets;
using System.Text;

namespace SportSpot.V1.WebSockets.Middleware
{
    public class CustomWebSocketMiddleware(RequestDelegate _next, IServiceProvider _serviceProvider)
    {
        public async Task Invoke(HttpContext context)
        {
            try
            {
                if (!context.WebSockets.IsWebSocketRequest)
                {
                    await _next(context);
                    return;
                }

                if (!context.Request.Headers.TryGetValue("Sec-WebSocket-Protocol", out StringValues token))
                {
                    await new UnauthorizedException().WriteToResponse(context.Response);
                    return;
                }

                if (token.Count == 0)
                {
                    await new UnauthorizedException().WriteToResponse(context.Response);
                    return;
                }

                string? authorization = token[0];
                if (string.IsNullOrWhiteSpace(authorization))
                {
                    await new UnauthorizedException().WriteToResponse(context.Response);
                    return;
                }

                using var scope = _serviceProvider.CreateScope();
                IWebSocketService webSocketService = scope.ServiceProvider.GetRequiredService<IWebSocketService>();
                ILogger<CustomWebSocketMiddleware> logger = scope.ServiceProvider.GetRequiredService<ILogger<CustomWebSocketMiddleware>>();

                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                bool success = await webSocketService.OnConnect(webSocket, authorization);
                if (!success)
                    return;

                await Receive(webSocket, webSocketService, logger, async (result, buffer) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string payload = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        await webSocketService.OnReceive(webSocket, payload);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await HandleDisconnect(webSocket, webSocketService);
                    }
                });
            }
            catch (AbstractSportSpotException ex)
            {
                await ex.WriteToResponse(context.Response);
            }
            catch (Exception)
            {
                await new InternalServerErrorException().WriteToResponse(context.Response);
            }
        }

        private static async Task HandleDisconnect(WebSocket socket, IWebSocketService webSocketService)
        {
            await webSocketService.OnDisconnect(socket);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed because of an error!", CancellationToken.None);
        }

        private static async Task Receive(WebSocket socket, IWebSocketService webSocketService, ILogger<CustomWebSocketMiddleware> logger, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];
            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                                                           cancellationToken: CancellationToken.None);
                    handleMessage(result, buffer);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while receiving a message: {ErrorMessage}", ex.Message);
                await HandleDisconnect(socket, webSocketService);
            }
        }
    }
}