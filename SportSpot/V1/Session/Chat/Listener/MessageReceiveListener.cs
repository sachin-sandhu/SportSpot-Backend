using SportSpot.V1.Session.Chat.Dtos;
using SportSpot.V1.Session.Chat.Services;
using SportSpot.V1.WebSockets.Enums;
using SportSpot.V1.WebSockets.Events;

namespace SportSpot.V1.Session.Chat.Listener
{
    public class MessageReceiveListener(IMessageService _messageService) : IListener
    {

        [EventHandler]
        public void OnReceiveMessage(WebSocketMessageReceivedEvent receivedEvent)
        {
            if (receivedEvent.WebSocketMessage.MessageType != WebSocketMessageType.MessageSendRequest)
                return;
            _messageService.HandleMessage((MessageSendRequestDto)receivedEvent.WebSocketMessage, receivedEvent.Sender);
        }

    }
}
