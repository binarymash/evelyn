namespace Evelyn.Agent.Features.Locations.Get.Validation
{
    using System;
    using FluentValidation;

    public class Validator : AbstractValidator<Model.Query>, IValidator<Model.Query>
    {
        public Validator()
        {
        }
    }
}
