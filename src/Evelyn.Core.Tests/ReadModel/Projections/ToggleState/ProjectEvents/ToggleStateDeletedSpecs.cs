namespace Evelyn.Core.Tests.ReadModel.Projections.ToggleState.ProjectEvents
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.ToggleState;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class ToggleStateDeletedSpecs : ProjectionBuilderHarness<ToggleStateDeleted>
    {
        [Fact]
        public void ProjectionDoesNotExist()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAToggleStateDeletedEvent())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void MultipleToggleStatesExist()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenOurToggleStateIsOnTheProjection())
                .And(_ => GivenTheProjectionHasOtherToggleStates())
                .When(_ => WhenWeHandleAToggleStateDeletedEvent())
                .Then(_ => ThenOurToggleStateIsRemoved())
                .And(_ => ThenTheProjectionAuditIsSet())
                .BDDfy();
        }

        [Fact]
        public void LastToggleStateIsDeleted()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenOurToggleStateIsOnTheProjection())
                .When(_ => WhenWeHandleAToggleStateDeletedEvent())
                .Then(_ => ThenTheProjectionIsDeleted())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(StreamPosition, Event, StoppingToken);
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
            var updatedEnvironmentStates = UpdatedProjection.ToggleState.EnvironmentStates.ToList();
            updatedEnvironmentStates.Count.Should().Be(OriginalProjection.ToggleState.EnvironmentStates.Count() - 1);

            foreach (var originalEnvironmentState in OriginalProjection.ToggleState.EnvironmentStates)
            {
                if (originalEnvironmentState.Key != Event.EnvironmentKey)
                {
                    updatedEnvironmentStates.Should().Contain(ts =>
                        ts.Key == originalEnvironmentState.Key &&
                        ts.Value == originalEnvironmentState.Value);
                }
            }
        }

        private void ThenTheProjectionIsDeleted()
        {
            ProjectionStore.Received().Delete(Projection.StoreKey(ProjectId, EnvironmentKey));
        }
    }
}
