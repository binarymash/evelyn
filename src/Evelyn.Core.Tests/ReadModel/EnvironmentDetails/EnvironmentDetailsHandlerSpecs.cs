namespace Evelyn.Core.Tests.ReadModel.ApplicationDetails
{
    using System;
    using AutoFixture;
    using CQRSlite.Routing;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using Evelyn.Core.ReadModel.Events;
    using Evelyn.Core.ReadModel.Infrastructure;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class EnvironmentDetailsHandlerSpecs : HandlerSpecs
    {
        private Guid _environment1Id;

        [Fact]
        public void Environment1DoesNotExist()
        {
            this.Given(_ => GivenThatWeDontCreateEnvironment1())
                .When(_ => WhenWeGetEnvironment1Details())
                .Then(_ => ThenGettingEnvironment1ThrownsNotFoundException())
                .BDDfy();
        }

        protected override void RegisterHandlers(Router router)
        {
            var handler = new EnvironmentDetailsHandler(EnvironmentDetailsStore);
            router.RegisterHandler<EnvironmentAdded>(handler.Handle);
        }

        private void GivenThatWeDontCreateEnvironment1()
        {
            _environment1Id = DataFixture.Create<Guid>();
        }

        private void WhenWeGetEnvironment1Details()
        {
            try
            {
                ReadModelFacade.GetEnvironmentDetails(_environment1Id).GetAwaiter().GetResult().Should().BeNull();
            }
            catch (Exception ex)
            {
                ThrownException = ex;
            }
        }

        private void ThenGettingEnvironment1ThrownsNotFoundException()
        {
            ThrownException.Should().BeOfType<NotFoundException>();
        }

        ////[Fact]
        ////public void ApplicationCreated()
        ////{
        ////    this.Given(_ => GivenAnApplicationIsCreated())
        ////        .When(_ => WhenTheEventsArePublished())
        ////        .And(_ => ThenTheApplicationDetailsCanBeRetrieved())
        ////        .BDDfy();
        ////}

        ////[Fact]
        ////public void MultipleApplicationsCreated()
        ////{
        ////    this.Given(_ => GivenAnApplicationIsCreated())
        ////        .And(_ => GivenAnotherApplicationIsCreated())
        ////        .When(_ => WhenTheEventsArePublished())
        ////        .And(_ => ThenBothApplicationDetailsCanBeRetrieved())
        ////        .BDDfy();
        ////}

        ////[Fact]
        ////public void AddingEnvironmentsToApplication()
        ////{
        ////    this.Given(_ => GivenAnApplicationIsCreated())
        ////        .And(_ => GivenAnotherApplicationIsCreated())
        ////        .And(_ => GivenWeAddAnEnvironmentToTheFirstApplication())
        ////        .And(_ => GivenWeAddAnEnvironmentToTheSecondApplication())
        ////        .And(_ => GivenWeAddAnotherEnvironmentToTheFirstApplication())
        ////        .When(_ => WhenTheEventsArePublished())
        ////        .And(_ => ThenTheFirstAndThirdEnvironmentsAreAddedToTheFirstApplication())
        ////        .And(_ => ThenTheVersionOfTheFirstApplicationHasBeenUpdated())
        ////        .And(_ => ThenTheLastModifiedTimeOfTheFirstApplicationHasBeenUpdated())
        ////        .And(_ => ThenTheSecondEnvironmentIsAddedToTheSecondApplication())
        ////        .And(_ => ThenTheVersionOfTheSecondApplicationHasBeenUpdated())
        ////        .And(_ => ThenTheLastModifiedTimeOfTheSecondApplicationHasBeenUpdated())
        ////        .BDDfy();
        ////}

        ////private void GivenAnApplicationIsCreated()
        ////{
        ////    _event1 = _fixture.Create<ApplicationCreated>();
        ////    _event1.Version = _eventsApplication1.Count + 1;
        ////    _event1.TimeStamp = DateTimeOffset.UtcNow;

        ////    _application1Id = _event1.Id;
        ////    _eventsApplication1.Add(_event1);
        ////    _events.Add(_event1);
        ////}

        ////private void GivenAnotherApplicationIsCreated()
        ////{
        ////    _event2 = _fixture.Create<ApplicationCreated>();
        ////    _event2.Version = _eventsApplication2.Count + 1;
        ////    _event2.TimeStamp = DateTimeOffset.UtcNow;

        ////    _application2Id = _event2.Id;
        ////    _eventsApplication2.Add(_event2);
        ////    _events.Add(_event2);
        ////}

        ////private void GivenWeAddAnEnvironmentToTheFirstApplication()
        ////{
        ////    _environmentAdded1 = _fixture.Create<EnvironmentAdded>();
        ////    _environmentAdded1.Id = _application1Id;
        ////    _environmentAdded1.Version = _eventsApplication1.Count() + 1;
        ////    _environmentAdded1.TimeStamp = DateTimeOffset.UtcNow;

        ////    _eventsApplication1.Add(_environmentAdded1);
        ////    _events.Add(_environmentAdded1);
        ////}

        ////private void GivenWeAddAnEnvironmentToTheSecondApplication()
        ////{
        ////    _environmentAdded2 = _fixture.Create<EnvironmentAdded>();
        ////    _environmentAdded2.Id = _application2Id;
        ////    _environmentAdded2.Version = _eventsApplication2.Count() + 1;
        ////    _environmentAdded2.TimeStamp = DateTimeOffset.UtcNow;

        ////    _eventsApplication2.Add(_environmentAdded2);
        ////    _events.Add(_environmentAdded2);
        ////}

        ////private void GivenWeAddAnotherEnvironmentToTheFirstApplication()
        ////{
        ////    _environmentAdded3 = _fixture.Create<EnvironmentAdded>();
        ////    _environmentAdded3.Id = _application1Id;
        ////    _environmentAdded3.Version = _eventsApplication1.Count() + 1;
        ////    _environmentAdded3.TimeStamp = DateTimeOffset.UtcNow;

        ////    _eventsApplication1.Add(_environmentAdded3);
        ////    _events.Add(_environmentAdded3);
        ////}

        ////private void WhenTheEventsArePublished()
        ////{
        ////    foreach (var ev in _events)
        ////    {
        ////        _publisher.Publish(ev).GetAwaiter().GetResult();
        ////    }
        ////}

        ////private void ThenTheApplicationDetailsCanBeRetrieved()
        ////{
        ////    ThenApplicationDetailsCanBeRetrievedFor(_event1);
        ////}

        ////private void ThenBothApplicationDetailsCanBeRetrieved()
        ////{
        ////    ThenApplicationDetailsCanBeRetrievedFor(_event1);
        ////    ThenApplicationDetailsCanBeRetrievedFor(_event2);
        ////}

        ////private void ThenApplicationDetailsCanBeRetrievedFor(ApplicationCreated ev)
        ////{
        ////    var applicationDetails = _readModelFacade.GetApplicationDetails(ev.Id);
        ////    applicationDetails.Id.Should().Be(ev.Id);
        ////    applicationDetails.Name.Should().Be(ev.Name);
        ////    applicationDetails.Version.Should().Be(ev.Version);
        ////    applicationDetails.Environments.Count().Should().Be(0);
        ////    applicationDetails.Created.Should().Be(ev.TimeStamp);
        ////    applicationDetails.LastModified.Should().Be(ev.TimeStamp);
        ////}

        ////private void ThenTheFirstAndThirdEnvironmentsAreAddedToTheFirstApplication()
        ////{
        ////    var applicationDetails = _readModelFacade.GetApplicationDetails(_application1Id);
        ////    applicationDetails.Environments.Count().Should().Be(2);
        ////    ThenTheEnvironmentIsAdded(_application1Id, _environmentAdded1);
        ////    ThenTheEnvironmentIsAdded(_application1Id, _environmentAdded3);
        ////}

        ////private void ThenTheSecondEnvironmentIsAddedToTheSecondApplication()
        ////{
        ////    var applicationDetails = _readModelFacade.GetApplicationDetails(_application2Id);
        ////    ThenTheEnvironmentIsAdded(_application2Id, _environmentAdded2);
        ////}

        ////private void ThenTheVersionOfTheFirstApplicationHasBeenUpdated()
        ////{
        ////    ThenTheVersionOfTheApplicationHasBeenUpdated(_application1Id, _environmentAdded3);
        ////}

        ////private void ThenTheVersionOfTheSecondApplicationHasBeenUpdated()
        ////{
        ////    ThenTheVersionOfTheApplicationHasBeenUpdated(_application2Id, _environmentAdded2);
        ////}

        ////private void ThenTheLastModifiedTimeOfTheFirstApplicationHasBeenUpdated()
        ////{
        ////    ThenTheLastModifiedTimeOfTheApplicationHasBeenUpdated(_application1Id, _environmentAdded3);
        ////}

        ////private void ThenTheLastModifiedTimeOfTheSecondApplicationHasBeenUpdated()
        ////{
        ////    ThenTheLastModifiedTimeOfTheApplicationHasBeenUpdated(_application2Id, _environmentAdded2);
        ////}

        ////private void ThenTheEnvironmentIsAdded(Guid applicationId, EnvironmentAdded environmentAdded)
        ////{
        ////    var applicationDetails = _readModelFacade.GetApplicationDetails(applicationId);
        ////    applicationDetails.Environments.ShouldContain(environment =>
        ////        environment.Id == environmentAdded.EnvironmentId &&
        ////        environment.Name == environmentAdded.Name);
        ////}

        ////private void ThenTheVersionOfTheApplicationHasBeenUpdated(Guid applicationId, EnvironmentAdded environmentAdded)
        ////{
        ////    var applicationDetails = _readModelFacade.GetApplicationDetails(applicationId);
        ////    applicationDetails.Version.Should().Be(environmentAdded.Version);
        ////}

        ////private void ThenTheLastModifiedTimeOfTheApplicationHasBeenUpdated(Guid applicationId, EnvironmentAdded environmentAdded)
        ////{
        ////    var applicationDetails = _readModelFacade.GetApplicationDetails(applicationId);
        ////    applicationDetails.LastModified.Should().Be(environmentAdded.TimeStamp);
        ////}
    }
}
