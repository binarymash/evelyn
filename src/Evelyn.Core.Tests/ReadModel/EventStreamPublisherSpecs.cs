namespace Evelyn.Core.Tests.ReadModel
{
    using System.Collections.Generic;
    using AutoFixture;
    using Core.ReadModel;
    using CQRSlite.Events;
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

        protected IEvent Message { get; set; }

        protected void ThenTheEventIsAddedToTheStreamFor<TDto>()
        {
            var eventStream = EventStreamFactory.GetEventStream<TDto>();
            eventStream.Count.Should().Be(1);
            eventStream.Should().Contain(Message);
        }

        protected class StubbedEventStreamFactory : IEventStreamFactory
        {
            private readonly Queue<IEvent> _events;

            public StubbedEventStreamFactory()
            {
                _events = new Queue<IEvent>();
            }

            public Queue<IEvent> GetEventStream<TDto>()
            {
                return _events;
            }
        }
    }
}
