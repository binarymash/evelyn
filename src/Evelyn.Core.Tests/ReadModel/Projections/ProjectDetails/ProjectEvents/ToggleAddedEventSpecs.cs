﻿namespace Evelyn.Core.Tests.ReadModel.Projections.ProjectDetails.ProjectEvents
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.ProjectDetails;
    using Evelyn.Core.WriteModel.Project.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class ToggleAddedEventSpecs : ProjectionHarness<ToggleAdded>
    {
        [Fact]
        public void ProjectionDoesNotExist()
        {
            this.Given(_ => _.GivenThereIsNoProjection())
                .When(_ => _.WhenWeHandleAToggleAddedEvent())
                .Then(_ => _.ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void Nominal()
        {
            this.Given(_ => _.GivenTheProjectionExists())
                .And(_ => _.GivenThereAreEnvironmentsOnTheProjection())
                .And(_ => _.GivenThereAreTogglesOnTheProjection())
                .When(_ => _.WhenWeHandleAToggleAddedEvent())
                .Then(_ => _.ThenTheProjectionIsUpdated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(Event, StoppingToken);
        }

        private async Task WhenWeHandleAToggleAddedEvent()
        {
            Event = DataFixture.Build<ToggleAdded>()
                .With(ar => ar.Id, ProjectId)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionIsUpdated()
        {
            UpdatedProjection.Created.Should().Be(OriginalProjection.Created);
            UpdatedProjection.CreatedBy.Should().Be(OriginalProjection.CreatedBy);

            UpdatedProjection.LastModified.Should().Be(Event.OccurredAt);
            UpdatedProjection.LastModifiedBy.Should().Be(Event.UserId);
            UpdatedProjection.Version.Should().Be(Event.Version);

            UpdatedProjection.Environments.Should().BeEquivalentTo(OriginalProjection.Environments);

            var updatedToggles = UpdatedProjection.Toggles.ToList();
            updatedToggles.Count.Should().Be(OriginalProjection.Toggles.Count() + 1);

            foreach (var originalToggle in OriginalProjection.Toggles)
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