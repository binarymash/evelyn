namespace Evelyn.Management.Api.Rest.Tests.Write.Environments.Controller
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core;
    using Core.WriteModel.Project.Commands.DeleteEnvironment;
    using CQRSlite.Commands;
    using CQRSlite.Domain.Exception;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class DeleteEnvironmentSpecs
    {
        private readonly Fixture _fixture;
        private readonly Rest.Write.Environments.Controller _controller;
        private readonly ICommandHandler<Command> _handler;
        private readonly Guid _projectId;
        private readonly string _environmentKey;
        private Rest.Write.Environments.Messages.DeleteEnvironment _message;
        private ObjectResult _result;

        public DeleteEnvironmentSpecs()
        {
            _fixture = new Fixture();
            _handler = Substitute.For<ICommandHandler<Command>>();
            _controller = new Rest.Write.Environments.Controller(null, _handler);
            _projectId = _fixture.Create<Guid>();
            _environmentKey = _fixture.Create<string>();
        }

        [Fact]
        public void SuccessfulDeleteEnvironment()
        {
            this.Given(_ => GivenAValidDeleteEnvironmentCommand())
                .When(_ => WhenTheMessageIsPosted())
                .Then(_ => ThenACommandIsPassedToTheCommandHandler())
                .And(_ => ThenA202AcceptedStatusIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ConcurrencyExceptionThrownByCommandHandler()
        {
            this.Given(_ => GivenAValidDeleteEnvironmentCommand())
                .And(_ => GivenTheCommandHandlerWillThrowAConcurrencyException())
                .When(_ => WhenTheMessageIsPosted())
                .Then(_ => ThenACommandIsPassedToTheCommandHandler())
                .And(_ => ThenA409ConflictStatusIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ExceptionThrownByCommandHandler()
        {
            this.Given(_ => GivenAValidDeleteEnvironmentCommand())
                .And(_ => GivenTheCommandHandlerWillThrowAnException())
                .When(_ => WhenTheMessageIsPosted())
                .Then(_ => ThenACommandIsPassedToTheCommandHandler())
                .And(_ => ThenA500InternalServerErrorStatusIsReturned())
                .BDDfy();
        }

        private void GivenAValidDeleteEnvironmentCommand()
        {
            _message = _fixture.Create<Rest.Write.Environments.Messages.DeleteEnvironment>();
        }

        private void GivenTheCommandHandlerWillThrowAConcurrencyException()
        {
            _handler
                .Handle(Arg.Any<Command>())
                .Returns(cah => throw new ConcurrencyException(Guid.NewGuid()));
        }

        private void GivenTheCommandHandlerWillThrowAnException()
        {
            _handler
                .Handle(Arg.Any<Command>())
                .Returns(cah => throw new System.Exception("boom!"));
        }

        private async Task WhenTheMessageIsPosted()
        {
            _result = await _controller.Post(_projectId, _environmentKey, _message);
        }

        private void ThenACommandIsPassedToTheCommandHandler()
        {
            _handler
                .Received(1)
                .Handle(Arg.Is<Command>(command =>
                    command.UserId == Constants.AnonymousUser &&
                    command.ProjectId == _projectId &&
                    command.Key == _environmentKey &&
                    command.ExpectedEnvironmentVersion == _message.ExpectedEnvironmentVersion));
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
