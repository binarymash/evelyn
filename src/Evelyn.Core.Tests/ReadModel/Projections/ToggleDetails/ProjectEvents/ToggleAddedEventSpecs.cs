namespace Evelyn.Core.Tests.ReadModel.Projections.ToggleDetails.ProjectEvents
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.ProjectDetails;
    using Evelyn.Core.ReadModel.Projections.ToggleDetails;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class ToggleAddedEventSpecs : ProjectionBuilderHarness<ToggleAdded>
    {
        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAToggleAddedEvent())
                .Then(_ => ThenTheProjectionAuditIsSet())
                .And(_ => ThenTheProjectionContainsTheToggleDetails())
                .And(_ => ThenTheToggleAuditIsCreated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(StreamVersion, Event, StoppingToken);
        }

        private async Task WhenWeHandleAToggleAddedEvent()
        {
            Event = DataFixture.Build<ToggleAdded>()
                .With(e => e.Id, ProjectId)
                .With(e => e.Key, ToggleKey)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionContainsTheToggleDetails()
        {
            // TODO: move
            ProjectionStore.Received().Create(Core.ReadModel.Projections.ToggleDetails.Projection.StoreKey(ProjectId, ToggleKey), UpdatedProjection);

            UpdatedProjection.Toggle.Key.Should().Be(Event.Key);
            UpdatedProjection.Toggle.Name.Should().Be(Event.Name);
        }

        private void ThenTheToggleAuditIsCreated()
        {
            var audit = UpdatedProjection.Toggle.Audit;
            audit.Created.Should().Be(Event.OccurredAt);
            audit.CreatedBy.Should().Be(Event.UserId);
            audit.LastModified.Should().Be(Event.OccurredAt);
            audit.LastModifiedBy.Should().Be(Event.UserId);
            audit.Version.Should().Be(Event.Version);
        }
    }
}