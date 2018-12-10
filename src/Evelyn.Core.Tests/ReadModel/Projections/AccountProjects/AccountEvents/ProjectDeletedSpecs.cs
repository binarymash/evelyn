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

    public class ProjectDeletedSpecs : ProjectionBuilderHarness<AccountEvents.ProjectDeleted>
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
                .And(_ => GivenOurProjectIsOnTheAccount())
                .And(_ => GivenAnotherProjectIsOnTheAccount())
                .When(_ => WhenWeHandleTheProjectDeletedEvent())
                .Then(_ => ThenTheProjectionAuditIsSet())
                .Then(_ => ThenOurProjectIsRemovedFromTheAccount())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(StreamVersion, Event, StoppingToken);
        }

        private async Task WhenWeHandleTheProjectDeletedEvent()
        {
            Event = DataFixture.Build<AccountEvents.ProjectDeleted>()
                .With(e => e.Id, AccountId)
                .With(e => e.ProjectId, ProjectId)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenOurProjectIsRemovedFromTheAccount()
        {
            UpdatedProjection.Account.AccountId.Should().Be(OriginalProjection.Account.AccountId);

            var projects = UpdatedProjection.Account.Projects.ToList();
            projects.Count.Should().Be(OriginalProjection.Account.Projects.Count() - 1);
        }
    }
}
