namespace Evelyn.Core.Tests.ReadModel.ApplicationDetails
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using CQRSlite.Events;
    using CQRSlite.Routing;
    using Evelyn.Core.ReadModel;
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
        private ApplicationDetailsDto _retrievedApplicationDetails;

        public ApplicationDetailsHandlerSpecs()
        {
            _eventsApplication1 = new List<IEvent>();
            _eventsApplication2 = new List<IEvent>();
        }

        [Fact]
        public void ApplicationDoesntExist()
        {
            this.Given(_ => GivenThatWeDontCreateApplication1())
                .When(_ => WhenWeGetTheDetailsForApplication1())
                .Then(_ => ThenANotFoundExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void OneApplicationCreated()
        {
            this.Given(_ => GivenApplication1IsCreated())
                .When(_ => GivenAllEventsArePublished())
                .And(_ => ThenTheApplicationDetailsCanBeRetrieved())
                .BDDfy();
        }

        [Fact]
        public void MultipleApplicationsCreated()
        {
            this.Given(_ => GivenApplication1IsCreated())
                .And(_ => GivenApplication2IsCreated())
                .When(_ => GivenAllEventsArePublished())
                .And(_ => ThenBothApplicationDetailsCanBeRetrieved())
                .BDDfy();
        }

        [Fact]
        public void AddingEnvironmentsToApplication()
        {
            this.Given(_ => GivenApplication1IsCreated())
                .And(_ => GivenApplication2IsCreated())
                .And(_ => GivenWeAddEnvironment1ToApplication1())
                .And(_ => GivenWeAddEnvironment2ToApplication2())
                .And(_ => GivenWeAddEnvironment3ToApplication1())
                .And(_ => GivenAllEventsArePublished())
                .When(_ => WhenWeGetTheDetailsForApplication1())
                .Then(_ => ThenThereAreTwoEnvironmentsOnTheApplication())
                .And(_ => ThenEnvrionment1IsOnTheApplication())
                .And(_ => ThenEnvrionment3IsOnTheApplication())
                .And(_ => ThenTheVersionOfTheApplicationHasBeenUpdatedForEnvironment3())
                .And(_ => ThenTheLastModifiedTimeOfTheApplicationHasBeenUpdatedForEnvironment3())
                .When(_ => WhenWeGetTheDetailsForApplication2())
                .Then(_ => ThenThereIsOneEnvironmentOnTheApplication())
                .And(_ => ThenEnvironment2IsOnTheApplication())
                .And(_ => ThenTheVersionOfTheApplicationHasBeenUpdatedForEnvironment2())
                .And(_ => ThenTheLastModifiedTimeOfTheApplicationHasBeenUpdatedForEnvironment2())
                .BDDfy();
        }

        protected override void RegisterHandlers(Router router)
        {
            var handler = new ApplicationDetailsHandler(ApplicationDetailsStore);
            router.RegisterHandler<ApplicationCreated>(handler.Handle);
            router.RegisterHandler<EnvironmentAdded>(handler.Handle);
        }

        private void GivenThatWeDontCreateApplication1()
        {
            _application1Id = DataFixture.Create<Guid>();
        }

        private void GivenApplication1IsCreated()
        {
            _event1 = DataFixture.Create<ApplicationCreated>();
            _event1.Version = _eventsApplication1.Count + 1;
            _event1.TimeStamp = DateTimeOffset.UtcNow;

            _application1Id = _event1.Id;
            _eventsApplication1.Add(_event1);
            Events.Add(_event1);
        }

        private void GivenApplication2IsCreated()
        {
            _event2 = DataFixture.Create<ApplicationCreated>();
            _event2.Version = _eventsApplication2.Count + 1;
            _event2.TimeStamp = DateTimeOffset.UtcNow;

            _application2Id = _event2.Id;
            _eventsApplication2.Add(_event2);
            Events.Add(_event2);
        }

        private void GivenWeAddEnvironment1ToApplication1()
        {
            _environmentAdded1 = DataFixture.Create<EnvironmentAdded>();
            _environmentAdded1.Id = _application1Id;
            _environmentAdded1.Version = _eventsApplication1.Count() + 1;
            _environmentAdded1.TimeStamp = DateTimeOffset.UtcNow;

            _eventsApplication1.Add(_environmentAdded1);
            Events.Add(_environmentAdded1);
        }

        private void GivenWeAddEnvironment2ToApplication2()
        {
            _environmentAdded2 = DataFixture.Create<EnvironmentAdded>();
            _environmentAdded2.Id = _application2Id;
            _environmentAdded2.Version = _eventsApplication2.Count() + 1;
            _environmentAdded2.TimeStamp = DateTimeOffset.UtcNow;

            _eventsApplication2.Add(_environmentAdded2);
            Events.Add(_environmentAdded2);
        }

        private void GivenWeAddEnvironment3ToApplication1()
        {
            _environmentAdded3 = DataFixture.Create<EnvironmentAdded>();
            _environmentAdded3.Id = _application1Id;
            _environmentAdded3.Version = _eventsApplication1.Count() + 1;
            _environmentAdded3.TimeStamp = DateTimeOffset.UtcNow;

            _eventsApplication1.Add(_environmentAdded3);
            Events.Add(_environmentAdded3);
        }

        private void WhenWeGetTheDetailsForApplication1()
        {
            WhenWeGetTheDetailsFor(_application1Id);
        }

        private void WhenWeGetTheDetailsForApplication2()
        {
            WhenWeGetTheDetailsFor(_application2Id);
        }

        private void WhenWeGetTheDetailsFor(Guid applicationId)
        {
            try
            {
                _retrievedApplicationDetails = ReadModelFacade.GetApplicationDetails(_application1Id);
            }
            catch (Exception ex)
            {
                ThrownException = ex;
            }
        }

        private void ThenANotFoundExceptionIsThrown()
        {
            ThrownException.ShouldBeOfType<NotFoundException>();
        }

        private void ThenThereAreTwoEnvironmentsOnTheApplication()
        {
            _retrievedApplicationDetails.Environments.Count().ShouldBe(2);
        }

        private void ThenThereIsOneEnvironmentOnTheApplication()
        {
            _retrievedApplicationDetails.Environments.Count().ShouldBe(1);
        }

        private void ThenEnvrionment1IsOnTheApplication()
        {
            ThenTheEnvironmentIsOnTheApplication(_environmentAdded1);
        }

        private void ThenEnvironment2IsOnTheApplication()
        {
            ThenTheEnvironmentIsOnTheApplication(_environmentAdded2);
        }

        private void ThenEnvrionment3IsOnTheApplication()
        {
            ThenTheEnvironmentIsOnTheApplication(_environmentAdded3);
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

        private void ThenTheEnvironmentIsOnTheApplication(EnvironmentAdded environmentAdded)
        {
            _retrievedApplicationDetails.Environments.ShouldContain(environment =>
                environment.Id == environmentAdded.EnvironmentId &&
                environment.Name == environmentAdded.Name);
        }

        private void ThenTheVersionOfTheApplicationHasBeenUpdatedForEnvironment3()
        {
            ThenTheVersionOfTheApplicationHasBeenUpdated(_environmentAdded3);
        }

        private void ThenTheVersionOfTheApplicationHasBeenUpdatedForEnvironment2()
        {
            ThenTheVersionOfTheApplicationHasBeenUpdated(_environmentAdded2);
        }

        private void ThenTheVersionOfTheApplicationHasBeenUpdated(EnvironmentAdded environmentAdded)
        {
            _retrievedApplicationDetails.Version.ShouldBe(environmentAdded.Version);
        }

        private void ThenTheLastModifiedTimeOfTheApplicationHasBeenUpdatedForEnvironment3()
        {
            ThenTheLastModifiedTimeOfTheApplicationHasBeenUpdated(_environmentAdded3);
        }

        private void ThenTheLastModifiedTimeOfTheApplicationHasBeenUpdatedForEnvironment2()
        {
            ThenTheLastModifiedTimeOfTheApplicationHasBeenUpdated(_environmentAdded2);
        }

        private void ThenTheLastModifiedTimeOfTheApplicationHasBeenUpdated(EnvironmentAdded environmentAdded)
        {
            _retrievedApplicationDetails.LastModified.ShouldBe(environmentAdded.TimeStamp);
        }
    }
}
