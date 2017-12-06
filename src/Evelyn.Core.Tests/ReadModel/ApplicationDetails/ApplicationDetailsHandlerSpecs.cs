namespace Evelyn.Core.Tests.ReadModel.ApplicationDetails
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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

        private ApplicationCreated _application1Created;
        private ApplicationCreated _application2Created;

        private EnvironmentAdded _environment1Added;
        private EnvironmentAdded _environment2Added;
        private EnvironmentAdded _environment3Added;

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
                .When(_ => WhenWeGetTheDetailsForApplication1())
                .Then(_ => ThenTheDetailsAreSetForApplication1())
                .And(_ => ThenThereAreNoEnvironmentsOnTheApplication())
                .BDDfy();
        }

        [Fact]
        public void MultipleApplicationsCreated()
        {
            this.Given(_ => GivenApplication1IsCreated())
                .And(_ => GivenApplication2IsCreated())
                .When(_ => WhenWeGetTheDetailsForApplication1())
                .Then(_ => ThenTheDetailsAreSetForApplication1())
                .And(_ => ThenThereAreNoEnvironmentsOnTheApplication())
                .When(_ => WhenWeGetTheDetailsForApplication2())
                .Then(_ => ThenTheDetailsAreSetForForApplication2())
                .And(_ => ThenThereAreNoEnvironmentsOnTheApplication())
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
            _application1Created = DataFixture.Create<ApplicationCreated>();
            _application1Created.Version = _eventsApplication1.Count + 1;
            _application1Created.TimeStamp = DateTimeOffset.UtcNow;

            _application1Id = _application1Created.Id;
            _eventsApplication1.Add(_application1Created);

            GivenWePublish(_application1Created);
        }

        private void GivenApplication2IsCreated()
        {
            _application2Created = DataFixture.Create<ApplicationCreated>();
            _application2Created.Version = _eventsApplication2.Count + 1;
            _application2Created.TimeStamp = DateTimeOffset.UtcNow;

            _application2Id = _application2Created.Id;
            _eventsApplication2.Add(_application2Created);

            GivenWePublish(_application2Created);
        }

        private void GivenWeAddEnvironment1ToApplication1()
        {
            _environment1Added = DataFixture.Create<EnvironmentAdded>();
            _environment1Added.Id = _application1Id;
            _environment1Added.Version = _eventsApplication1.Count() + 1;
            _environment1Added.TimeStamp = DateTimeOffset.UtcNow;

            _eventsApplication1.Add(_environment1Added);

            GivenWePublish(_environment1Added);
        }

        private void GivenWeAddEnvironment2ToApplication2()
        {
            _environment2Added = DataFixture.Create<EnvironmentAdded>();
            _environment2Added.Id = _application2Id;
            _environment2Added.Version = _eventsApplication2.Count() + 1;
            _environment2Added.TimeStamp = DateTimeOffset.UtcNow;

            _eventsApplication2.Add(_environment2Added);

            GivenWePublish(_environment2Added);
        }

        private void GivenWeAddEnvironment3ToApplication1()
        {
            _environment3Added = DataFixture.Create<EnvironmentAdded>();
            _environment3Added.Id = _application1Id;
            _environment3Added.Version = _eventsApplication1.Count() + 1;
            _environment3Added.TimeStamp = DateTimeOffset.UtcNow;

            _eventsApplication1.Add(_environment3Added);

            GivenWePublish(_environment3Added);
        }

        private async Task WhenWeGetTheDetailsForApplication1()
        {
            await WhenWeGetTheDetailsFor(_application1Id);
        }

        private async Task WhenWeGetTheDetailsForApplication2()
        {
            await WhenWeGetTheDetailsFor(_application2Id);
        }

        private async Task WhenWeGetTheDetailsFor(Guid applicationId)
        {
            try
            {
                _retrievedApplicationDetails = await ReadModelFacade.GetApplicationDetails(applicationId);
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

        private void ThenThereAreNoEnvironmentsOnTheApplication()
        {
            _retrievedApplicationDetails.Environments.Count().ShouldBe(0);
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
            ThenTheEnvironmentIsOnTheApplication(_environment1Added);
        }

        private void ThenEnvironment2IsOnTheApplication()
        {
            ThenTheEnvironmentIsOnTheApplication(_environment2Added);
        }

        private void ThenEnvrionment3IsOnTheApplication()
        {
            ThenTheEnvironmentIsOnTheApplication(_environment3Added);
        }

        private void ThenTheDetailsAreSetForApplication1()
        {
            ThenTheDetailsAreSetFor(_application1Created);
        }

        private void ThenTheDetailsAreSetForForApplication2()
        {
            ThenTheDetailsAreSetFor(_application2Created);
        }

        private void ThenTheDetailsAreSetFor(ApplicationCreated ev)
        {
            _retrievedApplicationDetails.Id.ShouldBe(ev.Id);
            _retrievedApplicationDetails.Name.ShouldBe(ev.Name);
            _retrievedApplicationDetails.Version.ShouldBe(ev.Version);
            _retrievedApplicationDetails.Created.ShouldBe(ev.TimeStamp);
            _retrievedApplicationDetails.LastModified.ShouldBe(ev.TimeStamp);
        }

        private void ThenTheEnvironmentIsOnTheApplication(EnvironmentAdded environmentAdded)
        {
            _retrievedApplicationDetails.Environments.ShouldContain(environment =>
                environment.Id == environmentAdded.EnvironmentId &&
                environment.Name == environmentAdded.Name);
        }

        private void ThenTheVersionOfTheApplicationHasBeenUpdatedForEnvironment3()
        {
            ThenTheVersionOfTheApplicationHasBeenUpdated(_environment3Added);
        }

        private void ThenTheVersionOfTheApplicationHasBeenUpdatedForEnvironment2()
        {
            ThenTheVersionOfTheApplicationHasBeenUpdated(_environment2Added);
        }

        private void ThenTheVersionOfTheApplicationHasBeenUpdated(EnvironmentAdded environmentAdded)
        {
            _retrievedApplicationDetails.Version.ShouldBe(environmentAdded.Version);
        }

        private void ThenTheLastModifiedTimeOfTheApplicationHasBeenUpdatedForEnvironment3()
        {
            ThenTheLastModifiedTimeOfTheApplicationHasBeenUpdated(_environment3Added);
        }

        private void ThenTheLastModifiedTimeOfTheApplicationHasBeenUpdatedForEnvironment2()
        {
            ThenTheLastModifiedTimeOfTheApplicationHasBeenUpdated(_environment2Added);
        }

        private void ThenTheLastModifiedTimeOfTheApplicationHasBeenUpdated(EnvironmentAdded environmentAdded)
        {
            _retrievedApplicationDetails.LastModified.ShouldBe(environmentAdded.TimeStamp);
        }
    }
}
