namespace Evelyn.Agent.UnitTests.Mediatr.Behaviors
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using BinaryMash.Responses;
    using MediatR;
    using Moq;
    using Shouldly;

    public abstract class BehaviourSpecs<TRequest, TResponse>
                where TResponse : Response
    {
        public BehaviourSpecs()
        {
            Next = new Mock<RequestHandlerDelegate<TResponse>>();
        }

        protected IPipelineBehavior<TRequest, TResponse> Behavior { get; set; }

        protected TRequest Request { get; set; }

        protected Mock<RequestHandlerDelegate<TResponse>> Next { get; set; }

        protected TResponse ResponseFromNext { get; set; }

        protected TResponse Response { get; set; }

        protected Exception ExceptionThrownFromNextHandler { get; set; }

        protected Exception ThrownException { get; set; }

        protected void GivenARequest(TRequest request)
        {
            Request = request;
        }

        protected void GivenTheNextHandlerWillReturnAResponse(TResponse response)
        {
            ResponseFromNext = response;
            Next.Setup(n => n())
                .ReturnsAsync(ResponseFromNext);
        }

        protected void GivenTheNextHandlerWillThrowAnException()
        {
            ExceptionThrownFromNextHandler = new Exception("boom!");
            Next.Setup(n => n())
                .Throws(ExceptionThrownFromNextHandler);
        }

        protected virtual void WhenHandled()
        {
            try
            {
                Response = Behavior.Handle(Request, Next.Object).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                ThrownException = ex;
            }
        }

        protected void ThenTheNextHandlerIsCalled()
        {
            Next.Verify(n => n(), Times.Once);
        }

        protected void ThenTheNextHandlerIsNotCalled()
        {
            Next.Verify(n => n(), Times.Never);
        }

        protected void ThenTheResponseFromTheNextHandlerIsReturned()
        {
            Response.ShouldBe(ResponseFromNext);
        }

        protected void ThenTheResponseContainsNoErrors()
        {
            Response.Errors.Any().ShouldBeFalse();
        }

        protected void ThenTheExceptionIsRethrown()
        {
            ThrownException.ShouldBe(ExceptionThrownFromNextHandler);
        }

        private void NoErrorIsLogged()
        {
            throw new NotImplementedException();
        }
    }
}
