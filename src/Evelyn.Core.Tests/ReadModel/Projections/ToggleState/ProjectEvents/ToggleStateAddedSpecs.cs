namespace Evelyn.Core.Tests.ReadModel.Projections.ToggleState.ProjectEvents
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class ToggleStateAddedSpecs : ProjectionBuilderHarness<ToggleStateAdded>
    {
        [Fact]
        public void ProjectionDoesNotExist()
        {
            this.Given(_ => GivenThereAreNoProjections())
                .When(_ => WhenWeHandleAToggleStateAddedEvent())
                .Then(_ => ThenTheProjectionIsCreatedWithTheNewState())
                .Then(_ => ThenTheProjectionAuditIsSet())
                .BDDfy();
        }

        [Fact]
        public void ProjectionAlreadyBuilt()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenTheProjectionStreamVersionIsTheSameAsTheNextEvent())
                .When(_ => WhenWeHandleAToggleStateAddedEvent())
                .Then(_ => ThenTheStoredProjectionIsUnchanged())
                .BDDfy();
        }

        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenTheProjectionHasStateForAnotherEnvironment())
                .When(_ => WhenWeHandleAToggleStateAddedEvent())
                .Then(_ => ThenTheNewStateIsAdded())
                .And(_ => ThenTheProjectionAuditIsSet())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(StreamPosition, Event, StoppingToken);
        }

        private async Task WhenWeHandleAToggleStateAddedEvent()
        {
            Event = DataFixture.Build<ToggleStateAdded>()
                .With(pc => pc.Id, ProjectId)
                .With(pc => pc.EnvironmentKey, EnvironmentKey)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionIsCreatedWithTheNewState()
        {
            var environmentStates = UpdatedProjection.ToggleState.EnvironmentStates.ToList();
            environmentStates.Count.Should().Be(1);

            environmentStates.Should().Contain(ts =>
                ts.Key == Event.EnvironmentKey &&
                ts.Value == Event.Value);
        }

        private void ThenTheNewStateIsAdded()
        {
            var environmentStates = UpdatedProjection.ToggleState.EnvironmentStates.ToList();
            environmentStates.Count.Should().Be(OriginalProjection.ToggleState.EnvironmentStates.Count() + 1);

            foreach (var environmentState in OriginalProjection.ToggleState.EnvironmentStates)
            {
                environmentStates.Should().Contain(ts =>
                    ts.Key == environmentState.Key &&
                    ts.Value == environmentState.Value);
            }

            environmentStates.Should().Contain(ts =>
                ts.Key == Event.EnvironmentKey &&
                ts.Value == Event.Value);
        }
    }
}
