namespace Evelyn.Management.Api.Rest.Tests.Read
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.ReadModel.ProjectDetails;
    using Core.ReadModel.ProjectList;
    using Evelyn.Core.ReadModel;
    using Evelyn.Management.Api.Rest.Read;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using TestStack.BDDfy;
    using Xunit;

    public class ProjectsControllerSpecs
    {
        private readonly Fixture _fixture;
        private readonly ProjectsController _controller;
        private readonly IReadModelFacade _readModelFacade;
        private Guid _idOfProjectToGet;
        private IEnumerable<ProjectListDto> _projectsReturnedByFacade;
        private ProjectDetailsDto _projectReturnedByFacade;
        private ObjectResult _result;

        public ProjectsControllerSpecs()
        {
            _fixture = new Fixture();
            _readModelFacade = Substitute.For<IReadModelFacade>();
            _controller = new ProjectsController(_readModelFacade);
        }

        [Fact]
        public void GetsAllProjects()
        {
            this.Given(_ => GivenThatThereAreProjects())
                .When(_ => WhenWeGetAllTheProjects())
                .Then(_ => ThenStatusCode200IsReturned())
                .And(_ => ThenAllProjectsAreReturned())
                .BDDfy();
        }

        [Fact]
        public void ExceptionWhenGettingProjects()
        {
            this.Given(_ => GivenThatAnExceptionIsThrownByHandlerWhenGettingProjects())
                .When(_ => WhenWeGetAllTheProjects())
                .Then(_ => ThenStatusCode500IsReturned())
                .BDDfy();
        }

        [Fact]
        public void GetsProject()
        {
            this.Given(_ => GivenTheProjectWeWantDoesExist())
                .When(_ => WhenWeGetTheProject())
                .Then(_ => ThenStatusCode200IsReturned())
                .And(_ => ThenTheExpectedProjectIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ProjectNotFound()
        {
            this.Given(_ => GivenTheProjectWeWantDoesntExist())
                .When(_ => WhenWeGetTheProject())
                .Then(_ => ThenStatusCode404IsReturned())
                .BDDfy();
        }

        [Fact]
        public void ExceptionWhenGettingProject()
        {
            this.Given(_ => GivenThatAnExceptionIsThrownWhenGettingProject())
                .When(_ => WhenWeGetTheProject())
                .Then(_ => ThenStatusCode500IsReturned())
                .BDDfy();
        }

        private void GivenThatThereAreProjects()
        {
            _projectsReturnedByFacade = _fixture.CreateMany<ProjectListDto>();

            _readModelFacade.GetProjects().Returns(_projectsReturnedByFacade);
        }

        private void GivenThatAnExceptionIsThrownByHandlerWhenGettingProjects()
        {
            _readModelFacade
                .GetProjects()
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

        private async Task WhenWeGetAllTheProjects()
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
            var returnedProjects = (_result.Value as IEnumerable<ProjectListDto>).ToList();

            returnedProjects.Should().Equal(_projectsReturnedByFacade);
        }

        private void ThenTheExpectedProjectIsReturned()
        {
            var returnedProject = _result.Value as ProjectDetailsDto;

            returnedProject.Should().Be(_projectReturnedByFacade);
        }
    }
}
