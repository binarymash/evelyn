namespace Evelyn.Core.ReadModel.EventStream
{
    using CQRSlite.Events;

    public class EventEnvelope
    {
        public EventEnvelope(long streamPosition, IEvent @event)
        {
            StreamPosition = streamPosition;
            Event = @event;
        }

        public long StreamPosition { get; }

        public IEvent Event { get; }
    }
}
