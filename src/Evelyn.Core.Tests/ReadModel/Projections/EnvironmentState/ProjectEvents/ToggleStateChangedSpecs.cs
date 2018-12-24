namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentState.ProjectEvents
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class ToggleStateChangedSpecs : ProjectionBuilderHarness<ToggleStateChanged>
    {
        [Fact]
        public void ProjectionDoesNotExist()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAToggleStateChangedEvent())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenOurToggleStateIsOnTheProjection())
                .And(_ => GivenTheProjectionHasOtherToggleStates())
                .When(_ => WhenWeHandleAToggleStateChangedEvent())
                .Then(_ => ThenOurToggleStateIsChanged())
                .And(_ => ThenTheProjectionAuditIsSet())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(StreamPosition, Event, StoppingToken);
        }

        private async Task WhenWeHandleAToggleStateChangedEvent()
        {
            Event = DataFixture.Build<ToggleStateChanged>()
                .With(pc => pc.Id, ProjectId)
                .With(pc => pc.EnvironmentKey, EnvironmentKey)
                .With(pc => pc.ToggleKey, ToggleKey)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenOurToggleStateIsChanged()
        {
            var updatedToggleStates = UpdatedProjection.EnvironmentState.ToggleStates.ToList();
            updatedToggleStates.Count.Should().Be(OriginalProjection.EnvironmentState.ToggleStates.Count());

            foreach (var originalToggleState in OriginalProjection.EnvironmentState.ToggleStates)
            {
                var expectedToggleStateValue =
                    (originalToggleState.Key == Event.ToggleKey)
                    ? Event.Value
                    : originalToggleState.Value;

                updatedToggleStates.Should().Contain(ts =>
                    ts.Key == originalToggleState.Key &&
                    ts.Value == expectedToggleStateValue);
            }
        }
    }
}
