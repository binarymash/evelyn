namespace Evelyn.Management.Api.Rest.Tests.Write.Projects.Controller
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using CQRSlite.Commands;
    using CQRSlite.Domain.Exception;
    using Evelyn.Core.WriteModel.Project.Commands;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Rest.Write;
    using TestStack.BDDfy;
    using Xunit;

    public class CreateProjectSpecs
    {
        private readonly Fixture _fixture;
        private readonly Rest.Write.Projects.Controller _controller;
        private readonly ICommandHandler<CreateProject> _createProjectHandler;
        private Rest.Write.Projects.Messages.CreateProject _message;
        private ObjectResult _result;

        public CreateProjectSpecs()
        {
            _fixture = new Fixture();
            _createProjectHandler = Substitute.For<ICommandHandler<Core.WriteModel.Project.Commands.CreateProject>>();
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
                .And(_ => ThenA400BadRequestStatusIsReturned())
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
                .Handle(Arg.Any<CreateProject>())
                .Returns(cah => throw new ConcurrencyException(Guid.NewGuid()));
        }

        private void GivenTheCommandHandlerWillThrowAnException()
        {
            _createProjectHandler
                .Handle(Arg.Any<CreateProject>())
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
                .Handle(Arg.Is<CreateProject>(command =>
                    command.UserId == Constants.AnonymousUser &&
                    command.AccountId == Constants.DefaultAccount &&
                    command.Id == _message.Id &&
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

        private void ThenA500InternalServerErrorStatusIsReturned()
        {
            _result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }
}
