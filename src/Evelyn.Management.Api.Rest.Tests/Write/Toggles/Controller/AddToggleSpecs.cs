namespace Evelyn.Management.Api.Rest.Tests.Write.Toggles.Controller
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core;
    using CQRSlite.Commands;
    using CQRSlite.Domain.Exception;
    using Evelyn.Core.WriteModel.Project.Commands.AddToggle;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class AddToggleSpecs
    {
        private readonly Fixture _fixture;
        private readonly Rest.Write.Toggles.Controller _controller;
        private readonly ICommandHandler<Command> _handler;
        private readonly Guid _projectId;
        private Rest.Write.Toggles.Messages.AddToggle _message;
        private ObjectResult _result;

        public AddToggleSpecs()
        {
            _fixture = new Fixture();
            _handler = Substitute.For<ICommandHandler<Command>>();
            _controller = new Rest.Write.Toggles.Controller(_handler, null);
            _projectId = _fixture.Create<Guid>();
        }

        [Fact]
        public void SuccessfulAddToggle()
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
            _message = _fixture.Create<Rest.Write.Toggles.Messages.AddToggle>();
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
            _result = await _controller.Post(_projectId, _message);
        }

        private void ThenACommandIsPassedToTheCommandHandler()
        {
            _handler
                .Received(1)
                .Handle(Arg.Is<Command>(command =>
                    command.UserId == Constants.AnonymousUser &&
                    command.ProjectId == _projectId &&
                    command.Key == _message.Key &&
                    command.Name == _message.Name &&
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
