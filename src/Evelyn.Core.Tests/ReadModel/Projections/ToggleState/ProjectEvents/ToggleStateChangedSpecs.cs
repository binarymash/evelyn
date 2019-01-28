namespace Evelyn.Core.Tests.ReadModel.Projections.ToggleState.ProjectEvents
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
            this.Given(_ => GivenThereAreNoProjections())
                .When(_ => WhenWeHandleAToggleStateChangedEvent())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenTheProjectionHasStateForOurEnvironment())
                .And(_ => GivenTheProjectionHasStateForAnotherEnvironment())
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
            var updatedEnvironmentStates = UpdatedProjection.ToggleState.EnvironmentStates.ToList();
            updatedEnvironmentStates.Count.Should().Be(OriginalProjection.ToggleState.EnvironmentStates.Count());

            foreach (var originalEnvironmentState in OriginalProjection.ToggleState.EnvironmentStates)
            {
                var expectedEnvironmentStateValue =
                    (originalEnvironmentState.Key == Event.EnvironmentKey)
                    ? Event.Value
                    : originalEnvironmentState.Value;

                updatedEnvironmentStates.Should().Contain(ts =>
                    ts.Key == originalEnvironmentState.Key &&
                    ts.Value == expectedEnvironmentStateValue);
            }
        }
    }
}
