namespace Evelyn.Core.Tests.ReadModel.EventStream.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.EventStream;
    using Evelyn.Core.ReadModel.EventStream.Handlers;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using TestStack.BDDfy;
    using Xunit;

    public class EventStreamHandlerSpecs
    {
        private readonly Fixture _dataFixture;
        private readonly ServiceProvider _serviceProvider;
        private CancellationTokenSource _stoppingTokenSource;

        private EventStream _eventStream;
        private EventStreamHandler _eventStreamHandler;
        private StubbedEventHandler _eventHandler;

        private List<EventEnvelope> _eventsAddedToStream;

        public EventStreamHandlerSpecs()
        {
            _dataFixture = new Fixture();
            _eventHandler = new StubbedEventHandler();
            _stoppingTokenSource = new CancellationTokenSource();

            IServiceCollection services = new ServiceCollection();
            services.AddHostedService<EventStreamHandler>();
            services.AddSingleton<IEventStreamFactory, EventStreamFactory>();
            services.AddSingleton<IEventHandler<EventStreamHandler>>(_eventHandler);
            services.AddLogging();

            _serviceProvider = services.BuildServiceProvider();
            _eventStreamHandler = _serviceProvider.GetService<IHostedService>() as EventStreamHandler;
            _eventStream = _serviceProvider.GetService<IEventStreamFactory>().GetEventStream<EventStreamHandler>();

            _eventsAddedToStream = new List<EventEnvelope>();
        }

        [Fact]
        public void EventAddedToStreamBeforeHandlerIsStarted()
        {
            try
            {
                this.Given(_ => _.GivenMultipleEventsAreAddedToTheEventStream())
                    .When(_ => _.WhenTheEventStreamHandlerIsStarted())
                    .Then(_ => _.ThenTheEventIsHandledByTheEventHandler())
                    .And(_ => _.ThenThereAreNoQueuedEventsInTheEventStream())
                    .BDDfy();
            }
            finally
            {
                _stoppingTokenSource.Cancel();
            }
        }

        [Fact]
        public void EventAddedToStreamAfterHandlerIsStarted()
        {
            try
            {
                this.Given(_ => _.GivenTheEventStreamHandlerIsStarted())
                    .When(_ => _.WhenAnEventIsAddedToTheEventStream())
                    .Then(_ => _.ThenTheEventIsHandledByTheEventHandler())
                    .And(_ => _.ThenThereAreNoQueuedEventsInTheEventStream())
                    .BDDfy();
            }
            finally
            {
                _stoppingTokenSource.Cancel();
            }
        }

        [Fact]
        public void ExceptionThrownByEventHandlerOnFirstAttempt()
        {
            try
            {
                this.Given(_ => _.GivenTheEventStreamHandlerIsStarted())
                    .And(_ => _.GivenTheEventHandlerWillThrowAnExceptionOnTheFirstAttempt())
                    .When(_ => _.WhenMultipleEventsAreAddedToTheEventStream())
                    .Then(_ => _.ThenAllTheEventsAreHandledByTheEventHandlerInTheCorrectOrder())
                    .And(_ => _.ThenThereAreNoQueuedEventsInTheEventStream())
                    .BDDfy();
            }
            finally
            {
                _stoppingTokenSource.Cancel();
            }
        }

        [Fact]
        public void StoppingWhenNoEvents()
        {
            try
            {
                this.Given(_ => _.GivenTheEventStreamHandlerIsStarted())
                    .When(_ => _.WhenTheEventStreamHandlerIsStopped())
                    .Then(_ => _.ThenTheEventStreamHandlerStops())
                    .BDDfy();
            }
            finally
            {
                _stoppingTokenSource.Cancel();
            }
        }

        private void GivenTheEventHandlerWillThrowAnExceptionOnTheFirstAttempt()
        {
            _eventHandler.ThrowOnHandleEventAttempt(1);
        }

        private async Task GivenTheEventStreamHandlerIsStarted()
        {
            await StartEventStreamHandler();
        }

        private void GivenMultipleEventsAreAddedToTheEventStream()
        {
            AddMultipleEventsToTheEventStream();
        }

        private async Task WhenTheEventStreamHandlerIsStarted()
        {
            await StartEventStreamHandler();
        }

        private void WhenAnEventIsAddedToTheEventStream()
        {
            var @event = new EventEnvelope(
                _dataFixture.Create<long>(),
                new SomeEvent());

            _eventStream.Enqueue(@event);

            _eventsAddedToStream.Add(@event);
        }

        private void WhenMultipleEventsAreAddedToTheEventStream()
        {
            AddMultipleEventsToTheEventStream();
        }

        private async Task WhenTheEventStreamHandlerIsStopped()
        {
            await _eventStreamHandler.StopAsync(default);
        }

        private void ThenTheEventIsHandledByTheEventHandler()
        {
            AssertHandledEvents(new[] { _eventsAddedToStream.First() });
        }

        private void ThenAllTheEventsAreHandledByTheEventHandlerInTheCorrectOrder()
        {
            AssertHandledEvents(_eventsAddedToStream);
        }

        private void ThenTheEventStreamHandlerStops()
        {
            var i = 0;
            var keepWaiting = true;

            while (keepWaiting)
            {
                i++;
                keepWaiting = i < 10 &&
                    _eventStreamHandler.Status != EventStreamHandlerStatus.Stopped;

                Task.Delay(50);
            }

            _eventStreamHandler.Status.Should().Be(EventStreamHandlerStatus.Stopped);
        }

        private async Task StartEventStreamHandler()
        {
            await _eventStreamHandler.StartAsync(_stoppingTokenSource.Token);
        }

        private void AddMultipleEventsToTheEventStream()
        {
            _eventsAddedToStream = new List<EventEnvelope>
            {
                new EventEnvelope(
                _dataFixture.Create<long>(),
                new SomeEvent()),

                new EventEnvelope(
                _dataFixture.Create<long>(),
                new SomeEvent())
            };

            _eventStream.EnqueueRange(_eventsAddedToStream);
        }

        private void AssertHandledEvents(IEnumerable<EventEnvelope> expectedOrderedEvents)
        {
            // Polly
            var iterations = 0;

            while (iterations < 10)
            {
                try
                {
                    _eventHandler.HandledEvents.Should().ContainInOrder(expectedOrderedEvents);
                    break;
                }
                catch
                {
                    iterations++;
                    Task.Delay(50);
                }
            }
        }

        private void ThenThereAreNoQueuedEventsInTheEventStream()
        {
            _eventStream.QueueSize.Should().Be(0);
        }

        private class SomeEvent : CQRSlite.Events.IEvent
        {
            public Guid Id { get; set; }

            public int Version { get; set; }

            public DateTimeOffset TimeStamp { get; set; }
        }

        private class StubbedEventHandler : IEventHandler<EventStreamHandler>
        {
            private int _handleEventAttempt = 0;
            private List<int> _throwOnHandleEventAttempts = new List<int>();
            private List<EventEnvelope> _handledEvents = new List<EventEnvelope>();

            public IList<EventEnvelope> HandledEvents => _handledEvents;

            public void ThrowOnHandleEventAttempt(int handleEventAttemptNumber)
            {
                _throwOnHandleEventAttempts.Add(handleEventAttemptNumber);
            }

            public async Task HandleEvent(EventEnvelope eventEnvelope, CancellationToken stoppingToken)
            {
                if (_throwOnHandleEventAttempts.Contains(_handleEventAttempt))
                {
                    throw new Exception("bang");
                }

                _handledEvents.Add(eventEnvelope);

                _handleEventAttempt++;

                await Task.CompletedTask;
            }
        }
    }
}
