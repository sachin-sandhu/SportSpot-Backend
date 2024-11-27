using SportSpot.V1.Session.Chat.Services;
using SportSpot.V1.Session.Events;

namespace SportSpot.V1.Session.Chat.Listener
{
    public class SessionDeleteListener(IMessageService _messageService) : IListener
    {
        [EventHandler]
        public async Task OnSessionDelete(SessionDeletedEvent e)
        {
            await _messageService.DeleteAll(e.SessionEntity);
        }
    }
}
