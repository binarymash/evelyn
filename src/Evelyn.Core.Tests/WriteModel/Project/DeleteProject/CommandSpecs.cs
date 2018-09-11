namespace Evelyn.Core.Tests.WriteModel.Project.DeleteProject
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using FluentAssertions;
    using global::Evelyn.Core.WriteModel.Project.Commands.DeleteProject;
    using global::Evelyn.Core.WriteModel.Project.Events;
    using TestStack.BDDfy;
    using Xunit;

    public class CommandSpecs : ProjectCommandHandlerSpecs<Handler, Command>
    {
        private Guid _accountId;
        private int _accountEventCount;

        private Guid _projectId;
        private int _projectEventCount;
        private int _projectLastModifiedVersion;

        private string _environment1Key;
        private string _environment2Key;

        private string _toggle1Key;
        private string _toggle2Key;

        public CommandSpecs()
        {
            IgnoreCollectionOrderDuringComparison = true;
            _accountEventCount = 0;
            _projectEventCount = 0;
            _projectLastModifiedVersion = -1;
        }

        [Fact]
        public void AlreadyDeleted()
        {
            this.Given(_ => GivenWeHaveRegisteredAnAccount())
                .And(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveDeletedTheProject())
                .When(_ => WhenWeDeleteTheProject())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenAProjectDeletedExceptionIsThrownFor(_projectId))
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ProjectExistsWithTogglesAndEnvironments(int projectVersionOffset)
        {
            this.Given(_ => GivenWeHaveRegisteredAnAccount())
                .And(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedAToggle())
                .And(_ => GivenWeHaveAddedAnotherToggle())
                .And(_ => GivenWeHaveAddedTwoEnvironments())
                .And(_ => GivenTheProjectVersionForOurNextCommandIsInTheFutureBy(projectVersionOffset))

                .When(_ => WhenWeDeleteTheProject())

                .Then(_ => ThenEightEventsArePublished())

                .And(_ => ThenAProjectDeletedEventIsPublished())
                .And(_ => ThenAnEnvironmentDeletedEventIsPublishedForEnvironment1())
                .And(_ => ThenAnEnvironmentStateDeletedEventIsPublishedForEnvironment1())
                .And(_ => ThenAnEnvironmentDeletedEventIsPublishedForEnvironment2())
                .And(_ => ThenAnEnvironmentStateDeletedEventIsPublishedForEnvironment2())
                .And(_ => ThenAToggleDeletedEventIsPublishedForToggle1())
                .And(_ => ThenAToggleDeletedEventIsPublishedForToggle2())

                .And(_ => ThenTheNumberOfChangesOnTheAggregateIs(14))

                .And(_ => ThenTheAggregateRootIsMarkedAsDeleted())

                .And(_ => ThenTheAggregateRootHasHadToggle1Removed())
                .And(_ => ThenTheAggregateRootHasHadToggle2Removed())
                .And(_ => ThenTheAggregateRootHasNoToggles())

                .And(_ => ThenTheAggregateRootHasHadEnvironment1Removed())
                .And(_ => ThenTheAggregateRootHasHadEnvironment2Removed())
                .And(_ => ThenTheAggregateRootHasNoEnvironments())

                .And(_ => ThenTheAggregateRootHasHadTheStatesForEnvironment1Removed())
                .And(_ => ThenTheAggregateRootHasHadTheStatesForEnvironment2Removed())
                .And(_ => ThenTheAggregateRootHasNoEnvironmentStates())

                .And(_ => ThenTheAggregateRootLastModifiedVersionIs(NewAggregate.Version - 2))
                .And(_ => ThenTheAggregateRootLastModifiedTimeHasBeenUpdated())
                .And(_ => ThenTheAggregateRootLastModifiedByHasBeenUpdated())
                .And(_ => ThenTheAggregateRootVersionHasBeenIncreasedBy(7))

                .BDDfy();
        }

        protected override Handler BuildHandler()
        {
            return new Handler(Logger, Session);
        }

        private void GivenWeHaveRegisteredAnAccount()
        {
            _accountId = DataFixture.Create<Guid>();

            HistoricalEvents.Add(new Core.WriteModel.Account.Events.AccountRegistered(UserId, _accountId, DateTimeOffset.UtcNow) { Version = _accountEventCount });
            _accountEventCount++;
        }

        private void GivenWeHaveCreatedAProject()
        {
            _projectId = DataFixture.Create<Guid>();

            HistoricalEvents.Add(new Core.WriteModel.Account.Events.ProjectCreated(UserId, _accountId, _projectId, DateTimeOffset.UtcNow) { Version = _accountEventCount });
            _accountEventCount++;

            GivenWeHaveCreatedAProjectWith(_projectId, _projectEventCount);
            _projectEventCount++;
            _projectLastModifiedVersion = _projectEventCount - 1;
        }

        private void GivenWeHaveDeletedTheProject()
        {
            HistoricalEvents.Add(new ProjectDeleted(UserId, _projectId, DateTimeOffset.UtcNow) { Version = _projectEventCount });
            _projectEventCount++;
            _projectLastModifiedVersion = _projectEventCount - 1;
        }

        private void GivenWeHaveAddedTwoEnvironments()
        {
            _environment1Key = DataFixture.Create<string>();
            _environment2Key = DataFixture.Create<string>();

            GivenWeHaveAddedAnEnvironmentWith(_projectId, _environment1Key, _projectEventCount);
            _projectEventCount++;
            _projectLastModifiedVersion = _projectEventCount - 1;

            GivenWeHaveAddedAnEnvironmentStateWith(
                _projectId,
                _environment1Key,
                _projectEventCount,
                new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(_toggle1Key, default), new KeyValuePair<string, string>(_toggle2Key, default) });
            _projectEventCount++;

            GivenWeHaveAddedAnEnvironmentWith(_projectId, _environment2Key, _projectEventCount);
            _projectEventCount++;
            _projectLastModifiedVersion = _projectEventCount - 1;

            GivenWeHaveAddedAnEnvironmentStateWith(
                _projectId,
                _environment2Key,
                _projectEventCount,
                new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(_toggle1Key, default), new KeyValuePair<string, string>(_toggle2Key, default) });
            _projectEventCount++;
        }

        private void GivenWeHaveAddedAToggle()
        {
            _toggle1Key = DataFixture.Create<string>();
            HistoricalEvents.Add(new ToggleAdded(UserId, _projectId, _toggle1Key, DataFixture.Create<string>(), DateTimeOffset.UtcNow) { Version = _projectEventCount });
            _projectEventCount++;
            _projectLastModifiedVersion = _projectEventCount - 1;
        }

        private void GivenWeHaveAddedAnotherToggle()
        {
            _toggle2Key = DataFixture.Create<string>();
            HistoricalEvents.Add(new ToggleAdded(UserId, _projectId, _toggle2Key, DataFixture.Create<string>(), DateTimeOffset.UtcNow) { Version = _projectEventCount });
            _projectEventCount++;
            _projectLastModifiedVersion = _projectEventCount - 1;
        }

        private void GivenTheProjectVersionForOurNextCommandIsInTheFutureBy(int projectVersionOffset)
        {
            _projectLastModifiedVersion += projectVersionOffset;
        }

        private void WhenWeDeleteTheProject()
        {
            UserId = DataFixture.Create<string>();
            var command = new Command(UserId, _accountId, _projectId, _projectLastModifiedVersion);
            WhenWeHandle(command);
        }

        private void ThenAProjectDeletedEventIsPublished()
        {
            var ev = (ProjectDeleted)PublishedEvents.First(e => e.GetType() == typeof(ProjectDeleted));

            ev.UserId.Should().Be(UserId);
            ev.Id.Should().Be(_projectId);
            ev.OccurredAt.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
        }

        private void ThenAnEnvironmentDeletedEventIsPublishedForEnvironment1()
        {
            ThenAnEnvironmentDeletedEventIsPublishedForEnvironment(_environment1Key);
        }

        private void ThenAnEnvironmentDeletedEventIsPublishedForEnvironment2()
        {
            ThenAnEnvironmentDeletedEventIsPublishedForEnvironment(_environment2Key);
        }

        private void ThenAnEnvironmentDeletedEventIsPublishedForEnvironment(string environmentKey)
        {
            var ev = (EnvironmentDeleted)PublishedEvents.First(e =>
                e.GetType() == typeof(EnvironmentDeleted) &&
                ((EnvironmentDeleted)e).Key == environmentKey);

            ev.UserId.Should().Be(UserId);
            ev.Key.Should().Be(environmentKey);
            ev.OccurredAt.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
        }

        private void ThenAnEnvironmentStateDeletedEventIsPublishedForEnvironment1()
        {
            ThenAnEnvironmentStateDeletedEventsIsPublishedForEnvironment(_environment1Key);
        }

        private void ThenAnEnvironmentStateDeletedEventIsPublishedForEnvironment2()
        {
            ThenAnEnvironmentStateDeletedEventsIsPublishedForEnvironment(_environment2Key);
        }

        private void ThenAnEnvironmentStateDeletedEventsIsPublishedForEnvironment(string environmentKey)
        {
            var ev = (EnvironmentStateDeleted)PublishedEvents.First(e =>
                e.GetType() == typeof(EnvironmentStateDeleted) &&
                ((EnvironmentStateDeleted)e).EnvironmentKey == environmentKey);

            ev.OccurredAt.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
            ev.UserId.Should().Be(UserId);
        }

        private void ThenAToggleDeletedEventIsPublishedForToggle1()
        {
            ThenAToggleDeletedEventIsPublishedFor(_toggle1Key);
        }

        private void ThenAToggleDeletedEventIsPublishedForToggle2()
        {
            ThenAToggleDeletedEventIsPublishedFor(_toggle2Key);
        }

        private void ThenAToggleDeletedEventIsPublishedFor(string toggleKey)
        {
            var ev = (ToggleDeleted)PublishedEvents.First(e =>
                e.GetType() == typeof(ToggleDeleted) &&
                ((ToggleDeleted)e).Key == toggleKey);

            ev.UserId.Should().Be(UserId);
            ev.OccurredAt.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
        }

        private void ThenTheAggregateRootIsMarkedAsDeleted()
        {
            NewAggregate.IsDeleted.Should().BeTrue();
        }

        private void ThenTheAggregateRootHasHadToggle1Removed()
        {
            NewAggregate.Toggles.Any(t => t.Key == _toggle1Key).Should().BeFalse();
        }

        private void ThenTheAggregateRootHasHadToggle2Removed()
        {
            NewAggregate.Toggles.Any(t => t.Key == _toggle2Key).Should().BeFalse();
        }

        private void ThenTheAggregateRootHasNoToggles()
        {
            NewAggregate.Toggles.Count().Should().Be(0);
        }

        private void ThenTheAggregateRootHasHadEnvironment1Removed()
        {
            NewAggregate.Environments.Any(t => t.Key == _environment1Key).Should().BeFalse();
        }

        private void ThenTheAggregateRootHasHadEnvironment2Removed()
        {
            NewAggregate.Environments.Any(t => t.Key == _environment2Key).Should().BeFalse();
        }

        private void ThenTheAggregateRootHasNoEnvironments()
        {
            NewAggregate.Environments.Count().Should().Be(0);
        }

        private void ThenTheAggregateRootHasHadTheStatesForEnvironment1Removed()
        {
            NewAggregate.EnvironmentStates.Any(t => t.EnvironmentKey == _environment1Key).Should().BeFalse();
        }

        private void ThenTheAggregateRootHasHadTheStatesForEnvironment2Removed()
        {
            NewAggregate.EnvironmentStates.Any(t => t.EnvironmentKey == _environment2Key).Should().BeFalse();
        }

        private void ThenTheAggregateRootHasNoEnvironmentStates()
        {
            NewAggregate.EnvironmentStates.Count().Should().Be(0);
        }
    }
}
