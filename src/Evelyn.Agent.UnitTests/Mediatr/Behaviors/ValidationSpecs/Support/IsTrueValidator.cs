namespace Evelyn.Agent.UnitTests.Mediatr.Behaviors.ValidationSpecs.Support
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using FluentValidation;

    public class IsTrueValidator : AbstractValidator<SomeRequest>
    {
        public IsTrueValidator()
        {
            this.RuleFor(sr => sr.MustBeTrue).Equal(true);
        }
    }
}
