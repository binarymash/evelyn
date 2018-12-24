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

    public class ToggleAddedEventSpecs : ProjectionBuilderHarness<ToggleAdded>
    {
        [Fact]
        public void ProjectionDoesNotExist()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAToggleAddedEvent())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenThereAreEnvironmentsOnTheProject())
                .And(_ => GivenThereAreTogglesOnTheProject())
                .When(_ => WhenWeHandleAToggleAddedEvent())
                .Then(_ => ThenTheToggleIsAdded())
                .And(_ => ThenTheProjectionAuditIsSet())
                .And(_ => ThenTheProjectAuditIsUpdated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(StreamPosition, Event, StoppingToken);
        }

        private async Task WhenWeHandleAToggleAddedEvent()
        {
            Event = DataFixture.Build<ToggleAdded>()
                .With(ar => ar.Id, ProjectId)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheToggleIsAdded()
        {
            var originalProject = OriginalProjection.Project;
            var updatedProject = UpdatedProjection.Project;

            updatedProject.Environments.Should().BeEquivalentTo(originalProject.Environments);

            var originalToggles = originalProject.Toggles.ToList();
            var updatedToggles = updatedProject.Toggles.ToList();

            updatedToggles.Count.Should().Be(originalToggles.Count() + 1);

            foreach (var originalToggle in originalToggles)
            {
                updatedToggles.Should().Contain(t =>
                    t.Key == originalToggle.Key &&
                    t.Name == originalToggle.Name);
            }

            updatedToggles.Should().Contain(t =>
                t.Key == Event.Key &&
                t.Name == Event.Name);
        }
    }
}