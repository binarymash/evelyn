namespace Evelyn.Core.Tests.ReadModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

    public class EnvironmentAddedSpecs
    {
        private readonly IEventPublisher _publisher;
        private readonly IReadModelFacade _readModelFacade;

        private Guid _application1Id;
        private string _application1Name;

        private Guid _application2Id;
        private string _application2Name;

        private Guid _environment1Id;
        private string _environment1Name;
        private string _environment1Key;

        private List<IEvent> _eventsApplication1;
        private List<IEvent> _eventsApplication2;
        private List<IEvent> _events;

        private IDatabase<ApplicationListDto> _applicationsStore;
        private IDatabase<ApplicationDetailsDto> _applicationDetailsStore;

        public EnvironmentAddedSpecs()
        {
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
                .And(_ => GivenAnEnvironmentIsAdded())
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
            _application1Id = Guid.NewGuid();
            _application1Name = "Whatever";

            var ev = new ApplicationCreated(_application1Id, _application1Name) { Version = _eventsApplication1.Count + 1 };
            _eventsApplication1.Add(ev);
            _events.Add(ev);
        }

        private void GivenAnotherApplicationIsCreated()
        {
            _application2Id = Guid.NewGuid();
            _application2Name = "Another application";

            var ev = new ApplicationCreated(_application2Id, _application2Name) { Version = _eventsApplication2.Count + 1 };
            _eventsApplication2.Add(ev);
            _events.Add(ev);
        }

        private void GivenAnEnvironmentIsAdded()
        {
            _environment1Id = Guid.NewGuid();
            _environment1Name = "My name";
            _environment1Key = "my key";

            var ev = new EnvironmentAdded(_application1Id, _environment1Id, _environment1Name, _environment1Key) { Version = _eventsApplication1.Count + 1 };
            _eventsApplication1.Add(ev);
            _events.Add(ev);
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
            applications[0].Id.ShouldBe(_application1Id);
            applications[0].Name.ShouldBe(_application1Name);
        }

        private void ThenTheApplicationDetailsCanBeRetrieved()
        {
            var application = _readModelFacade.GetApplicationDetails(_application1Id);
            application.Name.ShouldBe(_application1Name);
        }

        private void ThenBothApplicationsAreInTheApplicationList()
        {
            var applications = _readModelFacade.GetApplications().ToList();
            applications.Count.ShouldBe(2);
            applications[0].Id.ShouldBe(_application1Id);
            applications[0].Name.ShouldBe(_application1Name);
            applications[1].Id.ShouldBe(_application2Id);
            applications[1].Name.ShouldBe(_application2Name);
        }

        private void ThenBothApplicationDetailsCanBeRetrieved()
        {
            var application = _readModelFacade.GetApplicationDetails(_application1Id);
            application.Name.ShouldBe(_application1Name);

            application = _readModelFacade.GetApplicationDetails(_application2Id);
            application.Name.ShouldBe(_application2Name);
        }
    }
}
