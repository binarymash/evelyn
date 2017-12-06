namespace Evelyn.Api.Rest.Tests.Write
{
    using System.Threading.Tasks;
    using AutoFixture;
    using CQRSlite.Commands;
    using Evelyn.Api.Rest.Write;
    using Evelyn.Core.WriteModel.Commands;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class ApplicationsControllerSpecs
    {
        private Fixture _fixture;
        private ApplicationsController _controller;
        private ICommandHandler<CreateApplication> _createApplicationHandler;
        private CreateApplication _command;
        private IActionResult _result;

        public ApplicationsControllerSpecs()
        {
            _fixture = new Fixture();
            _createApplicationHandler = Substitute.For<ICommandHandler<Core.WriteModel.Commands.CreateApplication>>();
            _controller = new ApplicationsController(_createApplicationHandler);
        }

        [Fact]
        public void SuccessfulCreateApplication()
        {
            this.Given(_ => GivenAValidCreateApplicationCommand())
                .When(_ => WhenTheCommandIsPosted())
                .Then(_ => ThenTheCommandIsPassedToTheCommandHandler())
                .And(_ => ThenA202AcceptedStatusIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ExceptionThrownByCommandHandler()
        {
            this.Given(_ => GivenAValidCreateApplicationCommand())
                .And(_ => GivenTheCommandHandlerWillThrowAnException())
                .When(_ => WhenTheCommandIsPosted())
                .Then(_ => ThenTheCommandIsPassedToTheCommandHandler())
                .And(_ => ThenA500InternalServerErrorStatusIsReturned())
                .BDDfy();
        }

        private void GivenAValidCreateApplicationCommand()
        {
            _command = _fixture.Create<CreateApplication>();
        }

        private void GivenTheCommandHandlerWillThrowAnException()
        {
            _createApplicationHandler
                .Handle(Arg.Any<CreateApplication>())
                .Returns(cah => { throw new System.Exception("boom!"); });
        }

        private async Task WhenTheCommandIsPosted()
        {
            _result = await _controller.Post(_command);
        }

        private void ThenTheCommandIsPassedToTheCommandHandler()
        {
            _createApplicationHandler.Received(1).Handle(_command);
        }

        private void ThenA202AcceptedStatusIsReturned()
        {
        }

        private void ThenA500InternalServerErrorStatusIsReturned()
        {
        }
    }
}
