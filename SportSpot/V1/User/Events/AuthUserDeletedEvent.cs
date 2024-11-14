using SportSpot.V1.User.Entities;

namespace SportSpot.V1.User.Events
{
    public record AuthUserDeletedEvent : IEvent
    {
        public required AuthUserEntity AuthUser { get; init; }
    }
}
