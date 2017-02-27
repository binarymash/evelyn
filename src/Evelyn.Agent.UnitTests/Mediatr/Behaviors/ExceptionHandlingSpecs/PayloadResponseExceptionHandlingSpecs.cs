﻿namespace Evelyn.Agent.UnitTests.Mediatr.Behaviors.ExceptionHandlingSpecs
{
    using BinaryMash.Responses;
    using Moq;
    using TestStack.BDDfy;
    using Xunit;

    public class PayloadResponseExceptionHandlingSpecs : ExceptionHandlingSpecs<Support.MyRequest, Response<Support.MyPayload>>
    {
        [Fact]
        public void ExceptionNotThrownByNextHandler()
        {
            this.Given(_ => GivenTheNextHandlerDoesNotThrowAnException())
                .When(_ => WhenHandled())
                .Then(_ => ThenTheNextHandlerIsCalled())
                .And(_ => ThenTheResponseFromTheNextHandlerIsReturned())
                .And(_ => ThenNoErrorIsLogged())
                .BDDfy();
        }

        [Fact]
        public void ExceptionThrownByNextHandler()
        {
            this.Given(_ => GivenTheNextHandlerThrowsAnException())
                .When(_ => WhenHandled())
                .Then(_ => ThenTheNextHandlerIsCalled())
                .And(_ => ThenTheExceptionIsLogged())
                .And(_ => ThenAnInternalErrorIsReturned())
                .BDDfy();
        }

        private void GivenTheNextHandlerDoesNotThrowAnException()
        {
            ResponseFromNext = BuildResponse
                .WithPayload(new Support.MyPayload { MyProperty = "test" })
                .Create();
            Next.Setup(n => n()).ReturnsAsync(ResponseFromNext);
        }
    }
}
