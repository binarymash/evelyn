namespace Evelyn.Agent.UnitTests.Mediatr.Behaviors.ValidationSpecs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BinaryMash.Responses;
    using Evelyn.Agent.Mediatr.Behaviors;
    using FluentValidation;
    using Shouldly;

    public abstract class ValidationSpecs<TRequest, TResponse> : BehaviourSpecs<TRequest, TResponse>
        where TResponse : Response
    {
        protected IEnumerable<IValidator<TRequest>> Validators { get; set; }

        protected IEnumerable<string> ExpectedValidationMessages { get; set; }

        protected void GivenValidatorsCollectionIsNull()
        {
            GivenValidators(null);
        }

        protected void GivenEmptyValidators()
        {
            GivenValidators(new List<IValidator<TRequest>>());
        }

        protected void GivenValidators(IEnumerable<IValidator<TRequest>> validators)
        {
            Validators = validators;
        }

        protected override void WhenHandled()
        {
            Behavior = new Validation<TRequest, TResponse>(Validators);
            base.WhenHandled();
        }

        protected void ThenTheResponseContainsAnInvalidNullRequestError()
        {
            this.Response.Errors.Count.ShouldBe(1);
            var error = Response.Errors.First();
            error.Code.ShouldBe("InvalidRequest");
            error.Message.ShouldBe("The request must not be null");
        }

        protected void ThenTheResponseContainsAnInvalidRequestErrorDescribingEachFailure()
        {
            this.Response.Errors.Count.ShouldBe(1);
            var error = Response.Errors.First();
            error.Code.ShouldBe("InvalidRequest");
            var validationMessages = error.Message.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            validationMessages.Count().ShouldBe(ExpectedValidationMessages.Count());
            foreach (var validationMessage in validationMessages)
            {
                ExpectedValidationMessages.Contains(validationMessage);
            }
        }
    }
}
