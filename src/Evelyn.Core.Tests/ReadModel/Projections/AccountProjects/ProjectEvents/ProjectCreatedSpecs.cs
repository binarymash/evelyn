namespace Evelyn.Core.Tests.ReadModel.Projections.AccountProjects.ProjectEvents
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.AccountProjects;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;
    using ProjectEvents = Evelyn.Core.WriteModel.Project.Events;

    public class ProjectCreatedSpecs : ProjectionBuilderHarness<ProjectEvents.ProjectCreated>
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
                .And(_ => GivenOurProjectIsOnTheAccount())
                .And(_ => GivenAnotherProjectIsOnTheAccount())
                .When(_ => WhenWeHandleAProjectCreatedEvent())
                .Then(_ => ThenTheProjectionAuditIsSet())
                .Then(_ => ThenTheNameOfTheProjectIsUpdated())
                .And(_ => ThenTheAccountAuditIsNotUpdated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(StreamPosition, Event, StoppingToken);
        }

        private async Task WhenWeHandleAProjectCreatedEvent()
        {
            Event = DataFixture.Build<ProjectEvents.ProjectCreated>()
                .With(e => e.AccountId, AccountId)
                .With(e => e.Id, ProjectId)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheNameOfTheProjectIsUpdated()
        {
            var originalAccount = OriginalProjection.Account;
            var updatedAccount = UpdatedProjection.Account;

            updatedAccount.AccountId.Should().Be(originalAccount.AccountId);

            var originalProjects = originalAccount.Projects.ToList();
            var updatedProjects = updatedAccount.Projects.ToList();

            updatedProjects.Count.Should().Be(originalProjects.Count());

            foreach (var originalProject in originalProjects.Where(p => p.Id != Event.Id))
            {
                updatedProjects.Exists(p =>
                    p.Id == originalProject.Id &&
                    p.Name == originalProject.Name).Should().BeTrue();
            }

            updatedProjects.Exists(p =>
                p.Id == Event.Id &&
                p.Name == Event.Name).Should().BeTrue();
        }

        private void ThenTheAccountAuditIsNotUpdated()
        {
            UpdatedProjection.Account.Audit.Created.Should().Be(OriginalProjection.Account.Audit.Created);
            UpdatedProjection.Account.Audit.CreatedBy.Should().Be(OriginalProjection.Account.Audit.CreatedBy);
            UpdatedProjection.Account.Audit.LastModified.Should().Be(OriginalProjection.Account.Audit.LastModified);
            UpdatedProjection.Account.Audit.LastModifiedBy.Should().Be(OriginalProjection.Account.Audit.LastModifiedBy);
            UpdatedProjection.Account.Audit.Version.Should().Be(OriginalProjection.Account.Audit.Version);
        }
    }
}
