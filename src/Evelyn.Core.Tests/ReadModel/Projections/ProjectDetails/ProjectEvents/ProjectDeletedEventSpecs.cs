namespace Evelyn.Core.Tests.ReadModel.Projections.ProjectDetails.ProjectEvents
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.ProjectDetails;
    using Evelyn.Core.WriteModel.Project.Events;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class ProjectDeletedEventSpecs : EventSpecs<ProjectDeleted>
    {
        [Fact]
        public void Nominal()
        {
            this.Given(_ => _.GivenTheProjectionExists())
                .When(_ => _.WhenWeHandleAProjectDeletedEvent())
                .Then(_ => _.ThenTheProjectionIsDeleted())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(Event, StoppingToken);
        }

        private async Task WhenWeHandleAProjectDeletedEvent()
        {
            Event = DataFixture.Build<ProjectDeleted>()
                .With(e => e.Id, ProjectId)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionIsDeleted()
        {
            ProjectionStore.Received().Delete(ProjectDetailsDto.StoreKey(ProjectId));
        }
    }
}
