namespace Evelyn.Core.Tests.ReadModel.Projections.ProjectDetails.ProjectEvents
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.ProjectDetails;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class EnvironmentAddedEventSpecs : ProjectionHarness<EnvironmentAdded>
    {
        [Fact]
        public void ProjectionDoesNotExist()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAnEnvironmentAddedEvent())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenThereAreEnvironmentsOnTheProjection())
                .And(_ => GivenThereAreTogglesOnTheProjection())
                .When(_ => WhenWeHandleAnEnvironmentAddedEvent())
                .Then(_ => ThenTheEnvironmentIsAdded())
                .And(_ => ThenTheAuditIsUpdated())
                .And(_ => ThenTheProjectAuditIsUpdated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(StreamVersion, Event, StoppingToken);
        }

        private async Task WhenWeHandleAnEnvironmentAddedEvent()
        {
            Event = DataFixture.Build<EnvironmentAdded>()
                .With(ar => ar.Id, ProjectId)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheEnvironmentIsAdded()
        {
            UpdatedProjection.Toggles.Should().BeEquivalentTo(OriginalProjection.Toggles);

            var updatedEnvironments = UpdatedProjection.Environments.ToList();
            updatedEnvironments.Count.Should().Be(OriginalProjection.Environments.Count() + 1);

            foreach (var originalEnvironments in OriginalProjection.Environments)
            {
                updatedEnvironments.Should().Contain(e =>
                    e.Key == originalEnvironments.Key &&
                    e.Name == originalEnvironments.Name);
            }

            updatedEnvironments.Should().Contain(e =>
                e.Key == Event.Key &&
                e.Name == Event.Name);
        }
    }
}