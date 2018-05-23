namespace Evelyn.Core.WriteModel.Account.Commands
{
    using FluentValidation;

    public class CreateProjectValidator : AbstractValidator<CreateProject>
    {
        public CreateProjectValidator()
        {
            RuleFor(command => command.ExpectedVersion)
                .GreaterThanOrEqualTo(0)
                .When(command => command.ExpectedVersion != null)
                .WithErrorCode(ValidationErrorCodes.ExpectedVersionInvalid);

            RuleFor(command => command.Name)
                .NotEmpty()
                .WithErrorCode(ValidationErrorCodes.NameNotSet);

            RuleFor(command => command.Name)
                .MaximumLength(128)
                .WithErrorCode(ValidationErrorCodes.NameTooLong);

            RuleFor(command => command.ProjectId)
                .NotEmpty()
                .WithErrorCode(ValidationErrorCodes.ProjectIdNotSet);
        }
    }
}
