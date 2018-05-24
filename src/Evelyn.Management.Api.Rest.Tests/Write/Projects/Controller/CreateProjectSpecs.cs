namespace Evelyn.Management.Api.Rest.Tests.Write.Projects.Controller
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core;
    using CQRSlite.Commands;
    using CQRSlite.Domain.Exception;
    using Evelyn.Core.WriteModel.Account.Commands.CreateProject;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class CreateProjectSpecs
    {
        private readonly Fixture _fixture;
        private readonly Rest.Write.Projects.Controller _controller;
        private readonly ICommandHandler<Command> _createProjectHandler;
        private Rest.Write.Projects.Messages.CreateProject _message;
        private ObjectResult _result;

        public CreateProjectSpecs()
        {
            _fixture = new Fixture();
            _createProjectHandler = Substitute.For<ICommandHandler<Command>>();
            _controller = new Rest.Write.Projects.Controller(_createProjectHandler);
        }

        [Fact]
        public void SuccessfulCreateProject()
        {
            this.Given(_ => GivenAValidCreateProjectCommand())
                .When(_ => WhenTheMessageIsPosted())
                .Then(_ => ThenTheCommandIsPassedToTheCommandHandler())
                .And(_ => ThenA202AcceptedStatusIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ConcurrencyExceptionThrownByCommandHandler()
        {
            this.Given(_ => GivenAValidCreateProjectCommand())
                .And(_ => GivenTheCommandHandlerWillThrowAConcurrencyException())
                .When(_ => WhenTheMessageIsPosted())
                .Then(_ => ThenTheCommandIsPassedToTheCommandHandler())
                .And(_ => ThenA409ConflictStatusIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ExceptionThrownByCommandHandler()
        {
            this.Given(_ => GivenAValidCreateProjectCommand())
                .And(_ => GivenTheCommandHandlerWillThrowAnException())
                .When(_ => WhenTheMessageIsPosted())
                .Then(_ => ThenTheCommandIsPassedToTheCommandHandler())
                .And(_ => ThenA500InternalServerErrorStatusIsReturned())
                .BDDfy();
        }

        private void GivenAValidCreateProjectCommand()
        {
            _message = _fixture.Create<Rest.Write.Projects.Messages.CreateProject>();
        }

        private void GivenTheCommandHandlerWillThrowAConcurrencyException()
        {
            _createProjectHandler
                .Handle(Arg.Any<Command>())
                .Returns(cah => throw new ConcurrencyException(Guid.NewGuid()));
        }

        private void GivenTheCommandHandlerWillThrowAnException()
        {
            _createProjectHandler
                .Handle(Arg.Any<Command>())
                .Returns(cah => throw new System.Exception("boom!"));
        }

        private async Task WhenTheMessageIsPosted()
        {
            _result = await _controller.Post(_message);
        }

        private void ThenTheCommandIsPassedToTheCommandHandler()
        {
            _createProjectHandler
                .Received(1)
                .Handle(Arg.Is<Command>(command =>
                    command.UserId == Constants.AnonymousUser &&
                    command.Id == Constants.DefaultAccount &&
                    command.ProjectId == _message.ProjectId &&
                    command.Name == _message.Name &&
                    command.ExpectedVersion == null));
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
