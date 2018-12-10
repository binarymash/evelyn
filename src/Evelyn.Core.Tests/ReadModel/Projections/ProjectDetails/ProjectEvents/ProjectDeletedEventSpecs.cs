namespace Evelyn.Core.Tests.ReadModel.Projections.ProjectDetails.ProjectEvents
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.ProjectDetails;
    using Evelyn.Core.WriteModel.Project.Events;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class ProjectDeletedEventSpecs : ProjectionBuilderHarness<ProjectDeleted>
    {
        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenTheProjectionExists())
                .When(_ => WhenWeHandleAProjectDeletedEvent())
                .Then(_ => ThenTheProjectionIsDeleted())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(StreamVersion, Event, StoppingToken);
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
            ProjectionStore.Received().Delete(Projection.StoreKey(ProjectId));
        }
    }
}
