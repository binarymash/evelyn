namespace Evelyn.Core.Tests.ReadModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class EventStreamHandlerSpecs
    {
        private readonly Fixture _dataFixture;
        private IEventStreamFactory _eventStreamFactory;
        private StubbedEventHandler _eventHandler;
        private EventStream _eventStream;
        private EventEnvelope _event;
        private CancellationTokenSource _stoppingTokenSource;
        private ServiceProvider _serviceProvider;
        private EventStreamHandler _eventStreamHandler;
        private List<EventEnvelope> _events;

        public EventStreamHandlerSpecs()
        {
            _dataFixture = new Fixture();

            _eventStream = new EventStream();
            _eventHandler = new StubbedEventHandler();

            _eventStreamFactory = Substitute.For<IEventStreamFactory>();
            _eventStreamFactory
                .GetEventStream<EventStreamHandler>()
                .Returns(_eventStream);

            _stoppingTokenSource = new CancellationTokenSource();

            IServiceCollection services = new ServiceCollection();
            services.AddHostedService<EventStreamHandler>();
            services.AddSingleton(_eventStreamFactory);
            services.AddSingleton<IEventHandler<EventStreamHandler>>(_eventHandler);
            services.AddLogging();

            _serviceProvider = services.BuildServiceProvider();
            _eventStreamHandler = _serviceProvider.GetService<IHostedService>() as EventStreamHandler;
        }

        [Fact]
        public void EventAddedToStream()
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
            await _eventStreamHandler.StartAsync(_stoppingTokenSource.Token);
        }

        private void WhenAnEventIsAddedToTheEventStream()
        {
            _event = new EventEnvelope(
                _dataFixture.Create<long>(),
                new SomeEvent());

            _eventStream.Enqueue(_event);
        }

        private void WhenMultipleEventsAreAddedToTheEventStream()
        {
            _events = new List<EventEnvelope>
            {
                new EventEnvelope(
                _dataFixture.Create<long>(),
                new SomeEvent()),

                new EventEnvelope(
                _dataFixture.Create<long>(),
                new SomeEvent())
            };

            _eventStream.EnqueueRange(_events);
        }

        private async Task WhenTheEventStreamHandlerIsStopped()
        {
            await _eventStreamHandler.StopAsync(default);
        }

        private void ThenTheEventIsHandledByTheEventHandler()
        {
            AssertHandledEvents(new List<EventEnvelope> { _event });
        }

        private void ThenAllTheEventsAreHandledByTheEventHandlerInTheCorrectOrder()
        {
            AssertHandledEvents(_events);
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

        private void AssertHandledEvents(List<EventEnvelope> expectedOrderedEvents)
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
                await Task.CompletedTask;
            }
        }
    }
}
