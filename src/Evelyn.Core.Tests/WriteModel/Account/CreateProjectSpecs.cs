namespace Evelyn.Core.Tests.WriteModel.Account
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Core.WriteModel.Account.Commands;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;
    using AccountEvent = Core.WriteModel.Account.Events;
    using ProjectEvent = Core.WriteModel.Project.Events;

    public class CreateProjectSpecs : AccountCommandHandlerSpecs<CreateProject>
    {
        private Guid _accountId;
        private Guid _projectId;
        private string _projectName;

        [Fact]
        public void ProjectDoesNotExist()
        {
            this.Given(_ => GivenWeHaveRegisteredAnAccount())
                .When(_ => WhenWeCreateAProjectOnTheAccount())
                .Then(_ => ThenTwoEventsArePublished())
                .And(_ => ThenAProjectAddedToAccountEventIsPublished())
                .And(_ => ThenAProjectCreatedEventIsPublishedForAProject())
                .BDDfy();
        }

        ////[Fact]
        ////public void ProjectedAlreadyExists()
        ////{
        ////    this.Given(_ => GivenWeHaveAlreadyCreateAProjected())
        ////        .And(_ => GivenACreateProjectCommand())
        ////        .When(_ => WhenTheCommandIsHandled())
        ////        .Then(_ => ThenNoEventIsPublished())
        ////        .BDDfy();
        ////}

        ////private void GivenWeHaveAlreadyCreatedAnProject()
        ////{
        ////    GivenWeHaveCreatedAProjectWith(_projectId);
        ////}

        private void GivenWeHaveRegisteredAnAccount()
        {
            _accountId = DataFixture.Create<Guid>();

            HistoricalEvents.Add(new AccountEvent.AccountRegistered(UserId, _accountId, DateTimeOffset.UtcNow) { Version = HistoricalEvents.Count });
        }

        private void WhenWeCreateAProjectOnTheAccount()
        {
            _projectId = DataFixture.Create<Guid>();
            _projectName = DataFixture.Create<string>();

            var command = new CreateProject(UserId, _accountId, _projectId, _projectName) { ExpectedVersion = HistoricalEvents.Count - 1 };
            WhenWeHandle(command);
        }

        private void ThenAProjectAddedToAccountEventIsPublished()
        {
            var @event = PublishedEvents.First(e => e.GetType() == typeof(AccountEvent.ProjectCreated)) as AccountEvent.ProjectCreated;
            @event.UserId.Should().Be(UserId);
            @event.ProjectId.Should().Be(_projectId);
        }

        private void ThenAProjectCreatedEventIsPublishedForAProject()
        {
            var @event = PublishedEvents.First(e => e.GetType() == typeof(ProjectEvent.ProjectCreated)) as ProjectEvent.ProjectCreated;
            @event.UserId.Should().Be(UserId);
            @event.AccountId.Should().Be(_accountId);
            @event.Id.Should().Be(_projectId);
            @event.Name.Should().Be(_projectName);
        }
    }
}
