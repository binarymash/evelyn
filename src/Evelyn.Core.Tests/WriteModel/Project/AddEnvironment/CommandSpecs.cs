namespace Evelyn.Core.Tests.WriteModel.Project.AddEnvironment
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Core.WriteModel.Project.Commands.AddEnvironment;
    using Core.WriteModel.Project.Domain;
    using Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class CommandSpecs : ProjectCommandHandlerSpecs<Handler, Command>
    {
        private Guid _projectId;

        private string _newEnvironmentName;
        private string _newEnvironmentKey;

        private string _existingEnvironmentName;
        private string _existingEnvironmentKey;

        private string _toggleKey;
        private string _toggleName;
        private int _projectVersion = -1;

        [Fact]
        public void ProjectHasBeenDeleted()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveDeletedTheProject())
                .When(_ => WhenWeAddAnEnvironment())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenAProjectDeletedExceptionIsThrownFor(_projectId))
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        [Fact]
        public void EnvironmentAlreadyExistWithSameKey()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedAnEnvironment())
                .When(_ => WhenWeAddAnotherEnvironmentWithTheSameKey())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenADuplicateEnvironmentKeyExceptionIsThrown())
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        [Fact]
        public void EnvironmentAlreadyExistWithSameName()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedAnEnvironment())
                .When(_ => WhenWeAddAnotherEnvironmentWithTheSameName())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenADuplicateEnvironmentNameExceptionIsThrown())
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        [Fact]
        public void StaleProjectVersion()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedAToggleToTheProject())
                .And(_ => GivenTheProjectVersionForOurNextCommandIsStale())
                .When(_ => WhenWeAddAnEnvironment())
                .Then(_ => ThenNoEventIsPublished())
                .And(_ => ThenAConcurrencyExceptionIsThrown())
                .And(_ => ThenThereAreNoChangesOnTheAggregate())
                .BDDfy();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ProjectWithToggle(int projectVersionOffset)
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedAToggleToTheProject())
                .And(_ => GivenTheProjectVersionForOurNextCommandIsInTheFutureBy(projectVersionOffset))
                .When(_ => WhenWeAddAnEnvironment())

                .Then(_ => ThenTwoEventsArePublished())

                .And(_ => ThenAnEnvironmentAddedEventIsPublished())
                .And(_ => ThenAnEnvironmentStateAddedEventIsPublished())

                .And(_ => ThenTheNumberOfChangesOnTheAggregateIs(6))

                .And(_ => ThenTheAggregateRootHasHadAnEnvironmentAdded())
                .And(_ => ThenTheAggregateRootHasHadAnEnvironmentStateAdded())
                .And(_ => ThenTheAggregateRootLastModifiedTimeHasBeenUpdated())
                .And(_ => ThenTheAggregateRootLastModifiedByHasBeenUpdated())
                .And(_ => ThenTheAggregateRootVersionHasBeenIncreasedBy(2))
                .And(_ => ThenTheAggregateRootLastModifiedVersionIs(NewAggregate.Version - 1))
                .BDDfy();
        }

        protected override Handler BuildHandler()
        {
            return new Handler(Logger, Session);
        }

        private void GivenWeHaveCreatedAProject()
        {
            _projectId = DataFixture.Create<Guid>();
            GivenWeHaveCreatedAProjectWith(_projectId);
            _projectVersion++;
        }

        private void GivenWeHaveDeletedTheProject()
        {
            HistoricalEvents.Add(new ProjectDeleted(UserId, _projectId, DateTime.UtcNow) { Version = HistoricalEvents.Count });
            _projectVersion++;
        }

        private void GivenWeHaveAddedAToggleToTheProject()
        {
            _toggleKey = DataFixture.Create<string>();
            _toggleName = DataFixture.Create<string>();

            HistoricalEvents.Add(new ToggleAdded(UserId, _projectId, _toggleKey, _toggleName, DateTime.UtcNow) { Version = HistoricalEvents.Count });
            _projectVersion++;
        }

        private void GivenWeHaveAddedAnEnvironment()
        {
            _existingEnvironmentKey = DataFixture.Create<string>();
            _existingEnvironmentName = DataFixture.Create<string>();

            HistoricalEvents.Add(new EnvironmentAdded(UserId, _projectId, _existingEnvironmentKey, _existingEnvironmentName, DateTime.UtcNow) { Version = HistoricalEvents.Count });
            _projectVersion++;
        }

        private void GivenTheProjectVersionForOurNextCommandIsStale()
        {
            _projectVersion--;
        }

        private void GivenTheProjectVersionForOurNextCommandIsInTheFutureBy(int projectVersionOffset)
        {
            _projectVersion += projectVersionOffset;
        }

        private void WhenWeAddAnEnvironment()
        {
            _newEnvironmentKey = DataFixture.Create<string>();
            _newEnvironmentName = DataFixture.Create<string>();
            UserId = DataFixture.Create<string>();

            var command = new Command(UserId, _projectId, _newEnvironmentKey, _newEnvironmentName, _projectVersion);
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherEnvironmentWithTheSameKey()
        {
            _newEnvironmentKey = _existingEnvironmentKey;
            _newEnvironmentName = DataFixture.Create<string>();
            UserId = DataFixture.Create<string>();

            var command = new Command(UserId, _projectId, _newEnvironmentKey, _newEnvironmentName, _projectVersion);
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherEnvironmentWithTheSameName()
        {
            _newEnvironmentKey = DataFixture.Create<string>();
            _newEnvironmentName = _existingEnvironmentName;
            UserId = DataFixture.Create<string>();

            var command = new Command(UserId, _projectId, _newEnvironmentKey, _newEnvironmentName, _projectVersion);
            WhenWeHandle(command);
        }

        private void ThenAnEnvironmentAddedEventIsPublished()
        {
            var @event = (EnvironmentAdded)PublishedEvents.First(ev => ev is EnvironmentAdded);
            @event.UserId.Should().Be(UserId);
            @event.Key.Should().Be(_newEnvironmentKey);
            @event.OccurredAt.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
        }

        private void ThenAnEnvironmentStateAddedEventIsPublished()
        {
            var @event = (EnvironmentStateAdded)PublishedEvents.First(ev => ev is EnvironmentStateAdded);
            @event.UserId.Should().Be(UserId);
            @event.EnvironmentKey.Should().Be(_newEnvironmentKey);
            @event.ToggleStates.ToList().Exists(ts => ts.Key == _toggleKey && ts.Value == default(bool).ToString());
            @event.OccurredAt.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
        }

        private void ThenADuplicateEnvironmentKeyExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is already an environment with the key {_newEnvironmentKey}");
        }

        private void ThenADuplicateEnvironmentNameExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is already an environment with the name {_newEnvironmentName}");
        }

        private void ThenTheAggregateRootHasHadAnEnvironmentAdded()
        {
            var environment = NewAggregate.Environments.First(e => e.Key == _newEnvironmentKey);

            environment.Created.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
            environment.CreatedBy.Should().Be(UserId);

            environment.LastModified.Should().Be(environment.Created);
            environment.LastModifiedBy.Should().Be(environment.CreatedBy);
            environment.LastModifiedVersion.Should().Be(OriginalAggregate.Version + 1);
        }

        private void ThenTheAggregateRootHasHadAnEnvironmentStateAdded()
        {
            var environmentState = NewAggregate.EnvironmentStates.First(es => es.EnvironmentKey == _newEnvironmentKey);

            environmentState.Created.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
            environmentState.CreatedBy.Should().Be(UserId);

            environmentState.LastModified.Should().Be(environmentState.Created);
            environmentState.LastModifiedBy.Should().Be(environmentState.CreatedBy);
            environmentState.LastModifiedVersion.Should().Be(NewAggregate.Version);

            environmentState.ToggleStates.Count().Should().Be(OriginalAggregate.Toggles.Count());
            foreach (var toggleState in NewAggregate.Toggles)
            {
                environmentState.ToggleStates.ToList()
                    .Exists(ts => MatchingToggleState(environmentState, ts, toggleState))
                    .Should().BeTrue();
            }
        }

        private bool MatchingToggleState(EnvironmentState environmentState, ToggleState toggleState, Toggle toggle)
        {
            return
                toggleState.Created == environmentState.Created &&
                toggleState.CreatedBy == environmentState.CreatedBy &&
                toggleState.LastModified == toggleState.Created &&
                toggleState.LastModifiedBy == toggleState.CreatedBy &&
                toggleState.Key == toggle.Key &&
                toggleState.Value == toggle.DefaultValue &&
                toggleState.LastModifiedVersion == NewAggregate.Version;
        }
    }
}
