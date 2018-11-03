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

    public class EnvironmentStateAddedSpecs : EventSpecs<EnvironmentStateAdded>
    {
        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAnEnvironmentStateAddedEvent())
                .Then(_ => ThenTheProjectionIsCreated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(Event, StoppingToken);
        }

        private async Task WhenWeHandleAnEnvironmentStateAddedEvent()
        {
            Event = DataFixture.Build<EnvironmentStateAdded>()
                .With(ar => ar.Id, ProjectId)
                .With(esa => esa.EnvironmentKey, EnvironmentKey)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionIsCreated()
        {
            ProjectionStore.Received().Create(EnvironmentStateDto.StoreKey(ProjectId, EnvironmentKey), UpdatedProjection);

            UpdatedProjection.Created.Should().Be(Event.OccurredAt);
            UpdatedProjection.CreatedBy.Should().Be(Event.UserId);
            UpdatedProjection.LastModified.Should().Be(Event.OccurredAt);
            UpdatedProjection.LastModifiedBy.Should().Be(Event.UserId);
            UpdatedProjection.Version.Should().Be(Event.Version);
            UpdatedProjection.ToggleStates.Should().BeEquivalentTo(Event.ToggleStates);
        }
    }
}
