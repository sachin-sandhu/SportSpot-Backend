using SportSpot.V1.Session.Services;
using SportSpot.V1.User.Events;

namespace SportSpot.V1.Session.Listener
{
    public class UserDeleteListener(ISessionService _sessionService) : IListener
    {
        [EventHandler]
        public async Task OnUserDelete(AuthUserDeletedEvent e)
        {
            try
            {
                await _sessionService.DeleteAll(e.AuthUser);
            }
            catch (Exception) { }
            try
            {
                await _sessionService.LeaveAll(e.AuthUser);
            }
            catch (Exception) { }
        }
    }
}
