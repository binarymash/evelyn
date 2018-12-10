namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentState.ProjectEvents
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class ToggleStateAddedSpecs : ProjectionBuilderHarness<ToggleStateAdded>
    {
        [Fact]
        public void ProjectionDoesNotExist()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAToggleStateAddedEvent())
                .Then(_ => ThenAnExceptionIsThrown())
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
            await ProjectionBuilder.Handle(StreamVersion, Event, StoppingToken);
        }

        private void GivenTheProjectAlreadyHasAToggleState()
        {
            OriginalProjection.EnvironmentState.AddToggleState(
                DataFixture.Create<EventAuditDto>(),
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

        private void ThenTheNewToggleStateIsAdded()
        {
            var toggleStates = UpdatedProjection.EnvironmentState.ToggleStates.ToList();
            toggleStates.Count.Should().Be(OriginalProjection.EnvironmentState.ToggleStates.Count() + 1);

            foreach (var toggleState in OriginalProjection.EnvironmentState.ToggleStates)
            {
                toggleStates.Should().Contain(ts =>
                    ts.Key == toggleState.Key &&
                    ts.Value == toggleState.Value);
            }

            toggleStates.Should().Contain(ts =>
                ts.Key == Event.ToggleKey &&
                ts.Value == Event.Value);
        }
    }
}
