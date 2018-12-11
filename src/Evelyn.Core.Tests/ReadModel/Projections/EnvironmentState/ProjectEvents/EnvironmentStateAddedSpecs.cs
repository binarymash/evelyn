namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentState.ProjectEvents
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.EnvironmentState;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class EnvironmentStateAddedSpecs : ProjectionBuilderHarness<EnvironmentStateAdded>
    {
        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAnEnvironmentStateAddedEvent())
                .Then(_ => ThenTheProjectionAuditIsSet())
                .And(_ => ThenTheProjectionContainsTheEnvironmentState())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(StreamPosition, Event, StoppingToken);
        }

        private async Task WhenWeHandleAnEnvironmentStateAddedEvent()
        {
            Event = DataFixture.Build<EnvironmentStateAdded>()
                .With(ar => ar.Id, ProjectId)
                .With(esa => esa.EnvironmentKey, EnvironmentKey)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionContainsTheEnvironmentState()
        {
            ProjectionStore.Received().Create(Projection.StoreKey(ProjectId, EnvironmentKey), UpdatedProjection);

            UpdatedProjection.EnvironmentState.ToggleStates.Should().BeEquivalentTo(Event.ToggleStates);
        }
    }
}
