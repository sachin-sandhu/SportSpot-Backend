﻿using SportSpot.Events.Services;
using SportSpot.V1.Exceptions;
using SportSpot.V1.Exceptions.WebSocket;
using SportSpot.V1.User.Entities;
using SportSpot.V1.User.Services;
using SportSpot.V1.WebSockets.Converter;
using SportSpot.V1.WebSockets.Dtos;
using SportSpot.V1.WebSockets.Events;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Serialization;

namespace SportSpot.V1.WebSockets.Services
{
    public class WebSocketService(IConnectionService _connectionService, IUserService _userService, IAuthService _authService, IEventService _eventService) : IWebSocketService
    {

        private readonly static JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new WebSocketMessageConverter(), new JsonStringEnumConverter() }
        };

        public async Task<bool> OnConnect(WebSocket webSocket, string authorization)
        {
            AuthUserEntity user = await _authService.AuthorizeUser(authorization);
            if (!_connectionService.AddWebSocket(webSocket, user.Id))
            {
                // If the user is already connected, we close the old
                WebSocket? toClose = _connectionService.GetWebSocket(user.Id);
                if (toClose != null)
                    await _connectionService.RemoveWebSocket(toClose, "Reconnected");

                return _connectionService.AddWebSocket(webSocket, user.Id);
            }
            return true;
        }

        public async Task OnDisconnect(WebSocket webSocket)
        {
            await _connectionService.RemoveWebSocket(webSocket, "Disconnected");
        }

        public async Task OnReceive(WebSocket webSocket, string payload)
        {
            IWebSocketMessageDto message = JsonSerializer.Deserialize<IWebSocketMessageDto>(payload, _options) ?? throw new InvalidWebSocketMessageException();
            if (message.IsToSend())
                throw new UnsupportActionWebSocketMessageException();
            Guid userId = _connectionService.GetUser(webSocket) ?? throw new UnauthorizedException();
            AuthUserEntity user = await _userService.GetUser(userId);

            WebSocketMessageReceivedEvent webSocketMessageReceivedEvent = new()
            {
                Sender = user,
                WebSocketMessage = message
            };
            await _eventService.FireEvent(webSocketMessageReceivedEvent);
        }

        public async Task<bool> SendMessage(AuthUserEntity user, IWebSocketMessageDto message)
        {
            return await SendMessage(user.Id, message);
        }

        public async Task<bool> SendMessage(Guid userId, IWebSocketMessageDto message)
        {
            if (!message.IsToSend())
                throw new UnsupportActionWebSocketMessageException();
            WebSocket? webSocket = _connectionService.GetWebSocket(userId);
            if (webSocket == null)
                return false;
            string payload = JsonSerializer.Serialize(message, _options);
            byte[] buffer = Encoding.UTF8.GetBytes(payload);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            return true;
        }
    }
}
