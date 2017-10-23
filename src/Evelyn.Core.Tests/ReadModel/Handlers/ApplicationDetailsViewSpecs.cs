namespace Evelyn.Core.Tests.ReadModel.Handlers
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

    public class ApplicationDetailsViewSpecs
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

        private EnvironmentAdded _environmentAdded1;
        private EnvironmentAdded _environmentAdded2;
        private EnvironmentAdded _environmentAdded3;

        private Guid _application1Id;
        private Guid _application2Id;

        public ApplicationDetailsViewSpecs()
        {
            _fixture = new Fixture();

            _eventsApplication1 = new List<IEvent>();
            _eventsApplication2 = new List<IEvent>();
            _events = new List<IEvent>();

            _applicationsStore = null;
            _applicationDetailsStore = new InMemoryDatabase<ApplicationDetailsDto>();

            _readModelFacade = new InMemoryReadModelFacade(_applicationsStore, _applicationDetailsStore);

            var router = new Router();
            router.RegisterHandler<ApplicationCreated>(new ApplicationDetailsView(_applicationDetailsStore).Handle);
            router.RegisterHandler<EnvironmentAdded>(new ApplicationDetailsView(_applicationDetailsStore).Handle);
            _publisher = router;
        }

        [Fact]
        public void ApplicationCreated()
        {
            this.Given(_ => GivenAnApplicationIsCreated())
                .When(_ => WhenTheEventsArePublished())
                .And(_ => ThenTheApplicationDetailsCanBeRetrieved())
                .BDDfy();
        }

        [Fact]
        public void MultipleApplicationsCreated()
        {
            this.Given(_ => GivenAnApplicationIsCreated())
                .And(_ => GivenAnotherApplicationIsCreated())
                .When(_ => WhenTheEventsArePublished())
                .And(_ => ThenBothApplicationDetailsCanBeRetrieved())
                .BDDfy();
        }

        [Fact]
        public void AddingEnvironmentsToApplication()
        {
            this.Given(_ => GivenAnApplicationIsCreated())
                .And(_ => GivenAnotherApplicationIsCreated())
                .And(_ => GivenWeAddAnEnvironmentToTheFirstApplication())
                .And(_ => GivenWeAddAnEnvironmentToTheSecondApplication())
                .And(_ => GivenWeAddAnotherEnvironmentToTheFirstApplication())
                .When(_ => WhenTheEventsArePublished())
                .And(_ => ThenTheFirstAndThirdEnvironmentsAreAddedToTheFirstApplication())
                .And(_ => ThenTheSecondEnvironmentIsAddedToTheSecondApplication())
                .BDDfy();
        }

        private void GivenAnApplicationIsCreated()
        {
            _event1 = _fixture.Create<ApplicationCreated>();
            _event1.Version = _eventsApplication1.Count + 1;
            _event1.TimeStamp = DateTimeOffset.UtcNow;

            _application1Id = _event1.Id;
            _eventsApplication1.Add(_event1);
            _events.Add(_event1);
        }

        private void GivenAnotherApplicationIsCreated()
        {
            _event2 = _fixture.Create<ApplicationCreated>();
            _event2.Version = _eventsApplication2.Count + 1;
            _event2.TimeStamp = DateTimeOffset.UtcNow;

            _application2Id = _event2.Id;
            _eventsApplication2.Add(_event2);
            _events.Add(_event2);
        }

        private void GivenWeAddAnEnvironmentToTheFirstApplication()
        {
            _environmentAdded1 = _fixture.Create<EnvironmentAdded>();
            _environmentAdded1.Id = _application1Id;
            _environmentAdded1.Version = _eventsApplication1.Count() + 1;
            _environmentAdded1.TimeStamp = DateTimeOffset.UtcNow;

            _eventsApplication1.Add(_environmentAdded1);
            _events.Add(_environmentAdded1);
        }

        private void GivenWeAddAnEnvironmentToTheSecondApplication()
        {
            _environmentAdded2 = _fixture.Create<EnvironmentAdded>();
            _environmentAdded2.Id = _application2Id;
            _environmentAdded2.Version = _eventsApplication2.Count() + 1;
            _environmentAdded2.TimeStamp = DateTimeOffset.UtcNow;

            _eventsApplication2.Add(_environmentAdded2);
            _events.Add(_environmentAdded2);
        }

        private void GivenWeAddAnotherEnvironmentToTheFirstApplication()
        {
            _environmentAdded3 = _fixture.Create<EnvironmentAdded>();
            _environmentAdded3.Id = _application1Id;
            _environmentAdded3.Version = _eventsApplication1.Count() + 1;
            _environmentAdded3.TimeStamp = DateTimeOffset.UtcNow;

            _eventsApplication1.Add(_environmentAdded3);
            _events.Add(_environmentAdded3);
        }

        private void WhenTheEventsArePublished()
        {
            foreach (var ev in _events)
            {
                _publisher.Publish(ev).GetAwaiter().GetResult();
            }
        }

        private void ThenTheApplicationDetailsCanBeRetrieved()
        {
            ThenApplicationDetailsCanBeRetrievedFor(_event1);
        }

        private void ThenBothApplicationDetailsCanBeRetrieved()
        {
            ThenApplicationDetailsCanBeRetrievedFor(_event1);
            ThenApplicationDetailsCanBeRetrievedFor(_event2);
        }

        private void ThenApplicationDetailsCanBeRetrievedFor(ApplicationCreated ev)
        {
            var applicationDetails = _readModelFacade.GetApplicationDetails(ev.Id);
            applicationDetails.Id.ShouldBe(ev.Id);
            applicationDetails.Name.ShouldBe(ev.Name);
            applicationDetails.Version.ShouldBe(ev.Version);
            applicationDetails.Created.ShouldBe(ev.TimeStamp);
            applicationDetails.Environments.Count().ShouldBe(0);
        }

        private void ThenTheFirstAndThirdEnvironmentsAreAddedToTheFirstApplication()
        {
            var applicationDetails = _readModelFacade.GetApplicationDetails(_application1Id);
            ThenTheEnvironmentIsAdded(_application1Id, _environmentAdded1);
            ThenTheEnvironmentIsAdded(_application1Id, _environmentAdded3);
        }

        private void ThenTheSecondEnvironmentIsAddedToTheSecondApplication()
        {
            var applicationDetails = _readModelFacade.GetApplicationDetails(_application2Id);
            ThenTheEnvironmentIsAdded(_application2Id, _environmentAdded2);
        }

        private void ThenTheEnvironmentIsAdded(Guid applicationId, EnvironmentAdded environmentAadded)
        {
            var applicationDetails = _readModelFacade.GetApplicationDetails(_application1Id);
            var environment = applicationDetails.Environments.First();
            environment.Id.ShouldBe(_environmentAdded1.EnvironmentId);
            environment.Name.ShouldBe(_environmentAdded1.Name);
        }
    }
}
