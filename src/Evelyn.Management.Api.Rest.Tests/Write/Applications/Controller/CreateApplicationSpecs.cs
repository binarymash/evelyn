namespace Evelyn.Management.Api.Rest.Tests.Write.Applications.Controller
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

    public class CreateApplicationSpecs
    {
        private readonly Fixture _fixture;
        private readonly Rest.Write.Applications.Controller _controller;
        private readonly ICommandHandler<CreateApplication> _createApplicationHandler;
        private Rest.Write.Applications.Messages.CreateApplication _message;
        private ObjectResult _result;

        public CreateApplicationSpecs()
        {
            _fixture = new Fixture();
            _createApplicationHandler = Substitute.For<ICommandHandler<Core.WriteModel.Commands.CreateApplication>>();
            _controller = new Rest.Write.Applications.Controller(_createApplicationHandler);
        }

        [Fact]
        public void SuccessfulCreateApplication()
        {
            this.Given(_ => GivenAValidCreateApplicationCommand())
                .When(_ => WhenTheMessageIsPosted())
                .Then(_ => ThenTheCommandIsPassedToTheCommandHandler())
                .And(_ => ThenA202AcceptedStatusIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ConcurrencyExceptionThrownByCommandHandler()
        {
            this.Given(_ => GivenAValidCreateApplicationCommand())
                .And(_ => GivenTheCommandHandlerWillThrowAConcurrencyException())
                .When(_ => WhenTheMessageIsPosted())
                .Then(_ => ThenTheCommandIsPassedToTheCommandHandler())
                .And(_ => ThenA400BadRequestStatusIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ExceptionThrownByCommandHandler()
        {
            this.Given(_ => GivenAValidCreateApplicationCommand())
                .And(_ => GivenTheCommandHandlerWillThrowAnException())
                .When(_ => WhenTheMessageIsPosted())
                .Then(_ => ThenTheCommandIsPassedToTheCommandHandler())
                .And(_ => ThenA500InternalServerErrorStatusIsReturned())
                .BDDfy();
        }

        private void GivenAValidCreateApplicationCommand()
        {
            _message = _fixture.Create<Rest.Write.Applications.Messages.CreateApplication>();
        }

        private void GivenTheCommandHandlerWillThrowAConcurrencyException()
        {
            _createApplicationHandler
                .Handle(Arg.Any<CreateApplication>())
                .Returns(cah => throw new ConcurrencyException(Guid.NewGuid()));
        }

        private void GivenTheCommandHandlerWillThrowAnException()
        {
            _createApplicationHandler
                .Handle(Arg.Any<CreateApplication>())
                .Returns(cah => throw new System.Exception("boom!"));
        }

        private async Task WhenTheMessageIsPosted()
        {
            _result = await _controller.Post(_message) as ObjectResult;
        }

        private void ThenTheCommandIsPassedToTheCommandHandler()
        {
            _createApplicationHandler
                .Received(1)
                .Handle(Arg.Is<CreateApplication>(command =>
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
