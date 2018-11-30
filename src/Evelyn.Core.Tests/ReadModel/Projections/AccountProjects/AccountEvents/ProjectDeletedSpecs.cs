namespace Evelyn.Core.Tests.ReadModel.Projections.AccountProjects.AccountEvents
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.AccountProjects;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;
    using AccountEvents = Evelyn.Core.WriteModel.Account.Events;

    public class ProjectDeletedSpecs : ProjectionHarness<AccountEvents.ProjectDeleted>
    {
        [Fact]
        public void ProjectionDoesNotExist()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleTheProjectDeletedEvent())
                .Then(_ => ThenAnExceptionIsThrown())
                .BDDfy();
        }

        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenTheProjectionExists())
                .And(_ => GivenOurProjectIsOnTheProjection())
                .And(_ => GivenAnotherProjectIsOnTheProjection())
                .When(_ => WhenWeHandleTheProjectDeletedEvent())
                .Then(_ => ThenOurProjectIsRemoved())
                .And(_ => ThenTheAuditIsUpdated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(Event, StoppingToken);
        }

        private async Task WhenWeHandleTheProjectDeletedEvent()
        {
            Event = DataFixture.Build<AccountEvents.ProjectDeleted>()
                .With(e => e.Id, AccountId)
                .With(e => e.ProjectId, ProjectId)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenOurProjectIsRemoved()
        {
            UpdatedProjection.AccountId.Should().Be(OriginalProjection.AccountId);

            var projects = UpdatedProjection.Projects.ToList();
            projects.Count.Should().Be(OriginalProjection.Projects.Count() - 1);
        }
    }
}
