namespace Evelyn.Management.Api.Rest.Tests.Write.Toggles.Controller
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core;
    using CQRSlite.Commands;
    using CQRSlite.Domain.Exception;
    using Evelyn.Core.WriteModel.Project.Commands.DeleteToggle;
    using Evelyn.Management.Api.Rest.Write.Toggles;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class DeleteToggleSpecs
    {
        private readonly Fixture _fixture;
        private readonly ILogger<Rest.Write.Toggles.Controller> _logger;
        private readonly Rest.Write.Toggles.Controller _controller;
        private readonly ICommandHandler<Command> _handler;
        private readonly Guid _projectId;
        private readonly string _toggleKey;
        private Rest.Write.Toggles.Messages.DeleteToggle _message;
        private ObjectResult _result;

        public DeleteToggleSpecs()
        {
            _fixture = new Fixture();
            _logger = Substitute.For<ILogger<Rest.Write.Toggles.Controller>>();
            _handler = Substitute.For<ICommandHandler<Command>>();
            _controller = new Rest.Write.Toggles.Controller(_logger, null, _handler);
            _projectId = _fixture.Create<Guid>();
            _toggleKey = _fixture.Create<string>();
        }

        [Fact]
        public void SuccessfulDeleteToggle()
        {
            this.Given(_ => GivenAValidDeleteToggleMessage())
                .When(_ => WhenTheMessageIsPosted())
                .Then(_ => ThenACommandIsPassedToTheCommandHandler())
                .And(_ => ThenA202AcceptedStatusIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ConcurrencyExceptionThrownByCommandHandler()
        {
            this.Given(_ => GivenAValidDeleteToggleMessage())
                .And(_ => GivenTheCommandHandlerWillThrowAConcurrencyException())
                .When(_ => WhenTheMessageIsPosted())
                .Then(_ => ThenACommandIsPassedToTheCommandHandler())
                .And(_ => ThenA409ConflictStatusIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ExceptionThrownByCommandHandler()
        {
            this.Given(_ => GivenAValidDeleteToggleMessage())
                .And(_ => GivenTheCommandHandlerWillThrowAnException())
                .When(_ => WhenTheMessageIsPosted())
                .Then(_ => ThenACommandIsPassedToTheCommandHandler())
                .And(_ => ThenA500InternalServerErrorStatusIsReturned())
                .BDDfy();
        }

        private void GivenAValidDeleteToggleMessage()
        {
            _message = _fixture.Create<Rest.Write.Toggles.Messages.DeleteToggle>();
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
            _result = await _controller.Post(_projectId, _toggleKey, _message);
        }

        private void ThenACommandIsPassedToTheCommandHandler()
        {
            _handler
                .Received(1)
                .Handle(Arg.Is<Command>(command =>
                    command.UserId == Constants.AnonymousUser &&
                    command.ProjectId == _projectId &&
                    command.Key == _toggleKey &&
                    command.ExpectedToggleVersion == _message.ExpectedToggleVersion));
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
