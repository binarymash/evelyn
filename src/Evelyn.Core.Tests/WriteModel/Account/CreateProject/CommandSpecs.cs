namespace Evelyn.Core.Tests.WriteModel.Account.CreateProject
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Core.WriteModel.Account.Commands.CreateProject;
    using Core.WriteModel.Project.Domain;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;
    using AccountEvent = Core.WriteModel.Account.Events;
    using ProjectEvent = Core.WriteModel.Project.Events;

    public class CommandSpecs : HandlerSpecs<Command>
    {
        private Guid _accountId;

        private Guid _existingProjectId;
        private Guid _projectId;
        private string _projectName;
        private int _accountVersion = -1;

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

        [Fact]
        public void StaleAccountVersion()
        {
            this.Given(_ => GivenWeHaveRegisteredAnAccount())
                .And(_ => GivenWeHaveAlreadyCreatedAProject())
                .And(_ => GivenTheAccountVersionForOurNextCommandIsStale())
                .When(_ => WhenWeCreateAProjectOnTheAccount())
                .Then(_ => ThenAConcurrencyExceptionIsThrown())
                .And(_ => ThenNoEventIsPublished())
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        [Fact]
        public void FutureAccountVersion()
        {
            this.Given(_ => GivenWeHaveRegisteredAnAccount())
                .And(_ => GivenTheAccountVersionForOurNextCommandIsInTheFuture())
                .When(_ => WhenWeCreateAProjectOnTheAccount())
                .Then(_ => ThenAConcurrencyExceptionIsThrown())
                .And(_ => ThenNoEventIsPublished())
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

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

        protected override Handler BuildHandler()
        {
            return new Handler(Logger, Session);
        }

        private void GivenWeHaveRegisteredAnAccount()
        {
            _accountId = DataFixture.Create<Guid>();

            HistoricalEvents.Add(new AccountEvent.AccountRegistered(UserId, _accountId, DateTimeOffset.UtcNow) { Version = HistoricalEvents.Count });
            _accountVersion++;
        }

        private void GivenWeHaveAlreadyCreatedAProject()
        {
            _existingProjectId = DataFixture.Create<Guid>();

            HistoricalEvents.Add(new AccountEvent.ProjectCreated(UserId, _accountId, _existingProjectId, DateTime.UtcNow) { Version = HistoricalEvents.Count });
            _accountVersion++;
        }

        private void GivenTheAccountVersionForOurNextCommandIsStale()
        {
            _accountVersion--;
        }

        private void GivenTheAccountVersionForOurNextCommandIsInTheFuture()
        {
            _accountVersion++;
        }

        private void WhenWeCreateAProjectOnTheAccount()
        {
            UserId = DataFixture.Create<string>();
            _projectId = DataFixture.Create<Guid>();
            _projectName = DataFixture.Create<string>();

            var command = new Command(UserId, _accountId, _projectId, _projectName, _accountVersion);
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherProjectWithTheSameId()
        {
            UserId = DataFixture.Create<string>();
            _projectId = _existingProjectId;
            _projectName = DataFixture.Create<string>();

            var command = new Command(UserId, _accountId, _existingProjectId, _projectName, _accountVersion);
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
            project.LastModifiedVersion.Should().Be(0);
            project.Created.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
            project.CreatedBy.Should().Be(UserId);
            project.LastModified.Should().Be(project.Created);
            project.LastModifiedBy.Should().Be(project.CreatedBy);
        }
    }
}
