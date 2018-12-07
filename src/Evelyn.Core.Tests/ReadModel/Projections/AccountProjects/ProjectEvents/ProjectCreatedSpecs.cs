namespace Evelyn.Core.Tests.ReadModel.Projections.AccountProjects.ProjectEvents
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.AccountProjects;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;
    using ProjectEvents = Evelyn.Core.WriteModel.Project.Events;

    public class ProjectCreatedSpecs : ProjectionHarness<ProjectEvents.ProjectCreated>
    {
        [Fact]
        public void ProjectionDoesNotExist()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAProjectCreatedEvent())
                .Then(_ => ThenAnExceptionIsThrown())
                .And(_ => ThenNoProjectionIsCreated())
                .BDDfy();
        }

        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenOurProjectIsOnTheProjection())
                .And(_ => GivenAnotherProjectIsOnTheProjection())
                .When(_ => WhenWeHandleAProjectCreatedEvent())
                .Then(_ => ThenOurProjectNameIsUpdated())
                .And(_ => ThenTheAuditIsUpdated())
                .And(_ => ThenTheAccountAuditIsNotUpdated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(StreamVersion, Event, StoppingToken);
        }

        private async Task WhenWeHandleAProjectCreatedEvent()
        {
            Event = DataFixture.Build<ProjectEvents.ProjectCreated>()
                .With(e => e.AccountId, AccountId)
                .With(e => e.Id, ProjectId)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenOurProjectNameIsUpdated()
        {
            UpdatedProjection.AccountId.Should().Be(OriginalProjection.AccountId);

            var projects = UpdatedProjection.Projects.ToList();
            projects.Count.Should().Be(OriginalProjection.Projects.Count());

            foreach (var originalProject in OriginalProjection.Projects.Where(p => p.Id != Event.Id))
            {
                projects.Exists(p =>
                    p.Id == originalProject.Id &&
                    p.Name == originalProject.Name).Should().BeTrue();
            }

            projects.Exists(p =>
                p.Id == Event.Id &&
                p.Name == Event.Name).Should().BeTrue();
        }

        private void ThenTheAccountAuditIsNotUpdated()
        {
            UpdatedProjection.AccountAudit.Created.Should().Be(OriginalProjection.AccountAudit.Created);
            UpdatedProjection.AccountAudit.CreatedBy.Should().Be(OriginalProjection.AccountAudit.CreatedBy);
            UpdatedProjection.AccountAudit.LastModified.Should().Be(OriginalProjection.AccountAudit.LastModified);
            UpdatedProjection.AccountAudit.LastModifiedBy.Should().Be(OriginalProjection.AccountAudit.LastModifiedBy);
            UpdatedProjection.AccountAudit.Version.Should().Be(OriginalProjection.AccountAudit.Version);
        }
    }
}
