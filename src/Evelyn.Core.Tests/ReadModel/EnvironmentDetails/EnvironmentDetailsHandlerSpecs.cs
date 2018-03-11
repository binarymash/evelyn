namespace Evelyn.Core.Tests.ReadModel.EnvironmentDetails
{
    using System;
    using AutoFixture;
    using Core.WriteModel.Project.Events;
    using CQRSlite.Routing;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class EnvironmentDetailsHandlerSpecs : HandlerSpecs
    {
        private string _environment1Key;

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
            _environment1Key = DataFixture.Create<string>();
        }

        private void WhenWeGetEnvironment1Details()
        {
            try
            {
                ReadModelFacade.GetEnvironmentDetails(_environment1Key).GetAwaiter().GetResult().Should().BeNull();
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
        ////public void ProjectCreated()
        ////{
        ////    this.Given(_ => GivenAnProjectIsCreated())
        ////        .When(_ => WhenTheEventsArePublished())
        ////        .And(_ => ThenTheProjectDetailsCanBeRetrieved())
        ////        .BDDfy();
        ////}

        ////[Fact]
        ////public void MultipleProjectCreated()
        ////{
        ////    this.Given(_ => GivenAnProjectIsCreated())
        ////        .And(_ => GivenAnotherProjectIsCreated())
        ////        .When(_ => WhenTheEventsArePublished())
        ////        .And(_ => ThenBothProjectDetailsCanBeRetrieved())
        ////        .BDDfy();
        ////}

        ////[Fact]
        ////public void AddingEnvironmentsToProject()
        ////{
        ////    this.Given(_ => GivenAnProjectIsCreated())
        ////        .And(_ => GivenAnotherProjectIsCreated())
        ////        .And(_ => GivenWeAddAnEnvironmentToTheFirstProject())
        ////        .And(_ => GivenWeAddAnEnvironmentToTheSecondProject())
        ////        .And(_ => GivenWeAddAnotherEnvironmentToTheFirstProject())
        ////        .When(_ => WhenTheEventsArePublished())
        ////        .And(_ => ThenTheFirstAndThirdEnvironmentsAreAddedToTheFirstProject())
        ////        .And(_ => ThenTheVersionOfTheFirstProjectHasBeenUpdated())
        ////        .And(_ => ThenTheLastModifiedTimeOfTheFirstProjectHasBeenUpdated())
        ////        .And(_ => ThenTheSecondEnvironmentIsAddedToTheSecondProject())
        ////        .And(_ => ThenTheVersionOfTheSecondProjectHasBeenUpdated())
        ////        .And(_ => ThenTheLastModifiedTimeOfTheSecondProjectHasBeenUpdated())
        ////        .BDDfy();
        ////}

        ////private void GivenAnProjectIsCreated()
        ////{
        ////    _event1 = DataFixture.Create<ProjectCreated>();
        ////    _event1.Version = _eventsProject1.Count + 1;
        ////    _event1.TimeStamp = DateTimeOffset.UtcNow;

        ////    _project1Id = _event1.Id;
        ////    _eventsProject1.Add(_event1);
        ////    _events.Add(_event1);
        ////}

        ////private void GivenAnotherProjectIsCreated()
        ////{
        ////    _event2 = DataFixture.Create<ProjectCreated>();
        ////    _event2.Version = _eventsProject2.Count + 1;
        ////    _event2.TimeStamp = DateTimeOffset.UtcNow;

        ////    _project2Id = _event2.Id;
        ////    _eventsProject2.Add(_event2);
        ////    _events.Add(_event2);
        ////}

        ////private void GivenWeAddAnEnvironmentToTheFirstProject()
        ////{
        ////    _environmentAdded1 = DataFixture.Create<EnvironmentAdded>();
        ////    _environmentAdded1.Id = _project1Id;
        ////    _environmentAdded1.Version = _eventsProject1.Count() + 1;
        ////    _environmentAdded1.TimeStamp = DateTimeOffset.UtcNow;

        ////    _eventsProject1.Add(_environmentAdded1);
        ////    _events.Add(_environmentAdded1);
        ////}

        ////private void GivenWeAddAnEnvironmentToTheSecondProject()
        ////{
        ////    _environmentAdded2 = DataFixture.Create<EnvironmentAdded>();
        ////    _environmentAdded2.Id = _project2Id;
        ////    _environmentAdded2.Version = _eventsProject2.Count() + 1;
        ////    _environmentAdded2.TimeStamp = DateTimeOffset.UtcNow;

        ////    _eventsProject2.Add(_environmentAdded2);
        ////    _events.Add(_environmentAdded2);
        ////}

        ////private void GivenWeAddAnotherEnvironmentToTheFirstProject()
        ////{
        ////    _environmentAdded3 = DataFixture.Create<EnvironmentAdded>();
        ////    _environmentAdded3.Id = _project1Id;
        ////    _environmentAdded3.Version = _eventsProject1.Count() + 1;
        ////    _environmentAdded3.TimeStamp = DateTimeOffset.UtcNow;

        ////    _eventsProject1.Add(_environmentAdded3);
        ////    _events.Add(_environmentAdded3);
        ////}

        ////private void WhenTheEventsArePublished()
        ////{
        ////    foreach (var ev in _events)
        ////    {
        ////        _publisher.Publish(ev).GetAwaiter().GetResult();
        ////    }
        ////}

        ////private void ThenTheProjectDetailsCanBeRetrieved()
        ////{
        ////    ThenProjectDetailsCanBeRetrievedFor(_event1);
        ////}

        ////private void ThenBothProjectDetailsCanBeRetrieved()
        ////{
        ////    ThenProjectDetailsCanBeRetrievedFor(_event1);
        ////    ThenProjectDetailsCanBeRetrievedFor(_event2);
        ////}

        ////private void ThenProjectDetailsCanBeRetrievedFor(ProjectCreated ev)
        ////{
        ////    var projectDetails = _readModelFacade.GetProjectDetails(ev.Id);
        ////    projectDetails.Id.Should().Be(ev.Id);
        ////    projectDetails.Key.Should().Be(ev.Key);
        ////    projectDetails.Version.Should().Be(ev.Version);
        ////    projectDetails.Environments.Count().Should().Be(0);
        ////    projectDetails.Created.Should().Be(ev.TimeStamp);
        ////    projectDetails.LastModified.Should().Be(ev.TimeStamp);
        ////}

        ////private void ThenTheFirstAndThirdEnvironmentsAreAddedToTheFirstProject()
        ////{
        ////    var projectDetails = _readModelFacade.GetProjectDetails(_project1Id);
        ////    projectDetails.Environments.Count().Should().Be(2);
        ////    ThenTheEnvironmentIsAdded(_project1Id, _environmentAdded1);
        ////    ThenTheEnvironmentIsAdded(_project1Id, _environmentAdded3);
        ////}

        ////private void ThenTheSecondEnvironmentIsAddedToTheSecondProject()
        ////{
        ////    var projectDetails = _readModelFacade.GetProjectDetails(_project2Id);
        ////    ThenTheEnvironmentIsAdded(_project2Id, _environmentAdded2);
        ////}

        ////private void ThenTheVersionOfTheFirstProjectHasBeenUpdated()
        ////{
        ////    ThenTheVersionOfTheProjectHasBeenUpdated(_project1Id, _environmentAdded3);
        ////}

        ////private void ThenTheVersionOfTheSecondProjectHasBeenUpdated()
        ////{
        ////    ThenTheVersionOfTheProjectHasBeenUpdated(_project2Id, _environmentAdded2);
        ////}

        ////private void ThenTheLastModifiedTimeOfTheFirstProjectHasBeenUpdated()
        ////{
        ////    ThenTheLastModifiedTimeOfTheProjectHasBeenUpdated(_project1Id, _environmentAdded3);
        ////}

        ////private void ThenTheLastModifiedTimeOfTheSecondProjectHasBeenUpdated()
        ////{
        ////    ThenTheLastModifiedTimeOfTheProjectHasBeenUpdated(_project2Id, _environmentAdded2);
        ////}

        ////private void ThenTheEnvironmentIsAdded(Guid ProjectId, EnvironmentAdded environmentAdded)
        ////{
        ////    var projectDetails = _readModelFacade.GetProjectDetails(projectId);
        ////    projectDetails.Environments.ShouldContain(environment =>
        ////        environment.Key == environmentAdded.Key);
        ////}

        ////private void ThenTheVersionOfTheProjectHasBeenUpdated(Guid ProjectId, EnvironmentAdded environmentAdded)
        ////{
        ////    var projectDetails = _readModelFacade.GetProjectDetails(projectId);
        ////    projectDetails.Version.Should().Be(environmentAdded.Version);
        ////}

        ////private void ThenTheLastModifiedTimeOfTheProjectHasBeenUpdated(Guid ProjectId, EnvironmentAdded environmentAdded)
        ////{
        ////    var projectDetails = _readModelFacade.GetProjectDetails(projectId);
        ////    projectDetails.LastModified.Should().Be(environmentAdded.TimeStamp);
        ////}
    }
}
