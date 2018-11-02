namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentState.ProjectEvents
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class EnvironmentStateAddedSpecs : EventSpecs
    {
        private EnvironmentStateAdded _event;

        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAnEnvironmentStateAddedEvent())
                .Then(_ => ThenTheProjectionIsCreatedWithTheCorrectProperties())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(_event, StoppingToken);
        }

        private async Task WhenWeHandleAnEnvironmentStateAddedEvent()
        {
            _event = DataFixture.Build<EnvironmentStateAdded>()
                .With(ar => ar.Id, ProjectId)
                .With(esa => esa.EnvironmentKey, EnvironmentKey)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionIsCreatedWithTheCorrectProperties()
        {
            ThenTheProjectionIsCreated();

            UpdatedProjection.Created.Should().Be(_event.OccurredAt);
            UpdatedProjection.CreatedBy.Should().Be(_event.UserId);
            UpdatedProjection.LastModified.Should().Be(_event.OccurredAt);
            UpdatedProjection.LastModifiedBy.Should().Be(_event.UserId);
            UpdatedProjection.Version.Should().Be(_event.Version);
            UpdatedProjection.ToggleStates.Should().BeEquivalentTo(_event.ToggleStates);
        }
    }
}
