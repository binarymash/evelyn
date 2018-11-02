namespace Evelyn.Core.Tests.ReadModel.Projections.ToggleDetails.ProjectEvents
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.ToggleDetails;
    using Evelyn.Core.WriteModel.Project.Events;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class ToggleDeletedEventSpecs : EventSpecs
    {
        private ToggleDeleted _event;

        [Fact]
        public void Nominal()
        {
            this.Given(_ => _.GivenTheProjectionExists())
                .When(_ => _.WhenWeHandleAToggleDeletedEvent())
                .Then(_ => _.ThenTheProjectionIsDeleted())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(_event, StoppingToken);
        }

        private async Task WhenWeHandleAToggleDeletedEvent()
        {
            _event = DataFixture.Build<ToggleDeleted>()
                .With(e => e.Id, ProjectId)
                .With(e => e.Key, ToggleKey)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionIsDeleted()
        {
            ProjectionStore.Received().Delete(ToggleDetailsDto.StoreKey(ProjectId, ToggleKey));
        }
    }
}