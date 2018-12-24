namespace Evelyn.Management.Api.Rest.Tests.Read
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.Projections;
    using Evelyn.Management.Api.Rest.Read;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using TestStack.BDDfy;
    using Xunit;

    public class ProjectsControllerSpecs
    {
        private readonly Fixture _fixture;
        private readonly ILogger<ProjectsController> _logger;
        private readonly ProjectsController _controller;
        private readonly IReadModelFacade _readModelFacade;
        private readonly Guid _accountId = Constants.DefaultAccount;
        private Guid _idOfProjectToGet;
        private Core.ReadModel.Projections.AccountProjects.Projection _accountProjectsProjectionReturnedByFacade;
        private Core.ReadModel.Projections.ProjectDetails.Projection _projectDetailsProjectionReturnedByFacade;
        private ObjectResult _result;

        public ProjectsControllerSpecs()
        {
            _fixture = new Fixture();
            _logger = Substitute.For<ILogger<ProjectsController>>();
            _readModelFacade = Substitute.For<IReadModelFacade>();
            _controller = new ProjectsController(_logger, _readModelFacade);
        }

        [Fact]
        public void GetProjectsOnAccount()
        {
            this.Given(_ => GivenThatThereAreProjectsOnAnAccount())
                .When(_ => WhenWeGetTheAccountProjects())
                .Then(_ => ThenStatusCode200IsReturned())
                .And(_ => ThenAllProjectsAreReturned())
                .BDDfy();
        }

        [Fact]
        public void ExceptionWhenGettingProjectOnAccount()
        {
            this.Given(_ => GivenThatAnExceptionIsThrownByHandlerWhenGettingProjects())
                .When(_ => WhenWeGetTheAccountProjects())
                .Then(_ => ThenStatusCode500IsReturned())
                .BDDfy();
        }

        [Fact]
        public void GetProjectDetails()
        {
            this.Given(_ => GivenTheProjectWeWantDoesExist())
                .When(_ => WhenWeGetTheProject())
                .Then(_ => ThenStatusCode200IsReturned())
                .And(_ => ThenTheExpectedProjectIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ProjectDetailsNotFound()
        {
            this.Given(_ => GivenTheProjectWeWantDoesntExist())
                .When(_ => WhenWeGetTheProject())
                .Then(_ => ThenStatusCode404IsReturned())
                .BDDfy();
        }

        [Fact]
        public void ExceptionWhenGettingProjectDetails()
        {
            this.Given(_ => GivenThatAnExceptionIsThrownWhenGettingProject())
                .When(_ => WhenWeGetTheProject())
                .Then(_ => ThenStatusCode500IsReturned())
                .BDDfy();
        }

        private void GivenThatThereAreProjectsOnAnAccount()
        {
            var eventAudit = _fixture.Create<EventAudit>();

            var account = Core.ReadModel.Projections.AccountProjects.Model.Account.Create(
                eventAudit,
                _accountId);

            _accountProjectsProjectionReturnedByFacade = Core.ReadModel.Projections.AccountProjects.Projection.Create(
                eventAudit,
                account);

            _readModelFacade.GetProjects(_accountId).Returns(_accountProjectsProjectionReturnedByFacade);
        }

        private void GivenThatAnExceptionIsThrownByHandlerWhenGettingProjects()
        {
            _readModelFacade
                .GetProjects(_accountId)
                .Throws(_fixture.Create<Exception>());
        }

        private void GivenTheProjectWeWantDoesExist()
        {
            _projectDetailsProjectionReturnedByFacade = _fixture.Create<Core.ReadModel.Projections.ProjectDetails.Projection>();
            _idOfProjectToGet = _projectDetailsProjectionReturnedByFacade.Project.Id;
            _readModelFacade
                .GetProjectDetails(_idOfProjectToGet)
                .Returns(_projectDetailsProjectionReturnedByFacade);
        }

        private void GivenTheProjectWeWantDoesntExist()
        {
            _idOfProjectToGet = _fixture.Create<Guid>();
            _readModelFacade
                .GetProjectDetails(_idOfProjectToGet)
                .Throws(_fixture.Create<ProjectionNotFoundException>());
        }

        private void GivenThatAnExceptionIsThrownWhenGettingProject()
        {
            _idOfProjectToGet = _fixture.Create<Guid>();
            _readModelFacade
                .GetProjectDetails(_idOfProjectToGet)
                .Throws(_fixture.Create<Exception>());
        }

        private async Task WhenWeGetTheAccountProjects()
        {
            _result = await _controller.Get();
        }

        private async Task WhenWeGetTheProject()
        {
            _result = await _controller.Get(_idOfProjectToGet);
        }

        private void ThenStatusCode200IsReturned()
        {
            _result.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        private void ThenStatusCode404IsReturned()
        {
            _result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        private void ThenStatusCode500IsReturned()
        {
            _result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        private void ThenAllProjectsAreReturned()
        {
            var projection = _result.Value as Core.ReadModel.Projections.AccountProjects.Projection;
            projection.Should().Be(_accountProjectsProjectionReturnedByFacade);
        }

        private void ThenTheExpectedProjectIsReturned()
        {
            var returnedProject = _result.Value as Core.ReadModel.Projections.ProjectDetails.Projection;
            returnedProject.Should().Be(_projectDetailsProjectionReturnedByFacade);
        }
    }
}
