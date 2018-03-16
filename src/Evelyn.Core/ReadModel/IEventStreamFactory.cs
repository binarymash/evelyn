namespace Evelyn.Core.ReadModel
{
    using System.Collections.Generic;
    using CQRSlite.Events;

    public interface IEventStreamFactory
    {
        Queue<IEvent> GetEventStream<T>();
    }
}