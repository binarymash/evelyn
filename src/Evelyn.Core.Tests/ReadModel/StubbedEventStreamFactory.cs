namespace Evelyn.Core.Tests.ReadModel
{
    using Evelyn.Core.ReadModel.EventStream;

    public class StubbedEventStreamFactory : IEventStreamFactory
    {
        private readonly EventStream _eventStream;

        public StubbedEventStreamFactory()
        {
            _eventStream = new EventStream();
        }

        public EventStream GetEventStream<TDto>()
        {
            return _eventStream;
        }
    }
}
