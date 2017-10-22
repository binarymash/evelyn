namespace Evelyn.Core.Tests.ReadModel
{
    using System;
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

    public class ApplicationCreatedSpecs
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

        public ApplicationCreatedSpecs()
        {
            _fixture = new Fixture();

            _eventsApplication1 = new List<IEvent>();
            _eventsApplication2 = new List<IEvent>();
            _events = new List<IEvent>();

            _applicationsStore = new InMemoryDatabase<ApplicationListDto>();
            _applicationDetailsStore = new InMemoryDatabase<ApplicationDetailsDto>();

            _readModelFacade = new InMemoryReadModelFacade(_applicationsStore, _applicationDetailsStore);

            var router = new Router();
            router.RegisterHandler<ApplicationCreated>(new ApplicationListView(_applicationsStore).Handle);
            router.RegisterHandler<ApplicationCreated>(new ApplicationDetailsView(_applicationDetailsStore).Handle);
            _publisher = router;
        }

        [Fact]
        public void ApplicationCreated()
        {
            this.Given(_ => GivenAnApplicationIsCreated())
                .When(_ => WhenTheEventsArePublished())
                .Then(_ => ThenTheApplicationIsAddedToTheApplicationList())
                .And(_ => ThenTheApplicationDetailsCanBeRetrieved())
                .BDDfy();
        }

        [Fact]
        public void MultipleApplicationsCreated()
        {
            this.Given(_ => GivenAnApplicationIsCreated())
                .And(_ => GivenAnotherApplicationIsCreated())
                .When(_ => WhenTheEventsArePublished())
                .Then(_ => ThenBothApplicationsAreInTheApplicationList())
                .And(_ => ThenBothApplicationDetailsCanBeRetrieved())
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

        private void ThenTheApplicationDetailsCanBeRetrieved()
        {
            var application = _readModelFacade.GetApplicationDetails(_event1.Id);
            application.Name.ShouldBe(_event1.Name);
            application.Version.ShouldBe(1);
            application.Created.ShouldBe(_event1.TimeStamp);
        }

        private void ThenBothApplicationsAreInTheApplicationList()
        {
            _readModelFacade.GetApplications().Count().ShouldBe(2);

            ThenThereIsAnApplicationInTheListFor(_event1);
            ThenThereIsAnApplicationInTheListFor(_event2);
        }

        private void ThenBothApplicationDetailsCanBeRetrieved()
        {
            ThenApplicationDetailsCanBeRetrievedFor(_event1);
            ThenApplicationDetailsCanBeRetrievedFor(_event2);
        }

        private void ThenThereIsAnApplicationInTheListFor(ApplicationCreated ev)
        {
            this._applicationsStore.Get().ShouldContain(application =>
                    application.Id == ev.Id &&
                    application.Name == ev.Name);
        }

        private void ThenApplicationDetailsCanBeRetrievedFor(ApplicationCreated ev)
        {
            var applicationDetails = _readModelFacade.GetApplicationDetails(ev.Id);
            applicationDetails.Id.ShouldBe(ev.Id);
            applicationDetails.Name.ShouldBe(ev.Name);
            applicationDetails.Version.ShouldBe(ev.Version);
            applicationDetails.Created.ShouldBe(ev.TimeStamp);
        }
    }
}
