namespace Evelyn.Core.Tests.ReadModel.ApplicationDetails
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using CQRSlite.Events;
    using CQRSlite.Routing;
    using Evelyn.Core.ReadModel.ApplicationDetails;
    using Evelyn.Core.ReadModel.Events;
    using Shouldly;
    using TestStack.BDDfy;
    using Xunit;

    public class ApplicationDetailsHandlerSpecs : HandlerSpecs
    {
        private List<IEvent> _eventsApplication1;
        private List<IEvent> _eventsApplication2;

        private ApplicationCreated _event1;
        private ApplicationCreated _event2;

        private EnvironmentAdded _environmentAdded1;
        private EnvironmentAdded _environmentAdded2;
        private EnvironmentAdded _environmentAdded3;

        private Guid _application1Id;
        private Guid _application2Id;

        public ApplicationDetailsHandlerSpecs()
        {
            _eventsApplication1 = new List<IEvent>();
            _eventsApplication2 = new List<IEvent>();
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
                .And(_ => ThenTheVersionOfTheFirstApplicationHasBeenUpdated())
                .And(_ => ThenTheLastModifiedTimeOfTheFirstApplicationHasBeenUpdated())
                .And(_ => ThenTheSecondEnvironmentIsAddedToTheSecondApplication())
                .And(_ => ThenTheVersionOfTheSecondApplicationHasBeenUpdated())
                .And(_ => ThenTheLastModifiedTimeOfTheSecondApplicationHasBeenUpdated())
                .BDDfy();
        }

        protected override void RegisterHandlers(Router router)
        {
            var handler = new ApplicationDetailsHandler(ApplicationDetailsStore);
            router.RegisterHandler<ApplicationCreated>(handler.Handle);
            router.RegisterHandler<EnvironmentAdded>(handler.Handle);
        }

        private void GivenAnApplicationIsCreated()
        {
            _event1 = DataFixture.Create<ApplicationCreated>();
            _event1.Version = _eventsApplication1.Count + 1;
            _event1.TimeStamp = DateTimeOffset.UtcNow;

            _application1Id = _event1.Id;
            _eventsApplication1.Add(_event1);
            Events.Add(_event1);
        }

        private void GivenAnotherApplicationIsCreated()
        {
            _event2 = DataFixture.Create<ApplicationCreated>();
            _event2.Version = _eventsApplication2.Count + 1;
            _event2.TimeStamp = DateTimeOffset.UtcNow;

            _application2Id = _event2.Id;
            _eventsApplication2.Add(_event2);
            Events.Add(_event2);
        }

        private void GivenWeAddAnEnvironmentToTheFirstApplication()
        {
            _environmentAdded1 = DataFixture.Create<EnvironmentAdded>();
            _environmentAdded1.Id = _application1Id;
            _environmentAdded1.Version = _eventsApplication1.Count() + 1;
            _environmentAdded1.TimeStamp = DateTimeOffset.UtcNow;

            _eventsApplication1.Add(_environmentAdded1);
            Events.Add(_environmentAdded1);
        }

        private void GivenWeAddAnEnvironmentToTheSecondApplication()
        {
            _environmentAdded2 = DataFixture.Create<EnvironmentAdded>();
            _environmentAdded2.Id = _application2Id;
            _environmentAdded2.Version = _eventsApplication2.Count() + 1;
            _environmentAdded2.TimeStamp = DateTimeOffset.UtcNow;

            _eventsApplication2.Add(_environmentAdded2);
            Events.Add(_environmentAdded2);
        }

        private void GivenWeAddAnotherEnvironmentToTheFirstApplication()
        {
            _environmentAdded3 = DataFixture.Create<EnvironmentAdded>();
            _environmentAdded3.Id = _application1Id;
            _environmentAdded3.Version = _eventsApplication1.Count() + 1;
            _environmentAdded3.TimeStamp = DateTimeOffset.UtcNow;

            _eventsApplication1.Add(_environmentAdded3);
            Events.Add(_environmentAdded3);
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
            var applicationDetails = ReadModelFacade.GetApplicationDetails(ev.Id);
            applicationDetails.Id.ShouldBe(ev.Id);
            applicationDetails.Name.ShouldBe(ev.Name);
            applicationDetails.Version.ShouldBe(ev.Version);
            applicationDetails.Environments.Count().ShouldBe(0);
            applicationDetails.Created.ShouldBe(ev.TimeStamp);
            applicationDetails.LastModified.ShouldBe(ev.TimeStamp);
        }

        private void ThenTheFirstAndThirdEnvironmentsAreAddedToTheFirstApplication()
        {
            var applicationDetails = ReadModelFacade.GetApplicationDetails(_application1Id);
            applicationDetails.Environments.Count().ShouldBe(2);
            ThenTheEnvironmentIsAdded(_application1Id, _environmentAdded1);
            ThenTheEnvironmentIsAdded(_application1Id, _environmentAdded3);
        }

        private void ThenTheSecondEnvironmentIsAddedToTheSecondApplication()
        {
            var applicationDetails = ReadModelFacade.GetApplicationDetails(_application2Id);
            ThenTheEnvironmentIsAdded(_application2Id, _environmentAdded2);
        }

        private void ThenTheVersionOfTheFirstApplicationHasBeenUpdated()
        {
            ThenTheVersionOfTheApplicationHasBeenUpdated(_application1Id, _environmentAdded3);
        }

        private void ThenTheVersionOfTheSecondApplicationHasBeenUpdated()
        {
            ThenTheVersionOfTheApplicationHasBeenUpdated(_application2Id, _environmentAdded2);
        }

        private void ThenTheLastModifiedTimeOfTheFirstApplicationHasBeenUpdated()
        {
            ThenTheLastModifiedTimeOfTheApplicationHasBeenUpdated(_application1Id, _environmentAdded3);
        }

        private void ThenTheLastModifiedTimeOfTheSecondApplicationHasBeenUpdated()
        {
            ThenTheLastModifiedTimeOfTheApplicationHasBeenUpdated(_application2Id, _environmentAdded2);
        }

        private void ThenTheEnvironmentIsAdded(Guid applicationId, EnvironmentAdded environmentAdded)
        {
            var applicationDetails = ReadModelFacade.GetApplicationDetails(applicationId);
            applicationDetails.Environments.ShouldContain(environment =>
                environment.Id == environmentAdded.EnvironmentId &&
                environment.Name == environmentAdded.Name);
        }

        private void ThenTheVersionOfTheApplicationHasBeenUpdated(Guid applicationId, EnvironmentAdded environmentAdded)
        {
            var applicationDetails = ReadModelFacade.GetApplicationDetails(applicationId);
            applicationDetails.Version.ShouldBe(environmentAdded.Version);
        }

        private void ThenTheLastModifiedTimeOfTheApplicationHasBeenUpdated(Guid applicationId, EnvironmentAdded environmentAdded)
        {
            var applicationDetails = ReadModelFacade.GetApplicationDetails(applicationId);
            applicationDetails.LastModified.ShouldBe(environmentAdded.TimeStamp);
        }
    }
}
