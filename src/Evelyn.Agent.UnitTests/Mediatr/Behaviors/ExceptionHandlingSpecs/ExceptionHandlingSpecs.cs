namespace Evelyn.Agent.UnitTests.Mediatr.Behaviors.ExceptionHandlingSpecs
{
    using System;
    using System.Linq;
    using BinaryMash.Responses;
    using Evelyn.Agent.Mediatr.Behaviors;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Shouldly;

    public abstract class ExceptionHandlingSpecs<TRequest, TResponse> : BehaviourSpecs<TRequest, TResponse>
        where TResponse : Response
    {
        public ExceptionHandlingSpecs()
        {
            Logger = new Mock<ILogger<Logging<TRequest, TResponse>>>();
            Behavior = new ExceptionHandling<TRequest, TResponse>(Logger.Object);
        }

        protected Mock<ILogger<Logging<TRequest, TResponse>>> Logger { get; set; }

        protected void GivenTheNextHandlerThrowsAnException()
        {
            ExceptionThrownFromNextHandler = new Exception("boom!");
            Next.Setup(n => n()).ThrowsAsync(ExceptionThrownFromNextHandler);
        }

        protected void ThenNoErrorIsLogged()
        {
            Logger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<object>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<object, Exception, string>>()), Times.Never);
        }

        protected void ThenTheExceptionIsLogged()
        {
            Logger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<object>(),
                    ExceptionThrownFromNextHandler,
                    It.IsAny<Func<object, Exception, string>>()), Times.Once);
        }

        protected void ThenAnInternalErrorIsReturned()
        {
            this.Response.Errors.Count.ShouldBe(1);
            var error = Response.Errors.First();
            error.Code.ShouldBe("InternalError");
            error.Message.ShouldBe("An internal error occurred");
        }
    }
}
