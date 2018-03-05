namespace Evelyn.Core.Tests.WriteModel.Project
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Evelyn.Core.ReadModel.Events;
    using Evelyn.Core.WriteModel.Commands;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class CreateProjectSpecs : ProjectCommandHandlerSpecs<CreateProject>
    {
        private string _accountId;
        private Guid _projectId;
        private string _projectName;

        [Fact]
        public void ProjectDoesNotExist()
        {
            this.When(_ => WhenWeCreateAProject())
                .Then(_ => ThenOneEventIsPublished())
                .And(_ => ThenThePublishedEventIsProjectedCreated())
                .And(_ => ThenTheUserIdIsSaved())
                .And(_ => ThenTheAccountIdIsSaved())
                .And(_ => ThenTheNameIsSaved())
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

        private void WhenWeCreateAProject()
        {
            _projectId = DataFixture.Create<Guid>();
            _projectName = DataFixture.Create<string>();
            _accountId = DataFixture.Create<string>();

            var command = new CreateProject(UserId, _accountId, _projectId, _projectName) { ExpectedVersion = HistoricalEvents.Count };
            WhenWeHandle(command);
        }

        private void ThenThePublishedEventIsProjectedCreated()
        {
            PublishedEvents.First().Should().BeOfType<ProjectCreated>();
        }

        private void ThenTheUserIdIsSaved()
        {
            ((ProjectCreated)PublishedEvents.First()).UserId.Should().Be(UserId);
        }

        private void ThenTheAccountIdIsSaved()
        {
            ((ProjectCreated)PublishedEvents.First()).AccountId.Should().Be(_accountId);
        }

        private void ThenTheNameIsSaved()
        {
            ((ProjectCreated)PublishedEvents.First()).Name.Should().Be(_projectName);
        }
    }
}
