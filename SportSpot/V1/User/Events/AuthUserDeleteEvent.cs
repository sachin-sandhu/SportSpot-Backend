namespace SportSpot.V1.User
{
    public record AuthUserDeleteEvent : IEvent, ICancellable
    {

        public required AuthUserEntity AuthUser { get; init; }
        private bool _cancelled;

        public bool IsCancelled() => _cancelled;

        public void SetCancelled(bool cancel) => _cancelled = cancel;
    }
}
