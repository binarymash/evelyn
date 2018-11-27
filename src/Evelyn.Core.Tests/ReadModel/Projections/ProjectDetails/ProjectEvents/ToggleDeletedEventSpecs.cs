namespace Evelyn.Core.Tests.ReadModel.Projections.ProjectDetails.ProjectEvents
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.ProjectDetails;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class ToggleDeletedEventSpecs : ProjectionHarness<ToggleDeleted>
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
                .And(_ => GivenThereAreEnvironmentsOnTheProjection())
                .And(_ => GivenThereAreTogglesOnTheProjection())
                .And(_ => GivenOurToggleIsOnTheProjection())
                .When(_ => WhenWeHandleAToggleDeletedEvent())
                .Then(_ => ThenOurToggleIsDeleted())
                .And(_ => ThenTheAuditIsUpdated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(Event, StoppingToken);
        }

        private void GivenOurToggleIsOnTheProjection()
        {
            _toggleKey = DataFixture.Create<string>();

            OriginalProjection.AddToggle(
                _toggleKey,
                DataFixture.Create<string>(),
                DataFixture.Create<DateTimeOffset>(),
                DataFixture.Create<string>(),
                DataFixture.Create<int>());
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
            UpdatedProjection.Environments.Should().BeEquivalentTo(OriginalProjection.Environments);

            var updatedToggles = UpdatedProjection.Toggles.ToList();
            updatedToggles.Count.Should().Be(OriginalProjection.Toggles.Count() - 1);

            foreach (var originalToggle in OriginalProjection.Toggles)
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