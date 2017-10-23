namespace Evelyn.Core.Tests.ReadModel.Handlers
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using CQRSlite.Events;
    using CQRSlite.Routing;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.Dtos;
    using Evelyn.Core.ReadModel.Events;
    using Evelyn.Core.ReadModel.Handlers;
    using Evelyn.Core.ReadModel.Infrastructure;
    using Shouldly;
    using TestStack.BDDfy;
    using Xunit;

    public class ApplicationListViewSpecs
    {
        private readonly Fixture _fixture;

        private readonly IEventPublisher _publisher;
        private readonly IReadModelFacade _readModelFacade;
        private IDatabase<ApplicationListDto> _applicationsStore;
        private IDatabase<ApplicationDetailsDto> _applicationDetailsStore;

        private List<IEvent> _eventsApplication1;
        private List<IEvent> _eventsApplication2;
        private List<IEvent> _events;

        private ApplicationCreated _event1;
        private ApplicationCreated _event2;

        public ApplicationListViewSpecs()
        {
            _fixture = new Fixture();

            _eventsApplication1 = new List<IEvent>();
            _eventsApplication2 = new List<IEvent>();
            _events = new List<IEvent>();

            _applicationsStore = new InMemoryDatabase<ApplicationListDto>();
            _applicationDetailsStore = null;

            _readModelFacade = new InMemoryReadModelFacade(_applicationsStore, _applicationDetailsStore);

            var router = new Router();
            router.RegisterHandler<ApplicationCreated>(new ApplicationListView(_applicationsStore).Handle);
            _publisher = router;
        }

        [Fact]
        public void ApplicationCreated()
        {
            this.Given(_ => GivenAnApplicationIsCreated())
                .When(_ => WhenTheEventsArePublished())
                .Then(_ => ThenTheApplicationIsAddedToTheApplicationList())
                .BDDfy();
        }

        [Fact]
        public void MultipleApplicationsCreated()
        {
            this.Given(_ => GivenAnApplicationIsCreated())
                .And(_ => GivenAnotherApplicationIsCreated())
                .When(_ => WhenTheEventsArePublished())
                .Then(_ => ThenBothApplicationsAreInTheApplicationList())
                .BDDfy();
        }

        private void GivenAnApplicationIsCreated()
        {
            _event1 = _fixture.Create<ApplicationCreated>();
            _event1.Version = _eventsApplication1.Count + 1;

            _eventsApplication1.Add(_event1);
            _events.Add(_event1);
        }

        private void GivenAnotherApplicationIsCreated()
        {
            _event2 = _fixture.Create<ApplicationCreated>();
            _event2.Version = _eventsApplication2.Count + 1;

            _eventsApplication2.Add(_event2);
            _events.Add(_event2);
        }

        private void WhenTheEventsArePublished()
        {
            foreach (var ev in _events)
            {
                _publisher.Publish(ev).GetAwaiter().GetResult();
            }
        }

        private void ThenTheApplicationIsAddedToTheApplicationList()
        {
            var applications = _readModelFacade.GetApplications().ToList();
            applications.Count.ShouldBe(1);
            applications[0].Id.ShouldBe(_event1.Id);
            applications[0].Name.ShouldBe(_event1.Name);
        }

        private void ThenBothApplicationsAreInTheApplicationList()
        {
            _readModelFacade.GetApplications().Count().ShouldBe(2);

            ThenThereIsAnApplicationInTheListFor(_event1);
            ThenThereIsAnApplicationInTheListFor(_event2);
        }

        private void ThenThereIsAnApplicationInTheListFor(ApplicationCreated ev)
        {
            this._applicationsStore.Get().ShouldContain(application =>
                    application.Id == ev.Id &&
                    application.Name == ev.Name);
        }
    }
}
