namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentState.ProjectEvents
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class ToggleStateChangedSpecs : EventSpecs
    {
        private ToggleStateChanged _event;

        [Fact]
        public void ProjectionDoesNotExist()
        {
            this.Given(_ => _.GivenThereIsNoProjection())
                .When(_ => _.WhenWeHandleAToggleStateChangedEvent())
                .Then(_ => _.ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void Nominal()
        {
            this.Given(_ => _.GivenTheProjectionExists())
                .And(_ => _.GivenOurToggleStateIsOnTheProjection())
                .And(_ => _.GivenTheProjectionHasOtherToggleStates())
                .When(_ => _.WhenWeHandleAToggleStateChangedEvent())
                .Then(_ => _.ThenTheProjectionHasBeenUpdated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(_event, StoppingToken);
        }

        private async Task WhenWeHandleAToggleStateChangedEvent()
        {
            _event = DataFixture.Build<ToggleStateChanged>()
                .With(pc => pc.Id, ProjectId)
                .With(pc => pc.EnvironmentKey, EnvironmentKey)
                .With(pc => pc.ToggleKey, ToggleKey)
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

            var updatedToggleStates = UpdatedProjection.ToggleStates.ToList();
            updatedToggleStates.Count.Should().Be(OriginalProjection.ToggleStates.Count());

            foreach (var originalToggleState in OriginalProjection.ToggleStates)
            {
                var expectedToggleStateValue =
                    (originalToggleState.Key == _event.ToggleKey)
                    ? _event.Value
                    : originalToggleState.Value;

                updatedToggleStates.Should().Contain(ts =>
                    ts.Key == originalToggleState.Key &&
                    ts.Value == expectedToggleStateValue);
            }
        }
    }
}
