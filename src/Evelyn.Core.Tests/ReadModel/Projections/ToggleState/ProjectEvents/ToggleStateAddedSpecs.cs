namespace Evelyn.Core.Tests.ReadModel.Projections.ToggleState.ProjectEvents
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;
    using Projections = Evelyn.Core.ReadModel.Projections;

    public class ToggleStateAddedSpecs : ProjectionBuilderHarness<ToggleStateAdded>
    {
        [Fact]
        public void ProjectionDoesNotExist()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAToggleStateAddedEvent())
                .Then(_ => ThenTheProjectionIsCreatedWithTheNewToggleState())
                .Then(_ => ThenTheProjectionAuditIsSet())
                .BDDfy();
        }

        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenTheProjectAlreadyHasAToggleState())
                .When(_ => WhenWeHandleAToggleStateAddedEvent())
                .Then(_ => ThenTheNewToggleStateIsAdded())
                .And(_ => ThenTheProjectionAuditIsSet())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(StreamPosition, Event, StoppingToken);
        }

        private void GivenTheProjectAlreadyHasAToggleState()
        {
            OriginalProjection.ToggleState.AddEnvironmentState(
                DataFixture.Create<Projections.EventAudit>(),
                DataFixture.Create<string>(),
                DataFixture.Create<string>());
        }

        private async Task WhenWeHandleAToggleStateAddedEvent()
        {
            Event = DataFixture.Build<ToggleStateAdded>()
                .With(pc => pc.Id, ProjectId)
                .With(pc => pc.EnvironmentKey, EnvironmentKey)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionIsCreatedWithTheNewToggleState()
        {
            var environmentStates = UpdatedProjection.ToggleState.EnvironmentStates.ToList();
            environmentStates.Count.Should().Be(1);

            environmentStates.Should().Contain(ts =>
                ts.Key == Event.EnvironmentKey &&
                ts.Value == Event.Value);
        }

        private void ThenTheNewToggleStateIsAdded()
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
