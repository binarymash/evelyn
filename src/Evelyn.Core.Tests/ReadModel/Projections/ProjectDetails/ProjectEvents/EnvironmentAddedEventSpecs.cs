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

    public class EnvironmentAddedEventSpecs : ProjectionBuilderHarness<EnvironmentAdded>
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
                .And(_ => GivenThereAreEnvironmentsOnTheProject())
                .And(_ => GivenThereAreTogglesOnTheProject())
                .When(_ => WhenWeHandleAnEnvironmentAddedEvent())
                .Then(_ => ThenTheProjectionAuditIsSet())
                .And(_ => ThenTheEnvironmentIsAddedToTheProject())
                .And(_ => ThenTheProjectAuditIsUpdated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(StreamPosition, Event, StoppingToken);
        }

        private async Task WhenWeHandleAnEnvironmentAddedEvent()
        {
            Event = DataFixture.Build<EnvironmentAdded>()
                .With(ar => ar.Id, ProjectId)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheEnvironmentIsAddedToTheProject()
        {
            var originalProject = OriginalProjection.Project;
            var updatedProject = UpdatedProjection.Project;

            updatedProject.Toggles.Should().BeEquivalentTo(originalProject.Toggles);

            var originalEnvironments = originalProject.Environments.ToList();
            var updatedEnvironments = updatedProject.Environments.ToList();

            updatedEnvironments.Count.Should().Be(originalEnvironments.Count() + 1);

            foreach (var originalEnvironment in originalEnvironments)
            {
                updatedEnvironments.Should().Contain(e =>
                    e.Key == originalEnvironment.Key &&
                    e.Name == originalEnvironment.Name);
            }

            updatedEnvironments.Should().Contain(e =>
                e.Key == Event.Key &&
                e.Name == Event.Name);
        }
    }
}