namespace Evelyn.Management.Api.Rest.Tests.Read
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core;
    using Core.ReadModel.AccountProjects;
    using Core.ReadModel.ProjectDetails;
    using Evelyn.Core.ReadModel;
    using Evelyn.Management.Api.Rest.Read;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Rest.Write;
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
        private AccountProjectsDto _accountProjectsReturnedByFacade;
        private ProjectDetailsDto _projectReturnedByFacade;
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
            _accountProjectsReturnedByFacade = new AccountProjectsDto(_accountId, _fixture.Create<int>(), DateTimeOffset.UtcNow, _fixture.Create<string>(), DateTimeOffset.UtcNow, _fixture.Create<string>(), _fixture.CreateMany<ProjectListDto>());

            _readModelFacade.GetProjects(_accountId).Returns(_accountProjectsReturnedByFacade);
        }

        private void GivenThatAnExceptionIsThrownByHandlerWhenGettingProjects()
        {
            _readModelFacade
                .GetProjects(_accountId)
                .Throws(_fixture.Create<Exception>());
        }

        private void GivenTheProjectWeWantDoesExist()
        {
            _projectReturnedByFacade = _fixture.Create<ProjectDetailsDto>();
            _idOfProjectToGet = _projectReturnedByFacade.Id;
            _readModelFacade
                .GetProjectDetails(_idOfProjectToGet)
                .Returns(_projectReturnedByFacade);
        }

        private void GivenTheProjectWeWantDoesntExist()
        {
            _idOfProjectToGet = _fixture.Create<Guid>();
            _readModelFacade
                .GetProjectDetails(_idOfProjectToGet)
                .Throws(_fixture.Create<NotFoundException>());
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
            var accountProjectsDto = _result.Value as AccountProjectsDto;
            accountProjectsDto.Should().Be(_accountProjectsReturnedByFacade);
        }

        private void ThenTheExpectedProjectIsReturned()
        {
            var returnedProject = _result.Value as ProjectDetailsDto;

            returnedProject.Should().Be(_projectReturnedByFacade);
        }
    }
}
