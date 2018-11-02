namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentState.ProjectEvents
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class ToggleStateDeletedSpecs : EventSpecs
    {
        private ToggleStateDeleted _event;

        [Fact]
        public void Nominal()
        {
            this.Given(_ => _.GivenTheProjectionExists())
                .And(_ => _.GivenOurToggleStateIsOnTheProjection())
                .When(_ => _.WhenWeHandleAToggleStateDeletedEvent())
                .Then(_ => _.ThenTheProjectionIsUpdated())
                .BDDfy();
        }

        [Fact]
        public void ProjectionDoesNotExist()
        {
            this.Given(_ => _.GivenThereIsNoProjection())
                .When(_ => _.WhenWeHandleAToggleStateDeletedEvent())
                .Then(_ => _.ThenAnExceptionIsThrown())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(_event, StoppingToken);
        }

        private async Task WhenWeHandleAToggleStateDeletedEvent()
        {
            _event = DataFixture.Build<ToggleStateDeleted>()
                .With(pc => pc.Id, ProjectId)
                .With(pc => pc.EnvironmentKey, EnvironmentKey)
                .With(pc => pc.ToggleKey, ToggleKey)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionIsUpdated()
        {
            UpdatedProjection.Created.Should().Be(OriginalProjection.Created);
            UpdatedProjection.CreatedBy.Should().Be(OriginalProjection.CreatedBy);

            UpdatedProjection.LastModified.Should().Be(_event.OccurredAt);
            UpdatedProjection.LastModifiedBy.Should().Be(_event.UserId);
            UpdatedProjection.Version.Should().Be(_event.Version);

            var updatedToggleStates = UpdatedProjection.ToggleStates.ToList();
            updatedToggleStates.Count.Should().Be(OriginalProjection.ToggleStates.Count() - 1);

            foreach (var originalToggleState in OriginalProjection.ToggleStates)
            {
                if (originalToggleState.Key != _event.ToggleKey)
                {
                    updatedToggleStates.Should().Contain(ts =>
                        ts.Key == originalToggleState.Key &&
                        ts.Value == originalToggleState.Value);
                }
            }
        }
    }
}
