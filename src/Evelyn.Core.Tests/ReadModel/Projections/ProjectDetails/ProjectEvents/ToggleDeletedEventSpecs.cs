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

    public class ToggleDeletedEventSpecs : EventSpecs
    {
        private ToggleDeleted _event;
        private string _toggleKey;

        [Fact]
        public void ProjectionDoesNotExist()
        {
            this.Given(_ => _.GivenThereIsNoProjection())
                .When(_ => _.WhenWeHandleAToggleDeletedEvent())
                .Then(_ => _.ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void Nominal()
        {
            this.Given(_ => _.GivenTheProjectionExists())
                .And(_ => _.GivenThereAreEnvironmentsOnTheProjection())
                .And(_ => _.GivenThereAreTogglesOnTheProjection())
                .And(_ => _.GivenOurToggleIsOnTheProjection())
                .When(_ => _.WhenWeHandleAToggleDeletedEvent())
                .Then(_ => _.ThenTheProjectionIsUpdated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(_event, StoppingToken);
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
            _event = DataFixture.Build<ToggleDeleted>()
                .With(e => e.Id, ProjectId)
                .With(e => e.Key, _toggleKey)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionIsUpdated()
        {
            UpdatedProjection.Created.Should().Be(OriginalProjection.Created);
            UpdatedProjection.CreatedBy.Should().Be(OriginalProjection.CreatedBy);

            UpdatedProjection.LastModified.Should().Be(_event.OccurredAt);
            UpdatedProjection.LastModifiedBy.Should().Be(_event.UserId);
            UpdatedProjection.Version.Should().Be(_event.Version);

            UpdatedProjection.Environments.Should().BeEquivalentTo(OriginalProjection.Environments);

            var updatedToggles = UpdatedProjection.Toggles.ToList();
            updatedToggles.Count.Should().Be(OriginalProjection.Toggles.Count() - 1);

            foreach (var originalToggle in OriginalProjection.Toggles)
            {
                if (originalToggle.Key == _event.Key)
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