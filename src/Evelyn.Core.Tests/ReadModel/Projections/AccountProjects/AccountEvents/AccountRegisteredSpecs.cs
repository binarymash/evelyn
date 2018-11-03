namespace Evelyn.Core.Tests.ReadModel.Projections.AccountProjects.AccountEvents
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Projections.AccountProjects;
    using Evelyn.Core.WriteModel.Account.Events;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class AccountRegisteredSpecs : EventSpecs<AccountRegistered>
    {
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
            await ProjectionBuilder.Handle(Event, StoppingToken);
        }

        private async Task WhenWeHandleAnAccountRegisteredEvent()
        {
            Event = DataFixture.Build<AccountRegistered>()
                .With(ar => ar.Id, AccountId)
                .Create();

            await WhenTheEventIsHandled();
        }

        private void ThenTheProjectionIsCreatedWithTheCorrectProperties()
        {
            ThenTheProjectionIsCreated();

            UpdatedProjection.AccountId.Should().Be(Event.Id);
            UpdatedProjection.Created.Should().Be(Event.OccurredAt);
            UpdatedProjection.CreatedBy.Should().Be(Event.UserId);
            UpdatedProjection.LastModified.Should().Be(Event.OccurredAt);
            UpdatedProjection.LastModifiedBy.Should().Be(Event.UserId);
            UpdatedProjection.Projects.Should().BeEmpty();
            UpdatedProjection.Version.Should().Be(0);
        }
    }
}
