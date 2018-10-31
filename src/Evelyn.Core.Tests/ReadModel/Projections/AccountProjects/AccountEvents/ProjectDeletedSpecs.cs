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

    public class ProjectDeletedSpecs : EventSpecs
    {
        private AccountEvents.ProjectDeleted _event;

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
                .Then(_ => ThenTheProjectionHasBeenUpdated())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(_event, StoppingToken);
        }

        private async Task WhenWeHandleTheProjectDeletedEvent()
        {
            _event = DataFixture.Build<AccountEvents.ProjectDeleted>()
                .With(e => e.Id, AccountId)
                .With(e => e.ProjectId, ProjectId)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionHasBeenUpdated()
        {
            UpdatedProjection.AccountId.Should().Be(OriginalProjection.AccountId);
            UpdatedProjection.Created.Should().Be(OriginalProjection.Created);
            UpdatedProjection.CreatedBy.Should().Be(OriginalProjection.CreatedBy);

            UpdatedProjection.LastModified.Should().Be(_event.OccurredAt);
            UpdatedProjection.LastModifiedBy.Should().Be(_event.UserId);
            UpdatedProjection.Version.Should().Be(_event.Version);

            var projects = UpdatedProjection.Projects.ToList();
            projects.Count.Should().Be(OriginalProjection.Projects.Count() - 1);
        }
    }
}
