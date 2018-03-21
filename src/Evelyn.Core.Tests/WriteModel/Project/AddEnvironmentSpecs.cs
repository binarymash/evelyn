namespace Evelyn.Core.Tests.WriteModel.Project
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Core.WriteModel.Project.Commands;
    using Core.WriteModel.Project.Domain;
    using Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class AddEnvironmentSpecs : ProjectCommandHandlerSpecs<AddEnvironment>
    {
        private Guid _projectId;
        private string _newEnvironmentKey;
        private string _existingEnvironmentKey;
        private string _toggleKey;
        private string _toggleName;

        [Fact]
        public void EnvironmentDoesNotExist()
        {
            this.Given(_ => GivenWeHaveCreatedAProject())
                .And(_ => GivenWeHaveAddedAToggleToTheProject())
                .When(_ => WhenWeAddAnEnvironment())

                .Then(_ => ThenTwoEventsArePublished())

                .And(_ => ThenAnEnvironmentAddedEventIsPublished())
                .And(_ => ThenAnEnvironmentStateAddedEventIsPublished())

                .And(_ => ThenThereAreFiveChangesOnTheAggregate())

                .And(_ => ThenTheAggregateRootHasHadAnEnvironmentAdded())
                .And(_ => ThenTheAggregateRootHasHadAnEnvironmentStateAdded())
                .And(_ => ThenTheAggregateRootLastModifiedTimeHasBeenUpdated())
                .And(_ => ThenTheAggregateRootLastModifiedByHasBeenUpdated())
                .And(_ => ThenTheAggregateRootVersionHasBeenIncreasedByTwo())
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

        private void GivenWeHaveCreatedAProject()
        {
            _projectId = DataFixture.Create<Guid>();
            GivenWeHaveCreatedAProjectWith(_projectId);
        }

        private void GivenWeHaveAddedAToggleToTheProject()
        {
            _toggleKey = DataFixture.Create<string>();
            _toggleName = DataFixture.Create<string>();

            HistoricalEvents.Add(new ToggleAdded(UserId, _projectId, _toggleKey, _toggleName, DateTime.UtcNow) { Version = HistoricalEvents.Count });
        }

        private void GivenWeHaveAddedAnEnvironment()
        {
            _existingEnvironmentKey = DataFixture.Create<string>();

            HistoricalEvents.Add(new EnvironmentAdded(UserId, _projectId, _existingEnvironmentKey, DateTime.UtcNow) { Version = HistoricalEvents.Count });
        }

        private void WhenWeAddAnEnvironment()
        {
            _newEnvironmentKey = DataFixture.Create<string>();
            UserId = DataFixture.Create<string>();

            var command = new AddEnvironment(UserId, _projectId, _newEnvironmentKey) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void WhenWeAddAnotherEnvironmentWithTheSameKey()
        {
            _newEnvironmentKey = _existingEnvironmentKey;
            UserId = DataFixture.Create<string>();

            var command = new AddEnvironment(UserId, _projectId, _newEnvironmentKey) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void ThenAnEnvironmentAddedEventIsPublished()
        {
            var @event = (EnvironmentAdded)PublishedEvents.First(ev => ev is EnvironmentAdded);
            @event.UserId.Should().Be(UserId);
            @event.Key.Should().Be(_newEnvironmentKey);
            @event.OccurredAt.Should().BeCloseTo(DateTimeOffset.UtcNow, 10000);
        }

        private void ThenAnEnvironmentStateAddedEventIsPublished()
        {
            var @event = (EnvironmentStateAdded)PublishedEvents.First(ev => ev is EnvironmentStateAdded);
            @event.UserId.Should().Be(UserId);
            @event.EnvironmentKey.Should().Be(_newEnvironmentKey);
            @event.ToggleStates.ToList().Exists(ts => ts.Key == _toggleKey && ts.Value == default(bool).ToString());
            @event.OccurredAt.Should().BeCloseTo(DateTimeOffset.UtcNow, 10000);
        }

        private void ThenADuplicateEnvironmentKeyExceptionIsThrown()
        {
            ThenAnInvalidOperationExceptionIsThrownWithMessage($"There is already an environment with the key {_newEnvironmentKey}");
        }

        private void ThenTheAggregateRootHasHadAnEnvironmentAdded()
        {
            var environment = NewAggregate.Environments.First(e => e.Key == _newEnvironmentKey);

            environment.Created.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
            environment.CreatedBy.Should().Be(UserId);

            environment.LastModified.Should().Be(environment.Created);
            environment.LastModifiedBy.Should().Be(environment.CreatedBy);
        }

        private void ThenTheAggregateRootHasHadAnEnvironmentStateAdded()
        {
            var environmentState = NewAggregate.EnvironmentStates.First(es => es.EnvironmentKey == _newEnvironmentKey);

            environmentState.Created.Should().BeAfter(TimeBeforeHandling).And.BeBefore(TimeAfterHandling);
            environmentState.CreatedBy.Should().Be(UserId);

            environmentState.LastModified.Should().Be(environmentState.Created);
            environmentState.LastModifiedBy.Should().Be(environmentState.CreatedBy);

            environmentState.Version.Should().Be(0);
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
                toggleState.Version == 0;
        }
    }
}
