﻿namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentDetails.ProjectEvents
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;
    using ProjectEvents = Evelyn.Core.WriteModel.Project.Events;

    public class EnvironmentAddedSpecs : ProjectionHarness<ProjectEvents.EnvironmentAdded>
    {
        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAProjectCreatedEvent())
                .Then(_ => ThenTheProjectionIsCreated())
                .And(_ => ThenTheAuditIsCreated())
                .And(_ => ThenTheEnvironmentAuditIsCreated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(StreamVersion, Event, StoppingToken);
        }

        private async Task WhenWeHandleAProjectCreatedEvent()
        {
            Event = DataFixture.Build<ProjectEvents.EnvironmentAdded>()
               .With(e => e.Id, ProjectId)
               .With(e => e.Key, EnvironmentKey)
               .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionIsCreated()
        {
            UpdatedProjection.ProjectId.Should().Be(Event.Id);

            UpdatedProjection.Key.Should().Be(Event.Key);
            UpdatedProjection.Name.Should().Be(Event.Name);
        }

        private void ThenTheEnvironmentAuditIsCreated()
        {
            UpdatedProjection.EnvironmentAudit.Created.Should().Be(Event.OccurredAt);
            UpdatedProjection.EnvironmentAudit.CreatedBy.Should().Be(Event.UserId);
            UpdatedProjection.EnvironmentAudit.LastModified.Should().Be(Event.OccurredAt);
            UpdatedProjection.EnvironmentAudit.LastModifiedBy.Should().Be(Event.UserId);
            UpdatedProjection.EnvironmentAudit.Version.Should().Be(Event.Version);
        }
    }
}
