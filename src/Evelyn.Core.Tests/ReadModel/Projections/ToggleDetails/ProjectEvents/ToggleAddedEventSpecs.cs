namespace Evelyn.Core.Tests.ReadModel.Projections.ToggleDetails.ProjectEvents
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.ProjectDetails;
    using Evelyn.Core.ReadModel.Projections.ToggleDetails;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class ToggleAddedEventSpecs : ProjectionHarness<ToggleAdded>
    {
        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAToggleAddedEvent())
                .Then(_ => ThenTheProjectionIsCreated())
                .And(_ => ThenTheAuditIsCreated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(Event, StoppingToken);
        }

        private async Task WhenWeHandleAToggleAddedEvent()
        {
            Event = DataFixture.Build<ToggleAdded>()
                .With(e => e.Id, ProjectId)
                .With(e => e.Key, ToggleKey)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionIsCreated()
        {
            ProjectionStore.Received().Create(ToggleDetailsDto.StoreKey(ProjectId, ToggleKey), UpdatedProjection);

            UpdatedProjection.Key.Should().Be(Event.Key);
            UpdatedProjection.Name.Should().Be(Event.Name);
        }
    }
}