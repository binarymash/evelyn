namespace Evelyn.Core
{
    using System.Collections.Generic;
    using CQRSlite.Events;

    public interface IInMemoryEventStore : IEventStore
    {
        IEnumerable<InMemoryEventEnvelope> Get(long fromPosition);
    }
}
