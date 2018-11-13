namespace Evelyn.Core.ReadModel
{
    public interface IEventStreamFactory
    {
        EventStream GetEventStream<TEventStreamHandler>();
    }
}