namespace Evelyn.Core.Tests.ReadModel.Projections.AccountProjects.AccountEvents
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.AccountProjects;
    using Evelyn.Core.WriteModel.Account.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class AccountRegisteredSpecs : EventSpecs
    {
        private AccountRegistered _event;

        [Fact]
        public void Nominal()
        {
            this.Given(_ => GivenThereIsNoProjection())
                .When(_ => WhenWeHandleAnAccountRegisteredEvent())
                .Then(_ => ThenTheProjectionIsCreatedWithTheCorrectProperties())
                .BDDfy();
        }

        protected override async Task HandleEventImplementation()
        {
            await ProjectionBuilder.Handle(_event, StoppingToken);
        }

        private async Task WhenWeHandleAnAccountRegisteredEvent()
        {
            _event = DataFixture.Build<AccountRegistered>()
                .With(ar => ar.Id, AccountId)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionIsCreatedWithTheCorrectProperties()
        {
            ThenTheProjectionIsCreated();

            UpdatedProjection.AccountId.Should().Be(_event.Id);
            UpdatedProjection.Created.Should().Be(_event.OccurredAt);
            UpdatedProjection.CreatedBy.Should().Be(_event.UserId);
            UpdatedProjection.LastModified.Should().Be(_event.OccurredAt);
            UpdatedProjection.LastModifiedBy.Should().Be(_event.UserId);
            UpdatedProjection.Projects.Should().BeEmpty();
            UpdatedProjection.Version.Should().Be(0);
        }
    }
}
