namespace Evelyn.Core.Tests.ReadModel.Projections.ToggleDetails.ProjectEvents
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.ProjectDetails;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class ToggleAddedEventSpecs : EventSpecs
    {
        private ToggleAdded _event;

        [Fact]
        public void Nominal()
        {
            this.Given(_ => _.GivenThereIsNoProjection())
                .When(_ => _.WhenWeHandleAToggleAddedEvent())
                .Then(_ => _.ThenTheProjectionIsCreatedWithTheCorrectProperties())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(_event, StoppingToken);
        }

        private async Task WhenWeHandleAToggleAddedEvent()
        {
            _event = DataFixture.Build<ToggleAdded>()
                .With(e => e.Id, ProjectId)
                .With(e => e.Key, ToggleKey)
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

            UpdatedProjection.Key.Should().Be(_event.Key);
            UpdatedProjection.Name.Should().Be(_event.Name);
        }
    }
}