﻿namespace Evelyn.Agent.UnitTests.Mediatr.Behaviors.ValidationSpecs
{
    using System.Collections.Generic;
    using BinaryMash.Responses;
    using FluentValidation;
    using TestStack.BDDfy;
    using Xunit;
    using static Evelyn.Agent.UnitTests.Mediatr.Behaviors.ValidationSpecs.ResponseSpecs;

    public class ResponseSpecs : ValidationBehaviourSpecs<SomeRequest, Response>
    {
        [Fact]
        public void NullRequest()
        {
            this.Given(_ => GivenANullRequest())
                .When(_ => WhenHandled())
                .Then(_ => ThenTheNextHandlerIsNotCalled())
                .And(_ => ThenTheResponseContainsAnInvalidNullRequestError())
                .BDDfy();
        }

        [Fact]
        public void NullValidators()
        {
            this.Given(_ => GivenValidatorsCollectionIsNull())
                .And(_ => GivenAValidRequest())
                .And(_ => GivenTheNextHandlerWillReturnAResponse())
                .When(_ => WhenHandled())
                .Then(_ => ThenTheNextHandlerIsCalled())
                .Then(_ => ThenTheResponseContainsNoErrors())
                .BDDfy();
        }

        [Fact]
        public void RequestWithOneValidationFailure()
        {
            this.Given(_ => GivenValidators())
                .And(_ => GivenARequestWithOneValidationFailure())
                .When(_ => WhenHandled())
                .Then(_ => ThenTheNextHandlerIsNotCalled())
                .And(_ => ThenTheResponseContainsAnInvalidRequestErrorDescribingEachFailure())
                .BDDfy();
        }

        [Fact]
        public void RequestWithTwoValidationFailures()
        {
            this.Given(_ => GivenValidators())
                .And(_ => GivenARequestWithTwoValidationFailures())
                .When(_ => WhenHandled())
                .Then(_ => ThenTheNextHandlerIsNotCalled())
                .And(_ => ThenTheResponseContainsAnInvalidRequestErrorDescribingEachFailure())
                .BDDfy();
        }

        [Fact]
        public void RequestAcceptedByValidators()
        {
            this.Given(_ => GivenValidators())
                .And(_ => GivenAValidRequest())
                .And(_ => GivenTheNextHandlerWillReturnAResponse())
                .When(_ => WhenHandled())
                .Then(_ => ThenTheNextHandlerIsCalled())
                .And(_ => ThenTheResponseContainsNoErrors())
                .BDDfy();
        }

        private void GivenANullRequest()
        {
            GivenARequest(null);
        }

        private void GivenAValidRequest()
        {
            GivenARequest(new SomeRequest(true, 10));
        }

        private void GivenARequestWithOneValidationFailure()
        {
            ExpectedValidationMessages = new List<string>
            {
                "'MustBeTrue' should be equal to 'True'."
            };

            GivenARequest(new SomeRequest(false, 10));
        }

        private void GivenARequestWithTwoValidationFailures()
        {
            ExpectedValidationMessages = new List<string>
            {
                "'MustBeTrue' should be equal to 'True'.",
                "'MustBeGreaterThan5' should be greater than 5"
            };

            GivenARequest(new SomeRequest(false, 0));
        }

        private void GivenValidators()
        {
            GivenValidators(new List<IValidator<SomeRequest>>()
            {
                new IsTrueValidator(),
                new IsGreaterThan5Validator()
            });
        }

        private void GivenTheNextHandlerWillReturnAResponse()
        {
            GivenTheNextHandlerWillReturnAResponse(BuildResponse.WithNoPayload().Create());
        }

        public class SomeRequest
        {
            public SomeRequest(bool mustBeTrue, int mustBeGreaterThan5)
            {
                MustBeTrue = mustBeTrue;
                MustBeGreaterThan5 = mustBeGreaterThan5;
            }

            public bool MustBeTrue { get; private set; }

            public int MustBeGreaterThan5 { get; private set; }
        }

        public class IsTrueValidator : AbstractValidator<SomeRequest>
        {
            public IsTrueValidator()
            {
                this.RuleFor(sr => sr.MustBeTrue).Equal(true);
            }
        }

        public class IsGreaterThan5Validator : AbstractValidator<SomeRequest>
        {
            public IsGreaterThan5Validator()
            {
                this.RuleFor(sr => sr.MustBeGreaterThan5).GreaterThan(5);
            }
        }
    }
}
