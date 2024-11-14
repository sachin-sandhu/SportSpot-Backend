using SportSpot.V1.User.Events;

namespace SportSpot.Listener
{
    public class TestListener(ILogger<TestListener> _logger) : IListener
    {
        [EventHandler]
        public void OnUserCreated(AuthUserCreatedEvent e)
        {
            _logger.LogInformation("User created: {UserName}", e.AuthUserEntity.UserName);
        }
    }
}
