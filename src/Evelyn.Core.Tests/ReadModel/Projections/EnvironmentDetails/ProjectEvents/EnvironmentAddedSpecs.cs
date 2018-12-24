namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentDetails.ProjectEvents
{
    using System.Threading.Tasks;
    using AutoFixture;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;
    using ProjectEvents = Evelyn.Core.WriteModel.Project.Events;

    public class EnvironmentAddedSpecs : ProjectionBuilderHarness<ProjectEvents.EnvironmentAdded>
    {
        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAnEnvironmentAddedEvent())
                .Then(_ => ThenTheProjectionAuditIsSet())
                .And(_ => ThenTheProjectionContainsTheEnvironmentDetails())
                .And(_ => ThenTheEnvironmentAuditIsCreated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(StreamPosition, Event, StoppingToken);
        }

        private async Task WhenWeHandleAnEnvironmentAddedEvent()
        {
            Event = DataFixture.Build<ProjectEvents.EnvironmentAdded>()
               .With(e => e.Id, ProjectId)
               .With(e => e.Key, EnvironmentKey)
               .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionContainsTheEnvironmentDetails()
        {
            var environment = UpdatedProjection.Environment;
            environment.ProjectId.Should().Be(Event.Id);
            environment.Key.Should().Be(Event.Key);
            environment.Name.Should().Be(Event.Name);
        }

        private void ThenTheEnvironmentAuditIsCreated()
        {
            var audit = UpdatedProjection.Environment.Audit;
            audit.Created.Should().Be(Event.OccurredAt);
            audit.CreatedBy.Should().Be(Event.UserId);
            audit.LastModified.Should().Be(Event.OccurredAt);
            audit.LastModifiedBy.Should().Be(Event.UserId);
            audit.Version.Should().Be(Event.Version);
        }
    }
}
