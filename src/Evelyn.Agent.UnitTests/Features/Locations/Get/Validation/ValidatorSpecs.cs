using Evelyn.Agent.Features.Locations.Get.Model;
using Evelyn.Agent.Features.Locations.Get.Validation;
using FluentValidation.Results;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace Evelyn.Agent.UnitTests.Features.Locations.Get.Validation
{
    public class ValidatorSpecs
    {
        Query query;

        Validator validator;

        ValidationResult result;

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
