namespace Evelyn.Agent.UnitTests.Mediatr.Behaviors.ValidationSpecs.Support
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using FluentValidation;

    public class IsGreaterThan5Validator : AbstractValidator<SomeRequest>
    {
        public IsGreaterThan5Validator()
        {
            this.RuleFor(sr => sr.MustBeGreaterThan5).GreaterThan(5);
        }
    }
}
