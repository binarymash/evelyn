////namespace Evelyn.Core.Tests.ReadModel.ToggleDetails
////{
////    using System;
////    using System.Threading;
////    using System.Threading.Tasks;
////    using AutoFixture;
////    using Core.ReadModel;
////    using Core.ReadModel.Infrastructure;
////    using Core.ReadModel.Projections.ToggleDetails;
////    using Core.WriteModel.Project.Events;
////    using CQRSlite.Events;
////    using FluentAssertions;
////    using NSubstitute;
////    using TestStack.BDDfy;
////    using Xunit;

////    public class EventStreamHandlerSpecs
////    {
////        private readonly Fixture _fixture;
////        private readonly EventStreamHandler _handler;
////        private readonly StubbedEventStreamFactory _eventStreamFactory;
////        private readonly IProjectionBuilder<ProjectionBuilderRequest, ToggleDetailsDto> _projectionBuilder;
////        private readonly IDatabase<string, ToggleDetailsDto> _db;

////        private ToggleAdded _event1;
////        private ToggleAdded _event2;
////        private ToggleDetailsDto _projection1;
////        private ToggleDetailsDto _projection2;

////        public EventStreamHandlerSpecs()
////        {
////            _fixture = new Fixture();
////            _projectionBuilder = Substitute.For<IProjectionBuilder<ProjectionBuilderRequest, ToggleDetailsDto>>();
////            _db = Substitute.For<IDatabase<string, ToggleDetailsDto>>();
////            _eventStreamFactory = new StubbedEventStreamFactory();
////            _handler = new EventStreamHandler(_projectionBuilder, _db, _eventStreamFactory);

////            _handler.StartAsync(new CancellationToken(false));
////        }

////        [Fact]
////        public void NoEventsInStream()
////        {
////            this.Given(_ => GivenNothing())
////                .When(_ => WhenThereAreNoEventsInTheStream())
////                .And(_ => WhenWeWaitAMoment())
////                .Then(_ => ThenNoProjectionIsBuilt())
////                .And(_ => ThenNoProjectionIsCached())
////                .BDDfy();
////        }

////        [Fact]
////        public void OneEventInStream()
////        {
////            this.Given(_ => GivenAProjectionCanBeBuiltForEvent1())
////                .When(_ => WhenEvent1IsAddedToTheEventStream())
////                .And(_ => WhenWeWaitAMoment())
////                .Then(_ => ThenAProjectionWasBuiltForEvent1())
////                .And(_ => ThenTheProjectionIsCachedForEvent1())
////                .And(_ => ThenEvent1IsRemovedFromTheStream())
////                .BDDfy();
////        }

////        [Fact]
////        public void MultipleEventsInStream()
////        {
////            this.Given(_ => GivenAProjectionCanBeBuiltForEvent1And2())
////                .When(_ => WhenEvent1IsAddedToTheEventStream())
////                .And(_ => WhenEvent2IsAddedToTheEventStream())
////                .And(_ => WhenWeWaitAMoment())
////                .Then(_ => ThenAProjectionWasBuiltForEvent1())
////                .And(_ => ThenTheProjectionIsCachedForEvent1())
////                .And(_ => ThenEvent1IsRemovedFromTheStream())
////                .Then(_ => ThenAProjectionWasBuiltForEvent2())
////                .And(_ => ThenTheProjectionIsCachedForEvent2())
////                .And(_ => ThenEvent2IsRemovedFromTheStream())
////                .BDDfy();
////        }

////        [Fact]
////        public void NullProjection()
////        {
////            this.Given(_ => GivenANullProjectionWillBeBuiltForEvent1())
////                .When(_ => WhenEvent1IsAddedToTheEventStream())
////                .And(_ => WhenWeWaitAMoment())
////                .Then(_ => ThenAProjectionWasBuiltForEvent1())
////                .And(_ => ThenTheProjectionCacheIdDeletedForEvent1())
////                .And(_ => ThenEvent1IsRemovedFromTheStream())
////                .BDDfy();
////        }

////        private void GivenNothing()
////        {
////        }

////        private void GivenAProjectionCanBeBuiltForEvent1()
////        {
////            _projection1 = _fixture.Create<ToggleDetailsDto>();
////            _projectionBuilder.Invoke(Arg.Any<ProjectionBuilderRequest>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(_projection1));
////        }

////        private void GivenANullProjectionWillBeBuiltForEvent1()
////        {
////            _projection1 = null;
////            _projectionBuilder.Invoke(Arg.Any<ProjectionBuilderRequest>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(_projection1));
////        }

////        private void GivenAProjectionCanBeBuiltForEvent1And2()
////        {
////            _projection1 = _fixture.Create<ToggleDetailsDto>();
////            _projection2 = _fixture.Create<ToggleDetailsDto>();
////            _projectionBuilder.Invoke(Arg.Any<ProjectionBuilderRequest>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(_projection1), Task.FromResult(_projection2));
////        }

////        private void WhenThereAreNoEventsInTheStream()
////        {
////        }

////        private void WhenEvent1IsAddedToTheEventStream()
////        {
////            _event1 = _fixture.Create<ToggleAdded>();
////            _eventStreamFactory.GetEventStream<ToggleDetailsDto>().Enqueue(_event1);
////        }

////        private void WhenEvent2IsAddedToTheEventStream()
////        {
////            _event2 = _fixture.Create<ToggleAdded>();
////            _eventStreamFactory.GetEventStream<ToggleDetailsDto>().Enqueue(_event2);
////        }

////        private async Task WhenWeWaitAMoment()
////        {
////            await Task.Delay(TimeSpan.FromSeconds(2));
////        }

////        private void ThenNoProjectionIsBuilt()
////        {
////            _projectionBuilder.DidNotReceiveWithAnyArgs().Invoke(Arg.Any<ProjectionBuilderRequest>());
////        }

////        private void ThenAProjectionWasBuiltForEvent1()
////        {
////            _projectionBuilder.Received().Invoke(Arg.Is<ProjectionBuilderRequest>(pbr => pbr.ProjectId == _event1.Id), Arg.Any<CancellationToken>());
////        }

////        private void ThenAProjectionWasBuiltForEvent2()
////        {
////            _projectionBuilder.Received().Invoke(Arg.Is<ProjectionBuilderRequest>(pbr => pbr.ProjectId == _event2.Id), Arg.Any<CancellationToken>());
////        }

////        private void ThenNoProjectionIsCached()
////        {
////            _db.DidNotReceiveWithAnyArgs().AddOrUpdate(Arg.Any<string>(), Arg.Any<ToggleDetailsDto>());
////        }

////        private void ThenTheProjectionIsCachedForEvent1()
////        {
////            _db.Received().AddOrUpdate($"{_event1.Id}-{_event1.Key}", _projection1);
////        }

////        private void ThenTheProjectionIsCachedForEvent2()
////        {
////            _db.Received().AddOrUpdate($"{_event2.Id}-{_event2.Key}", _projection2);
////        }

////        private void ThenTheProjectionCacheIdDeletedForEvent1()
////        {
////            _db.Received().Delete($"{_event1.Id}-{_event1.Key}");
////        }

////        private void ThenEvent1IsRemovedFromTheStream()
////        {
////            ThenTheEventStreamDoesNotContain(_event1);
////        }

////        private void ThenEvent2IsRemovedFromTheStream()
////        {
////            ThenTheEventStreamDoesNotContain(_event2);
////        }

////        private void ThenTheEventStreamDoesNotContain(IEvent @event)
////        {
////            _eventStreamFactory.GetEventStream<ToggleDetailsDto>().Should().NotContain(@event);
////        }
////    }
////}
