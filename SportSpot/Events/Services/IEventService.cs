namespace SportSpot.Events.Services
{
    public interface IEventService
    {
        Task<bool> FireEvent(IEvent @event);
        void RegisterListener(IListener listener);
        void UnRegisterListener(IListener listener);
        List<IListener> GetRegisteredListeners();
    }
}
