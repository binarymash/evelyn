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
    using TestStack.BDDfy;
    using Xunit;

    public class FlipToggleSpecs
    {
        private readonly Fixture _fixture;
        private readonly Rest.Write.ToggleStates.Controller _controller;
        private readonly ICommandHandler<FlipToggle> _handler;
        private readonly Guid _applicationId;
        private Rest.Write.ToggleStates.Messages.FlipToggle _message;
        private ObjectResult _result;

        public FlipToggleSpecs()
        {
            _fixture = new Fixture();
            _handler = Substitute.For<ICommandHandler<FlipToggle>>();
            _controller = new Rest.Write.ToggleStates.Controller(_handler);
            _applicationId = _fixture.Create<Guid>();
        }

        [Fact]
        public void SuccessfulFlipToggle()
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
            _message = _fixture.Create<Rest.Write.ToggleStates.Messages.FlipToggle>();
        }

        private void GivenTheCommandHandlerWillThrowAConcurrencyException()
        {
            _handler
                .Handle(Arg.Any<FlipToggle>())
                .Returns(cah => throw new ConcurrencyException(Guid.NewGuid()));
        }

        private void GivenTheCommandHandlerWillThrowAnException()
        {
            _handler
                .Handle(Arg.Any<FlipToggle>())
                .Returns(cah => throw new System.Exception("boom!"));
        }

        private async Task WhenTheMessageIsPosted()
        {
            _result = await _controller.Post(_applicationId, _message) as ObjectResult;
        }

        private void ThenACommandIsPassedToTheCommandHandler()
        {
            // TODO: add all properties
            _handler
                .Received(1)
                .Handle(Arg.Is<FlipToggle>(command =>
                    command.ApplicationId == _applicationId));
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
