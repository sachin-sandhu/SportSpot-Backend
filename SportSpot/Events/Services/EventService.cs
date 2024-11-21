using MongoDB.Driver;
using System.Reflection;

namespace SportSpot.Events.Services
{
    public class EventService : IEventService
    {
        private static readonly List<EventPriority> _sortedPriorities = [.. Enum.GetValues<EventPriority>().Cast<EventPriority>().OrderBy(p => p)];

        private readonly List<RegisteredListener> _listener = [];

        public async Task<bool> FireEvent(IEvent @event)
        {
            Type type = @event.GetType();
            Dictionary<EventPriority, List<RegisteredEventHandler>> registeredEventHandler = [];

            foreach (RegisteredListener listener in _listener)
            {
                if (!listener.Events.TryGetValue(type, out List<RegisteredEventHandler>? handlers)) continue;
                foreach (RegisteredEventHandler handler in handlers)
                {
                    if (!registeredEventHandler.ContainsKey(handler.Attribute.Priority))
                        registeredEventHandler.Add(handler.Attribute.Priority, []);
                    registeredEventHandler[handler.Attribute.Priority].Add(handler);
                }
            }

            foreach (EventPriority priority in _sortedPriorities)
            {
                if (!registeredEventHandler.TryGetValue(priority, out List<RegisteredEventHandler>? handlers)) continue;
                foreach (RegisteredEventHandler handler in handlers)
                {
                    if (handler.Method.Invoke(handler.Listener, [@event]) is Task task)
                        await task;
                }
            }
            bool isCancellable = @event.GetType().GetInterfaces().ToList().Exists(x => x == typeof(ICancellable));
            if (!isCancellable) return false;
            return ((ICancellable)@event).IsCancelled();
        }

        public void RegisterListener(IListener listener)
        {
            if (_listener.Exists(x => x.Listener == listener)) return;
            RegisteredListener registeredListener = new() { Listener = listener };
            listener.GetType().GetMethods().ToList().ForEach(method =>
            {
                EventHandlerAttribute? attribute = method.GetCustomAttribute<EventHandlerAttribute>();
                if (attribute == null) return;
                if (method.GetParameters().Length != 1) throw new InvalidOperationException("Event method must have one parameter of type IEvent");

                List<ParameterInfo> eventArgs = method.GetParameters().Where(x => x.ParameterType.GetInterfaces().Contains(typeof(IEvent))).ToList();
                if (eventArgs.Count == 0) throw new InvalidOperationException("Event method has no IEvent Parameter");
                Type eventType = eventArgs[0].ParameterType;

                if (!registeredListener.Events.ContainsKey(eventType))
                    registeredListener.Events.Add(eventType, []);

                registeredListener.Events[eventType].Add(new RegisteredEventHandler { Attribute = attribute, Method = method, Listener = listener });
            });
            _listener.Add(registeredListener);
        }

        public void UnRegisterListener(IListener listener) => _listener.RemoveAll(x => x.Listener == listener);

        public List<IListener> GetRegisteredListeners() => _listener.Select(x => x.Listener).ToList();
    }
}