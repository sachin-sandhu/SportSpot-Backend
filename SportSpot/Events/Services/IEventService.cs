namespace SportSpot.Events
{
    public interface IEventService
    {
        Task FireEvent(IEvent @event);
        void RegisterListener(IListener listener);
        void UnRegisterListener(IListener listener);
        List<IListener> GetRegisteredListeners();
    }
}
