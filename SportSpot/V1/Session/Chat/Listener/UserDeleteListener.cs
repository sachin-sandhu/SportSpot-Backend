using SportSpot.V1.Session.Chat.Services;
using SportSpot.V1.Session.Services;
using SportSpot.V1.User.Events;

namespace SportSpot.V1.Session.Chat.Listener
{
    public class UserDeleteListener(IMessageService _messageService) : IListener
    {
        [EventHandler]
        public async Task OnUserDelete(AuthUserDeletedEvent e)
        {
            try
            {
                await _messageService.DeleteAll(e.AuthUser);
            }
            catch (Exception)
            {
                //Ignore
            }
        }
    }
}
