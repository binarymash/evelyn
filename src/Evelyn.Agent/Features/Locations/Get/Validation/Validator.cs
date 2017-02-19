namespace Evelyn.Agent.Features.Locations.Get.Validation
{
    using FluentValidation;
    using System;

    public class Validator : AbstractValidator<Model.Query>, IValidator<Model.Query>
    {
        public Validator()
        {
        }
    }
}
