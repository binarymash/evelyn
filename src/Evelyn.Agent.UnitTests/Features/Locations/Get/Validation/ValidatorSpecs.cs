namespace Evelyn.Agent.UnitTests.Features.Locations.Get.Validation
{
    using Evelyn.Agent.Features.Locations.Get;
    using FluentValidation.Results;
    using Shouldly;
    using TestStack.BDDfy;
    using Xunit;

    public class ValidatorSpecs
    {
        private Query query;

        private Validator validator;

        private ValidationResult result;

        public ValidatorSpecs()
        {
            validator = new Validator();
        }

        [Fact]
        public void QueryIsValid()
        {
            this.Given(_ => GivenAQuery())
                .When(_ => WhenValidated())
                .Then(_ => ThenTheQueryIsValid())
                .BDDfy();
        }

        private void GivenAQuery()
        {
            query = new Query();
        }

        private void WhenValidated()
        {
            result = validator.Validate(query);
        }

        private void ThenTheQueryIsValid()
        {
            result.IsValid.ShouldBeTrue();
        }
    }
}
