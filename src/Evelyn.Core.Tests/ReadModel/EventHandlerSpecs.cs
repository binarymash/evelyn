namespace Evelyn.Core.Tests.ReadModel
{
    using System;
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
        private Exception _thrownException;
        private EventHandlerStateDto _originalEventHandlerState;
        private EventHandlerStateDto _updatedEventHandlerState;

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
            _stoppingToken = default;
        }

        [Fact]
        public void NoRegisteredProjectionBuilders()
        {
            this.Given(_ => _.GivenThereAreNoRegisteredProjectionBuilders())
                .And(_ => _.GivenThereIsNoEventHandlerState())
                .When(_ => _.WhenWeHandleAnEvent())
                .Then(_ => _.ThenTheStateIsCreated())
                .Then(_ => _.ThenNoExceptionIsThrown())
                .BDDfy();
        }

        private void GivenThereAreNoRegisteredProjectionBuilders()
        {
            _projectionBuilderRegistrar
                .Get(Arg.Any<Type>())
                .Returns(ProjectionBuildersByEventType.Null);
        }

        private void GivenThereIsNoEventHandlerState()
        {
            _originalEventHandlerState = null;

            _eventHandlerStateStore
                .Get(Arg.Any<string>())
                .Returns(EventHandlerStateDto.Null);
        }

        private async Task WhenWeHandleAnEvent()
        {
            _eventHandler = new Core.ReadModel.EventHandler<SomeStream>(_logger, _eventHandlerStateStore, _projectionBuilderRegistrar);

            _eventEnvelope = new EventEnvelope(
                _dataFixture.Create<long>(),
                _dataFixture.Create<SomeEvent>());

            try
            {
                await _eventHandler.HandleEvent(_eventEnvelope, _stoppingToken);
            }
            catch (Exception ex)
            {
                _thrownException = ex;
            }
        }

        private void ThenNoExceptionIsThrown()
        {
            _thrownException.Should().BeNull();
        }

        private void ThenTheStateIsCreated()
        {
            _eventHandlerStateStore.Received().Create(EventHandlerStateDto.StoreKey(typeof(SomeStream)), _updatedEventHandlerState);
            _updatedEventHandlerState.Version.Should().Be(_eventEnvelope.StreamVersion);
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

        private EventHandlerStateDto CopyOf(EventHandlerStateDto original)
        {
            var serializedDto = JsonConvert.SerializeObject(original);
            return JsonConvert.DeserializeObject<EventHandlerStateDto>(serializedDto, _deserializeWithPrivateSetters);
        }
    }
}
