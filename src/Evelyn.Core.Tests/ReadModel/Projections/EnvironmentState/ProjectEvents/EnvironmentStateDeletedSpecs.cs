namespace Evelyn.Core.Tests.ReadModel.Projections.EnvironmentState.ProjectEvents
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.EnvironmentState;
    using Evelyn.Core.WriteModel.Project.Events;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class EnvironmentStateDeletedSpecs : EventSpecs
    {
        private EnvironmentStateDeleted _event;

        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenTheProjectionExists())
                .When(_ => WhenWeHandleAnEnvironmentStateDeletedEvent())
                .Then(_ => ThenTheProjectionIsDeleted())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(_event, StoppingToken);
        }

        private async Task WhenWeHandleAnEnvironmentStateDeletedEvent()
        {
            _event = DataFixture.Build<EnvironmentStateDeleted>()
                .With(pc => pc.Id, ProjectId)
                .With(pc => pc.EnvironmentKey, EnvironmentKey)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionIsDeleted()
        {
            ProjectionStore.Received().Delete(EnvironmentStateDto.StoreKey(ProjectId, EnvironmentKey));
        }
    }
}
