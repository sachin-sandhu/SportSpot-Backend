using System.Reflection;

namespace SportSpot.Events
{
    public class EventService : IEventService
    {

        private static readonly List<EventPriority> _sortedPriorites = [.. Enum.GetValues(typeof(EventPriority)).Cast<EventPriority>().OrderBy(p => p)];

        private readonly List<RegisteredListener> _listener = [];

        public Task FireEvent(IEvent @event)
        {
            Type type = @event.GetType();
            Dictionary<EventPriority, List<RegisteredEventHandler>> registeredEventHandler = [];
            _listener.ForEach(x =>
            {
                if (!x.Events.TryGetValue(type, out List<RegisteredEventHandler>? value)) return;
                foreach (RegisteredEventHandler handler in value)
                {
                    if (!registeredEventHandler.ContainsKey(handler.Attribute.Priority))
                        registeredEventHandler.Add(handler.Attribute.Priority, []);
                    registeredEventHandler[handler.Attribute.Priority].Add(handler);
                }
            });

            foreach (EventPriority priority in _sortedPriorites)
            {
                if (!registeredEventHandler.TryGetValue(priority, out List<RegisteredEventHandler>? value)) continue;
                foreach (RegisteredEventHandler handler in value)
                {
                    handler.Method.Invoke(handler.Listener, [@event]);
                }
            }
            return Task.CompletedTask;
        }

        public void RegisterListener(IListener listener)
        {
            if (_listener.Select(x => x.Listener == listener).Any()) return;
            RegisteredListener registeredListener = new() { Listener = listener };
            listener.GetType().GetMethods().ToList().ForEach(method =>
            {
                EventHandlerAttribute? attribute = method.GetCustomAttribute<EventHandlerAttribute>();
                if (attribute == null) return;
                if (method.GetParameters().Length != 1) throw new InvalidOperationException("Event method must have one parameter of type IEvent");

                List<ParameterInfo> eventArgs = method.GetParameters().Where(x => x.ParameterType.GetInterfaces().Contains(typeof(IEvent))).ToList();
                if (eventArgs.Count == 0) throw new InvalidOperationException("Event method has no IEvent Parameter");
                if (eventArgs.Count > 1) throw new InvalidOperationException("Event method can only have one parameter of type IEvent");
                Type eventType = eventArgs[0].ParameterType;

                if(!registeredListener.Events.ContainsKey(eventType))
                    registeredListener.Events.Add(eventType, []);

                registeredListener.Events[eventType].Add(new RegisteredEventHandler { Attribute = attribute, Method = method, Listener = listener });
            });
            _listener.Add(registeredListener);
        }

        public void UnRegisterListener(IListener listener) => _listener.RemoveAll(x => x.Listener == listener);

        public List<IListener> GetRegisteredListeners() => _listener.Select(x => x.Listener).ToList();
    }
}
