namespace Evelyn.Core.Tests.ReadModel.EventStream.Handlers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using CQRSlite.Events;
    using Evelyn.Core.Infrastructure;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.EventStream;
    using Evelyn.Core.ReadModel.EventStream.Handlers;
    using Evelyn.Core.ReadModel.Projections;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class EventHandlerSpecs
    {
        private readonly Fixture _dataFixture;
        private readonly CancellationTokenSource _stoppingTokenSource;
        private readonly IProjectionStore<Core.ReadModel.Projections.EventHandlerState.Projection> _eventHandlerStateStore;
        private readonly ServiceProvider _serviceProvider;
        private readonly JsonSerializerSettings _deserializeWithPrivateSetters;

        private Core.ReadModel.EventStream.Handlers.EventHandler<SomeStream> _eventHandler;
        private EventEnvelope _eventEnvelope;
        private Core.ReadModel.Projections.EventHandlerState.Projection _originalEventHandlerState;
        private Core.ReadModel.Projections.EventHandlerState.Projection _updatedEventHandlerState;
        private Exception _thrownException;

        public EventHandlerSpecs()
        {
            _dataFixture = new Fixture();
            _stoppingTokenSource = new CancellationTokenSource();

            _deserializeWithPrivateSetters = new JsonSerializerSettings
            {
                ContractResolver = new JsonPrivateResolver()
            };

            _eventHandlerStateStore = Substitute.For<IProjectionStore<Core.ReadModel.Projections.EventHandlerState.Projection>>();

            _eventHandlerStateStore
                .WhenForAnyArgs((IProjectionStore<Core.ReadModel.Projections.EventHandlerState.Projection> ps) => ps.Create(Arg.Any<string>(), Arg.Any<Core.ReadModel.Projections.EventHandlerState.Projection>()))
                .Do(ci => _updatedEventHandlerState = ci.ArgAt<Core.ReadModel.Projections.EventHandlerState.Projection>(1));

            _eventHandlerStateStore
                .WhenForAnyArgs((IProjectionStore<Core.ReadModel.Projections.EventHandlerState.Projection> ps) => ps.Update(Arg.Any<string>(), Arg.Any<Core.ReadModel.Projections.EventHandlerState.Projection>()))
                .Do(ci => _updatedEventHandlerState = ci.ArgAt<Core.ReadModel.Projections.EventHandlerState.Projection>(1));

            IServiceCollection services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton(_eventHandlerStateStore);
            services.AddSingleton<IProjectionBuilderRegistrar, ProjectionBuilderRegistrar>();
            services.AddSingleton<Core.ReadModel.EventStream.Handlers.EventHandler<SomeStream>>();
            services.AddSingleton<ProjectionBuilder1>();
            services.AddSingleton<ProjectionBuilder2>();
            services.AddSingleton<ProjectionBuilder3>();
            services.AddSingleton<ProjectionBuilder4>();

            _serviceProvider = services.BuildServiceProvider();

            HandledEventTracker.Reset();
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
        }

        private void GivenThereAreProjectionBuildersForOtherEvents()
        {
            var projectionBuilders = new List<Type>()
            {
                typeof(ProjectionBuilder3),
                typeof(ProjectionBuilder4)
            };

            _serviceProvider
                .GetService<IProjectionBuilderRegistrar>()
                .Register(typeof(SomeStream), projectionBuilders);
        }

        private void GivenThereAreProjectionBuildersForOurEvent()
        {
            var projectionBuilders = new List<Type>()
            {
                typeof(ProjectionBuilder1),
                typeof(ProjectionBuilder2)
            };

            _serviceProvider
                .GetService<IProjectionBuilderRegistrar>()
                .Register(typeof(SomeStream), projectionBuilders);
        }

        private void GivenOurFirstProjectionBuilderThrowsAnException()
        {
            HandledEventTracker.ThrowExceptionOnInvocation(typeof(ProjectionBuilder1));
        }

        private void GivenWeReceiveAnEvent()
        {
            _eventEnvelope = new EventEnvelope(
                _dataFixture.Create<long>(),
                _dataFixture.Create<SomeEvent>());
        }

        private void GivenThereIsNoEventHandlerState()
        {
            SubstituteExtensions.Returns(
                _eventHandlerStateStore.Get(Arg.Any<string>()),
                (Func<NSubstitute.Core.CallInfo, Core.ReadModel.Projections.EventHandlerState.Projection>)(ps => throw new ProjectionNotFoundException()));
        }

        private void GivenTheEventHasNotYetBeenHandled()
        {
            var eventAudit = EventAudit.Create(
                _dataFixture.Create<DateTimeOffset>(),
                _dataFixture.Create<string>(),
                _dataFixture.Create<long>(),
                _eventEnvelope.StreamVersion - 1);

            _originalEventHandlerState = Core.ReadModel.Projections.EventHandlerState.Projection.Create(eventAudit);

            _eventHandlerStateStore
                .Get(Arg.Any<string>())
                .Returns(_originalEventHandlerState);
        }

        private void GivenTheEventHasAlreadyBeenHandled()
        {
            var eventAudit = EventAudit.Create(
                _dataFixture.Create<DateTimeOffset>(),
                _dataFixture.Create<string>(),
                _dataFixture.Create<long>(),
                _eventEnvelope.StreamVersion + 1);

            _originalEventHandlerState = Core.ReadModel.Projections.EventHandlerState.Projection.Create(eventAudit);

            _eventHandlerStateStore
                .Get(Arg.Any<string>())
                .Returns(_originalEventHandlerState);
        }

        private async Task WhenWeHandleTheEvent()
        {
            _eventHandler = _serviceProvider.GetService<Core.ReadModel.EventStream.Handlers.EventHandler<SomeStream>>();

            try
            {
                await _eventHandler.HandleEvent(_eventEnvelope, _stoppingTokenSource.Token);
            }
            catch (Exception ex)
            {
                _thrownException = ex;
            }
        }

        private void ThenTheProjectionBuildersForTheOtherEventsAreNotInvoked()
        {
            HandledEventTracker.CallCount(typeof(ProjectionBuilder3)).Should().Be(0);
            HandledEventTracker.CallCount(typeof(ProjectionBuilder4)).Should().Be(0);
        }

        private void ThenTheProjectionBuildersForOurEventAreNotInvoked()
        {
            HandledEventTracker.CallCount(typeof(ProjectionBuilder1)).Should().Be(0);
            HandledEventTracker.CallCount(typeof(ProjectionBuilder2)).Should().Be(0);
        }

        private void ThenTheProjectionBuildersForOurEventAreInvoked()
        {
            HandledEventTracker.CallCount(typeof(ProjectionBuilder1)).Should().Be(1);
            HandledEventTracker.CallCount(typeof(ProjectionBuilder2)).Should().Be(1);
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
            _eventHandlerStateStore.Received().Create(Core.ReadModel.Projections.EventHandlerState.Projection.StoreKey(typeof(SomeStream)), _updatedEventHandlerState);
            _updatedEventHandlerState.Audit.Version.Should().Be(_eventEnvelope.StreamVersion);
        }

        private void ThenTheStateIsUpdated()
        {
            _eventHandlerStateStore.Received().Update(Core.ReadModel.Projections.EventHandlerState.Projection.StoreKey(typeof(SomeStream)), _updatedEventHandlerState);
            _updatedEventHandlerState.Audit.Version.Should().Be(_eventEnvelope.StreamVersion);
        }

        private void ThenTheStateIsNotUpdated()
        {
            _eventHandlerStateStore.DidNotReceive().Create(Core.ReadModel.Projections.EventHandlerState.Projection.StoreKey(typeof(SomeStream)), _updatedEventHandlerState);
            _eventHandlerStateStore.DidNotReceive().Update(Core.ReadModel.Projections.EventHandlerState.Projection.StoreKey(typeof(SomeStream)), _updatedEventHandlerState);
            _eventHandlerStateStore.DidNotReceive().Delete(Core.ReadModel.Projections.EventHandlerState.Projection.StoreKey(typeof(SomeStream)));
        }

        private Core.ReadModel.Projections.EventHandlerState.Projection CopyOf(Core.ReadModel.Projections.EventHandlerState.Projection original)
        {
            var serializedDto = JsonConvert.SerializeObject(original);
            return JsonConvert.DeserializeObject<Core.ReadModel.Projections.EventHandlerState.Projection>(serializedDto, _deserializeWithPrivateSetters);
        }

        public static class HandledEventTracker
        {
            private static List<Type> _throwExceptionOnInvocation = new List<Type>();

            private static ConcurrentDictionary<Type, int> _callCount = new ConcurrentDictionary<Type, int>();

            public static void Reset()
            {
                _throwExceptionOnInvocation = new List<Type>();
                _callCount = new ConcurrentDictionary<Type, int>();
            }

            public static int CallCount(Type projectionBuilderType)
            {
                if (!_callCount.TryGetValue(projectionBuilderType, out int callCount))
                {
                    callCount = 0;
                }

                return callCount;
            }

            public static async Task HandleEvent(Type projectionBuilderType, IEvent @event, CancellationToken stoppingToken)
            {
                IncrementCallCount(projectionBuilderType);

                if (_throwExceptionOnInvocation.Contains(projectionBuilderType))
                {
                    throw new Exception("boom");
                }

                await Task.CompletedTask;
            }

            public static void ThrowExceptionOnInvocation(Type projectionBuilderType)
            {
                _throwExceptionOnInvocation.Add(projectionBuilderType);
            }

            private static void IncrementCallCount(Type projectionBuilderType)
            {
                _callCount.AddOrUpdate(projectionBuilderType, 1, (pbt, cc) => cc++);
            }
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

        public class ProjectionBuilder1 : IBuildProjectionsFrom<SomeEvent>
        {
            public async Task Handle(long streamVersion, SomeEvent @event, CancellationToken stoppingToken)
            {
                await HandledEventTracker.HandleEvent(this.GetType(), @event, stoppingToken);
            }
        }

        public class ProjectionBuilder2 : IBuildProjectionsFrom<SomeEvent>
        {
            public async Task Handle(long streamVersion, SomeEvent @event, CancellationToken stoppingToken)
            {
                await HandledEventTracker.HandleEvent(this.GetType(), @event, stoppingToken);
            }
        }

        public class ProjectionBuilder3 : IBuildProjectionsFrom<SomeOtherEvent>
        {
            public async Task Handle(long streamVersion, SomeOtherEvent @event, CancellationToken stoppingToken)
            {
                await HandledEventTracker.HandleEvent(this.GetType(), @event, stoppingToken);
            }
        }

        public class ProjectionBuilder4 : IBuildProjectionsFrom<SomeOtherEvent>
        {
            public async Task Handle(long streamVersion, SomeOtherEvent @event, CancellationToken stoppingToken)
            {
                await HandledEventTracker.HandleEvent(this.GetType(), @event, stoppingToken);
            }
        }
    }
}
