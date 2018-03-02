namespace Evelyn.Storage.EventStore
{
    using System;
    using System.Text;
    using CQRSlite.Events;
    using global::EventStore.ClientAPI;
    using Newtonsoft.Json;

    public class EventMapper
    {
        public EventData MapEvent(IEvent @event)
        {
            return new EventData(
                Guid.NewGuid(),
                @event.GetType().AssemblyQualifiedName,
                false,
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)),
                null);
        }

        public IEvent MapEvent(ResolvedEvent @event)
        {
            var type = Type.GetType(@event.Event.EventType);
            var json = Encoding.UTF8.GetString(@event.Event.Data);
            return JsonConvert.DeserializeObject(json, type) as IEvent;
        }
    }
}
