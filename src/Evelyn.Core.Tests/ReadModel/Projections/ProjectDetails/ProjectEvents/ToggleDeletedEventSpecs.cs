namespace Evelyn.Core.Tests.ReadModel.Projections.ProjectDetails.ProjectEvents
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;
    using Projections = Evelyn.Core.ReadModel.Projections;

    public class ToggleDeletedEventSpecs : ProjectionBuilderHarness<ToggleDeleted>
    {
        private string _toggleKey;

        [Fact]
        public void ProjectionDoesNotExist()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAToggleDeletedEvent())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenThereAreEnvironmentsOnTheProject())
                .And(_ => GivenThereAreTogglesOnTheProject())
                .And(_ => GivenOurToggleIsOnTheProjection())
                .When(_ => WhenWeHandleAToggleDeletedEvent())
                .Then(_ => ThenOurToggleIsDeleted())
                .And(_ => ThenTheProjectionAuditIsSet())
                .And(_ => ThenTheProjectAuditIsUpdated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(StreamPosition, Event, StoppingToken);
        }

        private void GivenOurToggleIsOnTheProjection()
        {
            _toggleKey = DataFixture.Create<string>();

            OriginalProjection.Project.AddToggle(
                DataFixture.Create<Projections.EventAudit>(),
                _toggleKey,
                DataFixture.Create<string>());
        }

        private async Task WhenWeHandleAToggleDeletedEvent()
        {
            Event = DataFixture.Build<ToggleDeleted>()
                .With(e => e.Id, ProjectId)
                .With(e => e.Key, _toggleKey)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenOurToggleIsDeleted()
        {
            var originalProject = OriginalProjection.Project;
            var updatedProject = UpdatedProjection.Project;

            updatedProject.Environments.Should().BeEquivalentTo(originalProject.Environments);

            var originalToggles = originalProject.Toggles.ToList();
            var updatedToggles = updatedProject.Toggles.ToList();

            updatedToggles.Count.Should().Be(originalToggles.Count() - 1);

            foreach (var originalToggle in originalToggles)
            {
                if (originalToggle.Key == Event.Key)
                {
                    continue;
                }

                updatedToggles.Should().Contain(t =>
                    t.Key == originalToggle.Key &&
                    t.Name == originalToggle.Name);
            }
        }
    }
}