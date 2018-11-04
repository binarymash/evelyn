namespace Evelyn.Core.ReadModel
{
    using System.Collections.Generic;

    public interface IEventStreamFactory
    {
        Queue<EventEnvelope> GetEventStream<TEventStreamHandler>();
    }
}