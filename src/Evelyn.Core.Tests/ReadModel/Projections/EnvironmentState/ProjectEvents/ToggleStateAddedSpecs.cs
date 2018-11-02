namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentState.ProjectEvents
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class ToggleStateAddedSpecs : EventSpecs
    {
        private ToggleStateAdded _event;

        [Fact]
        public void ProjectionDoesNotExist()
        {
            this.Given(_ => _.GivenThereIsNoProjection())
                .When(_ => _.WhenWeHandleAToggleStateAddedEvent())
                .Then(_ => _.ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void Nominal()
        {
            this.Given(_ => _.GivenTheProjectionExists())
                .And(_ => _.GivenTheProjectAlreadyHasAToggleState())
                .When(_ => _.WhenWeHandleAToggleStateAddedEvent())
                .Then(_ => _.ThenTheProjectionHasBeenUpdated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(_event, StoppingToken);
        }

        private void GivenTheProjectAlreadyHasAToggleState()
        {
            OriginalProjection.AddToggleState(
                DataFixture.Create<string>(),
                DataFixture.Create<string>(),
                DataFixture.Create<int>(),
                DataFixture.Create<DateTimeOffset>(),
                DataFixture.Create<string>());
        }

        private async Task WhenWeHandleAToggleStateAddedEvent()
        {
            _event = DataFixture.Build<ToggleStateAdded>()
                .With(pc => pc.Id, ProjectId)
                .With(pc => pc.EnvironmentKey, EnvironmentKey)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionHasBeenUpdated()
        {
            UpdatedProjection.Created.Should().Be(OriginalProjection.Created);
            UpdatedProjection.CreatedBy.Should().Be(OriginalProjection.CreatedBy);

            UpdatedProjection.LastModified.Should().Be(_event.OccurredAt);
            UpdatedProjection.LastModifiedBy.Should().Be(_event.UserId);
            UpdatedProjection.Version.Should().Be(_event.Version);

            var toggleStates = UpdatedProjection.ToggleStates.ToList();
            toggleStates.Count.Should().Be(OriginalProjection.ToggleStates.Count() + 1);

            foreach (var toggleState in OriginalProjection.ToggleStates)
            {
                toggleStates.Should().Contain(ts =>
                    ts.Key == toggleState.Key &&
                    ts.Value == toggleState.Value);
            }

            toggleStates.Should().Contain(ts =>
                ts.Key == _event.ToggleKey &&
                ts.Value == _event.Value);
        }
    }
}
