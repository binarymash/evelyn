namespace Evelyn.Core.ReadModel.EventStream
{
    public interface IEventStreamFactory
    {
        EventStream GetEventStream<TEventStreamHandler>();
    }
}