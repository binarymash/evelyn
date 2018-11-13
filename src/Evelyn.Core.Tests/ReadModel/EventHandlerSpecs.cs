namespace Evelyn.Core.Tests.ReadModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using CQRSlite.Events;
    using Evelyn.Core.Infrastructure;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.Projections;
    using Evelyn.Core.ReadModel.Projections.EventHandlerState;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class EventHandlerSpecs
    {
        private readonly Fixture _dataFixture;
        private readonly CancellationToken _stoppingToken;
        private readonly ILogger<Core.ReadModel.EventHandler<SomeStream>> _logger;
        private readonly IProjectionStore<EventHandlerStateDto> _eventHandlerStateStore;
        private readonly IProjectionBuilderRegistrar _projectionBuilderRegistrar;
        private readonly JsonSerializerSettings _deserializeWithPrivateSetters;

        private Core.ReadModel.EventHandler<SomeStream> _eventHandler;
        private EventEnvelope _eventEnvelope;
        private ProjectionBuildersByEventType _projectionBuildersByEventType;
        private EventHandlerStateDto _originalEventHandlerState;
        private EventHandlerStateDto _updatedEventHandlerState;
        private StubbedProjectionBuilder _someEventProjectionBuilder1;
        private StubbedProjectionBuilder _someEventProjectionBuilder2;
        private StubbedProjectionBuilder _someOtherEventProjectionBuilder1;
        private StubbedProjectionBuilder _someOtherEventProjectionBuilder2;
        private Exception _thrownException;

        public EventHandlerSpecs()
        {
            _dataFixture = new Fixture();

            _logger = Substitute.For<ILogger<Core.ReadModel.EventHandler<SomeStream>>>();

            _deserializeWithPrivateSetters = new JsonSerializerSettings
            {
                ContractResolver = new JsonPrivateResolver()
            };

            _eventHandlerStateStore = Substitute.For<IProjectionStore<EventHandlerStateDto>>();
            _eventHandlerStateStore.Get(Arg.Any<string>())
                .Returns(ps => CopyOf(_originalEventHandlerState));
            _eventHandlerStateStore.WhenForAnyArgs(ps => ps.Create(Arg.Any<string>(), Arg.Any<EventHandlerStateDto>()))
                .Do(ci => _updatedEventHandlerState = ci.ArgAt<EventHandlerStateDto>(1));
            _eventHandlerStateStore.WhenForAnyArgs(ps => ps.Update(Arg.Any<string>(), Arg.Any<EventHandlerStateDto>()))
                .Do(ci => _updatedEventHandlerState = ci.ArgAt<EventHandlerStateDto>(1));

            _projectionBuilderRegistrar = Substitute.For<IProjectionBuilderRegistrar>();
            _projectionBuildersByEventType = new ProjectionBuildersByEventType();

            _stoppingToken = default;
        }

        [Fact]
        public void NoProjectionBuilders()
        {
            this.Given(_ => _.GivenThereAreNoRegisteredProjectionBuilders())
                .And(_ => _.GivenWeReceiveAnEvent())
                .And(_ => _.GivenThereIsNoEventHandlerState())
                .When(_ => _.WhenWeHandleTheEvent())
                .Then(_ => _.ThenTheStateIsCreated())
                .Then(_ => _.ThenNoExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void EventWithoutProjectionBuildersHasNotYetBeenHandled()
        {
            this.Given(_ => _.GivenThereAreProjectionBuildersForOtherEvents())
                .And(_ => _.GivenWeReceiveAnEvent())
                .And(_ => _.GivenTheEventHasNotYetBeenHandled())
                .When(_ => _.WhenWeHandleTheEvent())
                .Then(_ => _.ThenTheProjectionBuildersForTheOtherEventsAreNotInvoked())
                .And(_ => _.ThenTheStateIsUpdated())
                .And(_ => _.ThenNoExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void EventWithoutProjectionBuildersHasAlreadyBeenHandled()
        {
            this.Given(_ => _.GivenThereAreProjectionBuildersForOtherEvents())
                .And(_ => _.GivenWeReceiveAnEvent())
                .And(_ => _.GivenTheEventHasAlreadyBeenHandled())
                .When(_ => _.WhenWeHandleTheEvent())
                .And(_ => _.ThenTheProjectionBuildersForTheOtherEventsAreNotInvoked())
                .And(_ => _.ThenTheStateIsNotUpdated())
                .And(_ => _.ThenNoExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void EventWithProjectionBuildersHasNotYetBeenHandled()
        {
            this.Given(_ => _.GivenThereAreProjectionBuildersForOtherEvents())
                .And(_ => _.GivenThereAreProjectionBuildersForOurEvent())
                .And(_ => _.GivenWeReceiveAnEvent())
                .And(_ => _.GivenTheEventHasNotYetBeenHandled())
                .When(_ => _.WhenWeHandleTheEvent())
                .Then(_ => _.ThenTheProjectionBuildersForOurEventAreInvoked())
                .And(_ => _.ThenTheProjectionBuildersForTheOtherEventsAreNotInvoked())
                .And(_ => _.ThenTheStateIsUpdated())
                .And(_ => _.ThenNoExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void EventWithProjectionBuildersHasAlreadyBeenHandled()
        {
            this.Given(_ => _.GivenThereAreProjectionBuildersForOtherEvents())
                .And(_ => _.GivenThereAreProjectionBuildersForOurEvent())
                .And(_ => _.GivenWeReceiveAnEvent())
                .And(_ => _.GivenTheEventHasAlreadyBeenHandled())
                .When(_ => _.WhenWeHandleTheEvent())
                .Then(_ => _.ThenTheProjectionBuildersForOurEventAreNotInvoked())
                .And(_ => _.ThenTheProjectionBuildersForTheOtherEventsAreNotInvoked())
                .And(_ => _.ThenTheStateIsNotUpdated())
                .And(_ => _.ThenNoExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void ProjectionBuilderThrowAnException()
        {
            this.Given(_ => _.GivenThereAreProjectionBuildersForOurEvent())
                .And(_ => _.GivenWeReceiveAnEvent())
                .And(_ => _.GivenTheEventHasNotYetBeenHandled())
                .And(_ => _.GivenOurFirstProjectionBuilderThrowsAnException())
                .When(_ => _.WhenWeHandleTheEvent())
                .Then(_ => _.ThenTheProjectionBuildersForOurEventAreInvoked())
                .And(_ => _.ThenTheStateIsNotUpdated())
                .And(_ => _.ThenAnExceptionIsThrown())
                .BDDfy();
        }

        private void GivenThereAreNoRegisteredProjectionBuilders()
        {
            _projectionBuildersByEventType = ProjectionBuildersByEventType.Null;
        }

        private void GivenThereAreProjectionBuildersForOtherEvents()
        {
            _someOtherEventProjectionBuilder1 = new StubbedProjectionBuilder();
            _someOtherEventProjectionBuilder2 = new StubbedProjectionBuilder();

            var projectionBuildersForSomeOtherEvent = new List<Func<IEvent, CancellationToken, Task>>()
            {
                _someOtherEventProjectionBuilder1.HandleEvent,
                _someOtherEventProjectionBuilder2.HandleEvent,
            };

            _projectionBuildersByEventType.Add(typeof(SomeOtherEvent), projectionBuildersForSomeOtherEvent);
        }

        private void GivenThereAreProjectionBuildersForOurEvent()
        {
            _someEventProjectionBuilder1 = new StubbedProjectionBuilder();
            _someEventProjectionBuilder2 = new StubbedProjectionBuilder();

            var projectionBuildersForSomeEvent = new List<Func<IEvent, CancellationToken, Task>>()
            {
                _someEventProjectionBuilder1.HandleEvent,
                _someEventProjectionBuilder2.HandleEvent,
            };

            _projectionBuildersByEventType.Add(typeof(SomeEvent), projectionBuildersForSomeEvent);
        }

        private void GivenOurFirstProjectionBuilderThrowsAnException()
        {
            _someEventProjectionBuilder1.ThrowExceptionOnInvocation();
        }

        private void GivenWeReceiveAnEvent()
        {
            _eventEnvelope = new EventEnvelope(
                _dataFixture.Create<long>(),
                _dataFixture.Create<SomeEvent>());
        }

        private void GivenThereIsNoEventHandlerState()
        {
            _originalEventHandlerState = EventHandlerStateDto.Null;
        }

        private void GivenTheEventHasNotYetBeenHandled()
        {
            _originalEventHandlerState = new EventHandlerStateDto(
                _eventEnvelope.StreamVersion - 1,
                _dataFixture.Create<DateTimeOffset>(),
                _dataFixture.Create<string>(),
                _dataFixture.Create<DateTimeOffset>(),
                _dataFixture.Create<string>());
        }

        private void GivenTheEventHasAlreadyBeenHandled()
        {
            _originalEventHandlerState = new EventHandlerStateDto(
                _eventEnvelope.StreamVersion + 1,
                _dataFixture.Create<DateTimeOffset>(),
                _dataFixture.Create<string>(),
                _dataFixture.Create<DateTimeOffset>(),
                _dataFixture.Create<string>());
        }

        private async Task WhenWeHandleTheEvent()
        {
            _projectionBuilderRegistrar
                .Get(Arg.Any<Type>())
                .Returns(_projectionBuildersByEventType);

            _eventHandler = new Core.ReadModel.EventHandler<SomeStream>(_logger, _eventHandlerStateStore, _projectionBuilderRegistrar);

            try
            {
                await _eventHandler.HandleEvent(_eventEnvelope, _stoppingToken);
            }
            catch (Exception ex)
            {
                _thrownException = ex;
            }
        }

        private void ThenTheProjectionBuildersForTheOtherEventsAreNotInvoked()
        {
            _someOtherEventProjectionBuilder1.CallCount.Should().Be(0);
            _someOtherEventProjectionBuilder2.CallCount.Should().Be(0);
        }

        private void ThenTheProjectionBuildersForOurEventAreNotInvoked()
        {
            _someEventProjectionBuilder1.CallCount.Should().Be(0);
            _someEventProjectionBuilder2.CallCount.Should().Be(0);
        }

        private void ThenTheProjectionBuildersForOurEventAreInvoked()
        {
            _someEventProjectionBuilder1.CallCount.Should().Be(1);
            _someEventProjectionBuilder2.CallCount.Should().Be(1);
        }

        private void ThenNoExceptionIsThrown()
        {
            _thrownException.Should().BeNull();
        }

        private void ThenAnExceptionIsThrown()
        {
            _thrownException.Should().NotBeNull();
        }

        private void ThenTheStateIsCreated()
        {
            _eventHandlerStateStore.Received().Create(EventHandlerStateDto.StoreKey(typeof(SomeStream)), _updatedEventHandlerState);
            _updatedEventHandlerState.Version.Should().Be(_eventEnvelope.StreamVersion);
        }

        private void ThenTheStateIsUpdated()
        {
            _eventHandlerStateStore.Received().Update(EventHandlerStateDto.StoreKey(typeof(SomeStream)), _updatedEventHandlerState);
            _updatedEventHandlerState.Version.Should().Be(_eventEnvelope.StreamVersion);
        }

        private void ThenTheStateIsNotUpdated()
        {
            _eventHandlerStateStore.DidNotReceive().Create(EventHandlerStateDto.StoreKey(typeof(SomeStream)), _updatedEventHandlerState);
            _eventHandlerStateStore.DidNotReceive().Update(EventHandlerStateDto.StoreKey(typeof(SomeStream)), _updatedEventHandlerState);
            _eventHandlerStateStore.DidNotReceive().Delete(EventHandlerStateDto.StoreKey(typeof(SomeStream)));
        }

        private EventHandlerStateDto CopyOf(EventHandlerStateDto original)
        {
            var serializedDto = JsonConvert.SerializeObject(original);
            return JsonConvert.DeserializeObject<EventHandlerStateDto>(serializedDto, _deserializeWithPrivateSetters);
        }

        public class SomeStream
        {
        }

        public class SomeEvent : IEvent
        {
            public Guid Id { get; set; }

            public int Version { get; set; }

            public DateTimeOffset TimeStamp { get; set; }
        }

        public class SomeOtherEvent : IEvent
        {
            public Guid Id { get; set; }

            public int Version { get; set; }

            public DateTimeOffset TimeStamp { get; set; }
        }

        public class StubbedProjectionBuilder
        {
            private bool _throwExceptionOnInvocation = false;

            public int CallCount { get; private set; }

            public async Task HandleEvent(IEvent @event, CancellationToken stoppingToken)
            {
                CallCount++;

                if (_throwExceptionOnInvocation)
                {
                    throw new Exception("boom");
                }

                await Task.CompletedTask;
            }

            public void ThrowExceptionOnInvocation()
            {
                _throwExceptionOnInvocation = true;
            }
        }
    }
}
