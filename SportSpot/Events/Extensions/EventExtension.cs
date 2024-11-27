using SportSpot.Events.Services;
using SportSpot.Listener;
using SportSpot.V1.Session.Chat.Listener;
using SportSpot.V1.Session.Listener;

namespace SportSpot.Events.Extensions
{
    public static class EventExtension
    {
        private readonly static List<Type> _listener = [typeof(TestListener)];
        private readonly static List<Type> _scopedListener = [typeof(MessageReceiveListener), typeof(SessionDeleteListener), typeof(UserDeleteListener)];

        public static void RegisterEvents(this IServiceCollection col)
        {
            _listener.ForEach(x =>
            {
                if (!x.GetInterfaces().Contains(typeof(IListener))) throw new InvalidOperationException("Listener must implement IListener");
                col.AddSingleton(x);
            });

            _scopedListener.ForEach(x =>
            {
                if (!x.GetInterfaces().Contains(typeof(IListener))) throw new InvalidOperationException("Listener must implement IListener");
                col.AddScoped(x);
            });
        }

        public static void RegisterEvents(this IServiceProvider col)
        {
            IEventService eventService = col.GetRequiredService<IEventService>();
            _listener.ForEach(x => eventService.RegisterListener(col.GetRequiredService<TestListener>() ?? throw new InvalidOperationException("Listener not found")));
            _scopedListener.ForEach(x => eventService.RegisterScopedListener(x));
        }
    }
}
