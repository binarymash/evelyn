namespace Evelyn.Core.Tests.WriteModel.Account
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Core.WriteModel.Account.Commands;
    using Core.WriteModel.Project.Domain;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;
    using AccountEvent = Core.WriteModel.Account.Events;
    using ProjectEvent = Core.WriteModel.Project.Events;

    public class CreateProjectSpecs : AccountCommandHandlerSpecs<CreateProject>
    {
        private Guid _accountId;

        private Guid _existingProjectId;
        private Guid _projectId;
        private string _projectName;

        [Fact]
        public void ProjectDoesNotExist()
        {
            this.Given(_ => GivenWeHaveRegisteredAnAccount())
                .When(_ => WhenWeCreateAProjectOnTheAccount())

                .Then(_ => ThenTwoEventsArePublished())

                .And(_ => ThenAProjectAddedToAccountEventIsPublished())
                .And(_ => ThenAProjectCreatedEventIsPublishedForAProject())

                .And(_ => ThenTheNumberOfChangesOnTheAggregateIs(4))

                .And(_ => ThenTheAggregateRootHasHadTheProjectAdded())
                .And(_ => ThenTheAggregateRootLastModifiedTimeHasBeenUpdated())
                .And(_ => ThenTheAggregateRootLastModifiedByHasBeenUpdated())
                .And(_ => ThenTheAggregateRootVersionHasBeenIncreasedBy(1))

                .And(_ => ThenTheProjectIsCreated())

                .BDDfy();
        }

        [Fact]
        public void ProjectedAlreadyExists()
        {
            this.Given(_ => GivenWeHaveRegisteredAnAccount())
                .And(_ => GivenWeHaveAlreadyCreatedAProject())
                .When(_ => WhenWeAddAnotherProjectWithTheSameId())
                .Then(_ => ThenADuplicateProjectExceptionIsThrown())
                .And(_ => ThenNoEventIsPublished())
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        private void GivenWeHaveRegisteredAnAccount()
        {
            _accountId = DataFixture.Create<Guid>();

            HistoricalEvents.Add(new AccountEvent.AccountRegistered(UserId, _accountId, DateTimeOffset.UtcNow) { Version = HistoricalEvents.Count });
        }

        private void GivenWeHaveAlreadyCreatedAProject()
        {
            _existingProjectId = DataFixture.Create<Guid>();

            HistoricalEvents.Add(new AccountEvent.ProjectCreated(UserId, _accountId, _existingProjectId, DateTime.UtcNow) { Version = HistoricalEvents.Count });
        }

        private void WhenWeCreateAProjectOnTheAccount()
        {
            UserId = DataFixture.Create<string>();
            _projectId = DataFixture.Create<Guid>();
            _projectName = DataFixture.Create<string>();

            var command = new CreateProject(UserId, _accountId, _projectId, _projectName) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherProjectWithTheSameId()
        {
            UserId = DataFixture.Create<string>();
            _projectId = _existingProjectId;
            _projectName = DataFixture.Create<string>();

            var command = new CreateProject(UserId, _accountId, _existingProjectId, _projectName) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void ThenADuplicateProjectExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is already a project with the id {_projectId}");
        }

        private void ThenAProjectAddedToAccountEventIsPublished()
        {
            var @event = PublishedEvents.First(e => e.GetType() == typeof(AccountEvent.ProjectCreated)) as AccountEvent.ProjectCreated;
            @event.UserId.Should().Be(UserId);
            @event.ProjectId.Should().Be(_projectId);
        }

        private void ThenAProjectCreatedEventIsPublishedForAProject()
        {
            var @event = PublishedEvents.First(e => e.GetType() == typeof(ProjectEvent.ProjectCreated)) as ProjectEvent.ProjectCreated;
            @event.UserId.Should().Be(UserId);
            @event.AccountId.Should().Be(_accountId);
            @event.Id.Should().Be(_projectId);
            @event.Name.Should().Be(_projectName);
        }

        private void ThenTheAggregateRootHasHadTheProjectAdded()
        {
            NewAggregate.Projects.Should().Contain(_projectId);
        }

        private void ThenTheProjectIsCreated()
        {
            var project = Session.Get<Project>(_projectId).Result;
            project.Name.Should().Be(_projectName);
            project.EnvironmentStates.Count().Should().Be(0);
            project.Environments.Count().Should().Be(0);
            project.Toggles.Count().Should().Be(0);
            project.Version.Should().Be(0);
            project.ScopedVersion.Should().Be(0);
            project.Created.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
            project.CreatedBy.Should().Be(UserId);
            project.LastModified.Should().Be(project.Created);
            project.LastModifiedBy.Should().Be(project.CreatedBy);
        }
    }
}
