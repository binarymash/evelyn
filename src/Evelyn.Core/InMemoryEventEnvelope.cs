namespace Evelyn.Core
{
    using CQRSlite.Events;

    public class InMemoryEventEnvelope
    {
        public InMemoryEventEnvelope(long position, IEvent @event)
        {
            Position = position;
            Event = @event;
        }

        public long Position { get; }

        public IEvent Event { get; set; }
    }
}
