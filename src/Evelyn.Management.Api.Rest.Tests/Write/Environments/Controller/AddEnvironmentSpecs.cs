namespace Evelyn.Management.Api.Rest.Tests.Write.Environments.Controller
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using CQRSlite.Commands;
    using CQRSlite.Domain.Exception;
    using Evelyn.Core.WriteModel.Commands;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class AddEnvironmentSpecs
    {
        private readonly Fixture _fixture;
        private readonly Rest.Write.Environments.Controller _controller;
        private readonly ICommandHandler<AddEnvironment> _handler;
        private readonly Guid _applicationId;
        private Rest.Write.Environments.Messages.AddEnvironment _message;
        private ObjectResult _result;

        public AddEnvironmentSpecs()
        {
            _fixture = new Fixture();
            _handler = Substitute.For<ICommandHandler<AddEnvironment>>();
            _controller = new Rest.Write.Environments.Controller(_handler);
            _applicationId = _fixture.Create<Guid>();
        }

        [Fact]
        public void SuccessfulAddEnvironment()
        {
            this.Given(_ => GivenAValidAddEnvironmentCommand())
                .When(_ => WhenTheMessageIsPosted())
                .Then(_ => ThenACommandIsPassedToTheCommandHandler())
                .And(_ => ThenA202AcceptedStatusIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ConcurrencyExceptionThrownByCommandHandler()
        {
            this.Given(_ => GivenAValidAddEnvironmentCommand())
                .And(_ => GivenTheCommandHandlerWillThrowAConcurrencyException())
                .When(_ => WhenTheMessageIsPosted())
                .Then(_ => ThenACommandIsPassedToTheCommandHandler())
                .And(_ => ThenA400BadRequestStatusIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ExceptionThrownByCommandHandler()
        {
            this.Given(_ => GivenAValidAddEnvironmentCommand())
                .And(_ => GivenTheCommandHandlerWillThrowAnException())
                .When(_ => WhenTheMessageIsPosted())
                .Then(_ => ThenACommandIsPassedToTheCommandHandler())
                .And(_ => ThenA500InternalServerErrorStatusIsReturned())
                .BDDfy();
        }

        private void GivenAValidAddEnvironmentCommand()
        {
            _message = _fixture.Create<Rest.Write.Environments.Messages.AddEnvironment>();
        }

        private void GivenTheCommandHandlerWillThrowAConcurrencyException()
        {
            _handler
                .Handle(Arg.Any<AddEnvironment>())
                .Returns(cah => throw new ConcurrencyException(Guid.NewGuid()));
        }

        private void GivenTheCommandHandlerWillThrowAnException()
        {
            _handler
                .Handle(Arg.Any<AddEnvironment>())
                .Returns(cah => throw new System.Exception("boom!"));
        }

        private async Task WhenTheMessageIsPosted()
        {
            _result = await _controller.Post(_applicationId, _message) as ObjectResult;
        }

        private void ThenACommandIsPassedToTheCommandHandler()
        {
            _handler
                .Received(1)
                .Handle(Arg.Is<AddEnvironment>(command =>
                    command.ApplicationId == _applicationId &&
                    command.Id == _message.Id &&
                    command.Name == _message.Name &&
                    command.ExpectedVersion == _message.ExpectedVersion));
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
