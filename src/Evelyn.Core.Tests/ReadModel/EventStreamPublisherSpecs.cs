namespace Evelyn.Core.Tests.ReadModel
{
    using AutoFixture;
    using Core.ReadModel;
    using FluentAssertions;

    public abstract class EventStreamPublisherSpecs<TPublisher>
    {
        protected EventStreamPublisherSpecs()
        {
            Fixture = new Fixture();
            EventStreamFactory = new StubbedEventStreamFactory();
        }

        protected Fixture Fixture { get; }

        protected StubbedEventStreamFactory EventStreamFactory { get; }

        protected TPublisher Publisher { get; set; }

        protected EventEnvelope Message { get; set; }

        protected void ThenTheEventIsAddedToTheStreamFor<TDto>()
        {
            var eventStream = EventStreamFactory.GetEventStream<TDto>();
            eventStream.Count.Should().Be(1);
            eventStream.Should().Contain(Message);
        }
    }
}
