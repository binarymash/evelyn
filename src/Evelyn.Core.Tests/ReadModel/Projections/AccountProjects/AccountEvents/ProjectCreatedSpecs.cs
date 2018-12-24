namespace Evelyn.Core.Tests.ReadModel.Projections.AccountProjects.AccountEvents
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.AccountProjects;
    using Evelyn.Core.WriteModel.Account.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class ProjectCreatedSpecs : ProjectionBuilderHarness<ProjectCreated>
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
                .And(_ => GivenOurProjectIsNotOnTheProjection())
                .And(_ => GivenAnotherProjectIsOnTheAccount())
                .When(_ => WhenWeHandleAProjectCreatedEvent())
                .Then(_ => ThenTheProjectionAuditIsSet())
                .And(_ => ThenOurProjectIsAddedToTheAccount())
                .And(_ => ThenTheAccountAuditIsUpdated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(StreamPosition, Event, StoppingToken);
        }

        private async Task WhenWeHandleAProjectCreatedEvent()
        {
            Event = DataFixture.Build<ProjectCreated>()
                .With(pc => pc.Id, AccountId)
                .With(pc => pc.ProjectId, ProjectId)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenOurProjectIsAddedToTheAccount()
        {
            var originalAccount = OriginalProjection.Account;
            var updatedAccount = UpdatedProjection.Account;

            updatedAccount.AccountId.Should().Be(originalAccount.AccountId);

            var originalProjects = originalAccount.Projects.ToList();
            var updatedProjects = updatedAccount.Projects.ToList();

            updatedProjects.Count.Should().Be(originalProjects.Count() + 1);

            foreach (var originalProject in originalProjects)
            {
                updatedProjects.Exists(p =>
                    p.Id == Event.ProjectId &&
                    p.Name == string.Empty).Should().BeTrue();
            }

            updatedProjects.Exists(p =>
                p.Id == Event.ProjectId &&
                p.Name == string.Empty).Should().BeTrue();
        }

        private void ThenTheAccountAuditIsUpdated()
        {
            UpdatedProjection.Account.Audit.Created.Should().Be(OriginalProjection.Account.Audit.Created);
            UpdatedProjection.Account.Audit.CreatedBy.Should().Be(OriginalProjection.Account.Audit.CreatedBy);
            UpdatedProjection.Account.Audit.LastModified.Should().Be(Event.OccurredAt);
            UpdatedProjection.Account.Audit.LastModifiedBy.Should().Be(Event.UserId);
            UpdatedProjection.Account.Audit.Version.Should().Be(Event.Version);
        }
    }
}
