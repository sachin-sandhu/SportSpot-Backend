namespace SportSpot.Events.Services
{
    public interface IEventService
    {
        Task<bool> FireEvent(IEvent @event);
        void RegisterListener(IListener listener);
        void RegisterScopedListener(Type listenerType);
        void UnRegisterListener(IListener listener);
        void UnRegisterScopedListener(Type listenerType);
        List<IListener> GetRegisteredListeners();
        List<Type> GetScopedRegisteredListeners();
    }
}
