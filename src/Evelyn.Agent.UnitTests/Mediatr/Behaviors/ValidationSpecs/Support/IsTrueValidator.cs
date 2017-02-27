namespace Evelyn.Agent.UnitTests.Mediatr.Behaviors.ValidationSpecs.Support
{
    using FluentValidation;

    public class IsTrueValidator : AbstractValidator<SomeRequest>
    {
        public IsTrueValidator()
        {
            this.RuleFor(sr => sr.MustBeTrue).Equal(true);
        }
    }
}
