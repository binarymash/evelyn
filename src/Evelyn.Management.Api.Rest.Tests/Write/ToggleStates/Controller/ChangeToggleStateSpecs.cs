namespace Evelyn.Management.Api.Rest.Tests.Write.ToggleStates.Controller
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.WriteModel.Commands;
    using CQRSlite.Commands;
    using CQRSlite.Domain.Exception;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Rest.Write;
    using TestStack.BDDfy;
    using Xunit;

    public class ChangeToggleStateSpecs
    {
        private readonly Fixture _fixture;
        private readonly Rest.Write.ToggleStates.Controller _controller;
        private readonly ICommandHandler<ChangeToggleState> _handler;
        private readonly Guid _applicationId;
        private readonly Guid _environmentId;
        private readonly Guid _toggleId;
        private Rest.Write.ToggleStates.Messages.ChangeToggleState _message;
        private ObjectResult _result;

        public ChangeToggleStateSpecs()
        {
            _fixture = new Fixture();
            _handler = Substitute.For<ICommandHandler<ChangeToggleState>>();
            _controller = new Rest.Write.ToggleStates.Controller(_handler);
            _applicationId = _fixture.Create<Guid>();
            _environmentId = _fixture.Create<Guid>();
            _toggleId = _fixture.Create<Guid>();
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
                .And(_ => ThenA400BadRequestStatusIsReturned())
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
                .Handle(Arg.Any<ChangeToggleState>())
                .Returns(cah => throw new ConcurrencyException(Guid.NewGuid()));
        }

        private void GivenTheCommandHandlerWillThrowAnException()
        {
            _handler
                .Handle(Arg.Any<ChangeToggleState>())
                .Returns(cah => throw new System.Exception("boom!"));
        }

        private async Task WhenTheMessageIsPosted()
        {
            _result = await _controller.Post(_applicationId, _environmentId, _toggleId, _message);
        }

        private void ThenACommandIsPassedToTheCommandHandler()
        {
            // TODO: add all properties
            _handler
                .Received(1)
                .Handle(Arg.Is<ChangeToggleState>(command =>
                    command.UserId == Constants.AnonymousUser &&
                    command.ApplicationId == _applicationId &&
                    command.EnvironmentId == _environmentId &&
                    command.ToggleId == _toggleId &&
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

        private void ThenA500InternalServerErrorStatusIsReturned()
        {
            _result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }
}
