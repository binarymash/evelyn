namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentState.ProjectEvents
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class ToggleStateDeletedSpecs : ProjectionBuilderHarness<ToggleStateDeleted>
    {
        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenOurToggleStateIsOnTheProjection())
                .When(_ => WhenWeHandleAToggleStateDeletedEvent())
                .Then(_ => ThenOurToggleStateIsRemoved())
                .And(_ => ThenTheProjectionAuditIsSet())
                .BDDfy();
        }

        [Fact]
        public void ProjectionDoesNotExist()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAToggleStateDeletedEvent())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(StreamVersion, Event, StoppingToken);
        }

        private async Task WhenWeHandleAToggleStateDeletedEvent()
        {
            Event = DataFixture.Build<ToggleStateDeleted>()
                .With(pc => pc.Id, ProjectId)
                .With(pc => pc.EnvironmentKey, EnvironmentKey)
                .With(pc => pc.ToggleKey, ToggleKey)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenOurToggleStateIsRemoved()
        {
            var updatedToggleStates = UpdatedProjection.EnvironmentState.ToggleStates.ToList();
            updatedToggleStates.Count.Should().Be(OriginalProjection.EnvironmentState.ToggleStates.Count() - 1);

            foreach (var originalToggleState in OriginalProjection.EnvironmentState.ToggleStates)
            {
                if (originalToggleState.Key != Event.ToggleKey)
                {
                    updatedToggleStates.Should().Contain(ts =>
                        ts.Key == originalToggleState.Key &&
                        ts.Value == originalToggleState.Value);
                }
            }
        }
    }
}
