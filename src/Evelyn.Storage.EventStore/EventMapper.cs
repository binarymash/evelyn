namespace Evelyn.Storage.EventStore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CQRSlite.Events;
    using global::EventStore.ClientAPI;
    using Newtonsoft.Json;

    public class EventMapper
    {
        private Dictionary<string, Type> _registeredEventTypes;

        public EventMapper()
        {
            RegisterEventTypes();
        }

        public EventData MapEvent(IEvent @event)
        {
            return new EventData(
                Guid.NewGuid(),
                @event.GetType().FullName,
                false,
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)),
                null);
        }

        public IEvent MapEvent(ResolvedEvent @event)
        {
            var type = _registeredEventTypes[@event.Event.EventType];
            var json = Encoding.UTF8.GetString(@event.Event.Data);
            return JsonConvert.DeserializeObject(json, type) as IEvent;
        }

        private void RegisterEventTypes()
        {
            _registeredEventTypes = new Dictionary<string, Type>();

            var eventTypes = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("Evelyn.Core"))
                .SelectMany(a => a.GetLoadableTypes().Where(IsConcreteEvent));

            foreach (var eventType in eventTypes)
            {
                _registeredEventTypes.Add(eventType.FullName, eventType);
            }
        }

        private bool IsConcreteEvent(Type type)
        {
            return typeof(IEvent).IsAssignableFrom(type) && !type.IsAbstract && type.IsClass;
        }
    }
}
