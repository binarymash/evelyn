namespace Evelyn.Management.Api.Rest.Tests.Write.ToggleStates.Controller
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core;
    using Core.WriteModel.Project.Commands.ChangeToggleState;
    using CQRSlite.Commands;
    using CQRSlite.Domain.Exception;
    using Evelyn.Management.Api.Rest.Write.ToggleStates;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class ChangeToggleStateSpecs
    {
        private readonly Fixture _fixture;
        private readonly ILogger<Rest.Write.ToggleStates.Controller> _logger;
        private readonly Rest.Write.ToggleStates.Controller _controller;
        private readonly ICommandHandler<Command> _handler;
        private readonly Guid _projectId;
        private readonly string _environmentKey;
        private readonly string _toggleKey;
        private Rest.Write.ToggleStates.Messages.ChangeToggleState _message;
        private ObjectResult _result;

        public ChangeToggleStateSpecs()
        {
            _fixture = new Fixture();
            _logger = Substitute.For<ILogger<Rest.Write.ToggleStates.Controller>>();
            _handler = Substitute.For<ICommandHandler<Command>>();
            _controller = new Rest.Write.ToggleStates.Controller(_logger, _handler);
            _projectId = _fixture.Create<Guid>();
            _environmentKey = _fixture.Create<string>();
            _toggleKey = _fixture.Create<string>();
        }

        [Fact]
        public void SuccessfulChangeToggleStatus()
        {
            this.Given(_ => GivenAValidAddToggleMessage())
                .When(_ => WhenTheMessageIsPosted())
                .Then(_ => ThenACommandIsPassedToTheCommandHandler())
                .And(_ => ThenA202AcceptedStatusIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ConcurrencyExceptionThrownByCommandHandler()
        {
            this.Given(_ => GivenAValidAddToggleMessage())
                .And(_ => GivenTheCommandHandlerWillThrowAConcurrencyException())
                .When(_ => WhenTheMessageIsPosted())
                .Then(_ => ThenACommandIsPassedToTheCommandHandler())
                .And(_ => ThenA409ConflictStatusIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ExceptionThrownByCommandHandler()
        {
            this.Given(_ => GivenAValidAddToggleMessage())
                .And(_ => GivenTheCommandHandlerWillThrowAnException())
                .When(_ => WhenTheMessageIsPosted())
                .Then(_ => ThenACommandIsPassedToTheCommandHandler())
                .And(_ => ThenA500InternalServerErrorStatusIsReturned())
                .BDDfy();
        }

        private void GivenAValidAddToggleMessage()
        {
            _message = _fixture.Create<Rest.Write.ToggleStates.Messages.ChangeToggleState>();
        }

        private void GivenTheCommandHandlerWillThrowAConcurrencyException()
        {
            _handler
                .Handle(Arg.Any<Command>())
                .Returns(cah => throw _fixture.Create<Core.WriteModel.ConcurrencyException>());
        }

        private void GivenTheCommandHandlerWillThrowAnException()
        {
            _handler
                .Handle(Arg.Any<Command>())
                .Returns(cah => throw new System.Exception("boom!"));
        }

        private async Task WhenTheMessageIsPosted()
        {
            _result = await _controller.Post(_projectId, _environmentKey, _toggleKey, _message);
        }

        private void ThenACommandIsPassedToTheCommandHandler()
        {
            // TODO: add all properties
            _handler
                .Received(1)
                .Handle(Arg.Is<Command>(command =>
                    command.UserId == Constants.AnonymousUser &&
                    command.ProjectId == _projectId &&
                    command.EnvironmentKey == _environmentKey &&
                    command.ToggleKey == _toggleKey &&
                    command.Value == _message.State));
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
