namespace Evelyn.Management.Api.Rest.Tests.Write.Projects.Controller
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core;
    using CQRSlite.Commands;
    using CQRSlite.Domain.Exception;
    using Evelyn.Core.WriteModel.Project.Commands.DeleteProject;
    using Evelyn.Management.Api.Rest.Write.Projects;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class DeleteProjectSpecs
    {
        private readonly Fixture _fixture;
        private readonly ILogger<Rest.Write.Projects.Controller> _logger;
        private readonly Rest.Write.Projects.Controller _controller;
        private readonly ICommandHandler<Command> _deleteProjectHandler;
        private Rest.Write.Projects.Messages.DeleteProject _message;
        private ObjectResult _result;
        private Guid _projectId;

        public DeleteProjectSpecs()
        {
            _fixture = new Fixture();
            _logger = Substitute.For<ILogger<Rest.Write.Projects.Controller>>();
            _deleteProjectHandler = Substitute.For<ICommandHandler<Command>>();
            _controller = new Rest.Write.Projects.Controller(_logger, null, _deleteProjectHandler);
            _projectId = _fixture.Create<Guid>();
        }

        [Fact]
        public void SuccessfulDeleteProject()
        {
            this.Given(_ => GivenAValidDeleteProjectCommand())
                .When(_ => WhenTheMessageIsPosted())
                .Then(_ => ThenTheCommandIsPassedToTheCommandHandler())
                .And(_ => ThenA202AcceptedStatusIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ConcurrencyExceptionThrownByCommandHandler()
        {
            this.Given(_ => GivenAValidDeleteProjectCommand())
                .And(_ => GivenTheCommandHandlerWillThrowAConcurrencyException())
                .When(_ => WhenTheMessageIsPosted())
                .Then(_ => ThenTheCommandIsPassedToTheCommandHandler())
                .And(_ => ThenA409ConflictStatusIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ExceptionThrownByCommandHandler()
        {
            this.Given(_ => GivenAValidDeleteProjectCommand())
                .And(_ => GivenTheCommandHandlerWillThrowAnException())
                .When(_ => WhenTheMessageIsPosted())
                .Then(_ => ThenTheCommandIsPassedToTheCommandHandler())
                .And(_ => ThenA500InternalServerErrorStatusIsReturned())
                .BDDfy();
        }

        private void GivenAValidDeleteProjectCommand()
        {
            _message = _fixture.Create<Rest.Write.Projects.Messages.DeleteProject>();
        }

        private void GivenTheCommandHandlerWillThrowAConcurrencyException()
        {
            _deleteProjectHandler
                .Handle(Arg.Any<Command>())
                .Returns(cah => throw _fixture.Create<Core.WriteModel.ConcurrencyException>());
        }

        private void GivenTheCommandHandlerWillThrowAnException()
        {
            _deleteProjectHandler
                .Handle(Arg.Any<Command>())
                .Returns(cah => throw new System.Exception("boom!"));
        }

        private async Task WhenTheMessageIsPosted()
        {
            _result = await _controller.Post(_projectId, _message);
        }

        private void ThenTheCommandIsPassedToTheCommandHandler()
        {
            _deleteProjectHandler
                .Received(1)
                .Handle(Arg.Is<Command>(command =>
                    command.UserId == Constants.AnonymousUser &&
                    command.ProjectId == _projectId &&
                    command.ExpectedProjectVersion == _message.ExpectedProjectVersion));
        }

        private void ThenA202AcceptedStatusIsReturned()
        {
            _result.StatusCode.Should().Be(StatusCodes.Status202Accepted);
        }

        private void ThenA400BadRequestStatusIsReturned()
        {
            _result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        private void ThenA409ConflictStatusIsReturned()
        {
            _result.StatusCode.Should().Be(StatusCodes.Status409Conflict);
        }

        private void ThenA500InternalServerErrorStatusIsReturned()
        {
            _result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }
}
