using MongoDB.Driver;
using System.Reflection;

namespace SportSpot.Events.Services
{
    public class EventService(IServiceProvider _serviceProvider) : IEventService
    {
        private static readonly List<EventPriority> _sortedPriorities = [.. Enum.GetValues<EventPriority>().OrderBy(p => p)];

        private readonly List<RegisteredListener> _listener = [];
        private readonly List<Type> _scopedListener = [];


        public async Task<bool> FireEvent(IEvent @event)
        {
            Type type = @event.GetType();
            Dictionary<EventPriority, List<RegisteredEventHandler>> registeredEventHandler = [];

            // Create Scoped Listener
            using var scope = _serviceProvider.CreateScope();
            List<RegisteredListener> scopedListeners = CreateScopedListener(scope);

            // Concatenate all listeners (Global and Scoped)
            List<RegisteredListener> listenerToCall = [.. _listener, .. scopedListeners];

            foreach (RegisteredListener listener in listenerToCall)
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

        private List<RegisteredListener> CreateScopedListener(IServiceScope scope)
        {
            List<RegisteredListener> scopedListeners = [];
            foreach (Type listenerType in _scopedListener)
            {
                IListener listener = (IListener)scope.ServiceProvider.GetRequiredService(listenerType) ?? throw new InvalidOperationException("Listener not found");
                RegisteredListener registeredListener = CreateRegisteredListenerFromListener(listener);
                scopedListeners.Add(registeredListener);
            }
            return scopedListeners;
        }

        public void RegisterListener(IListener listener)
        {
            if (_listener.Exists(x => x.Listener == listener)) return;
            RegisteredListener registeredListener = CreateRegisteredListenerFromListener(listener);
            _listener.Add(registeredListener);
        }

        private RegisteredListener CreateRegisteredListenerFromListener(IListener listener)
        {
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
            return registeredListener;
        }

        public void UnRegisterListener(IListener listener) => _listener.RemoveAll(x => x.Listener == listener);

        public void RegisterScopedListener(Type listenerType)
        {
            if (_scopedListener.Contains(listenerType)) return;
            _scopedListener.Add(listenerType);
        }

        public void UnRegisterScopedListener(Type listenerType)
        {
            _scopedListener.Remove(listenerType);
        }

        public List<Type> GetScopedRegisteredListeners()
        {
            return _scopedListener;
        }

        public List<IListener> GetRegisteredListeners() => _listener.Select(x => x.Listener).ToList();

    }
}