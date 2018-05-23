namespace Evelyn.Core.WriteModel.Project.Commands
{
    using FluentValidation;

    public class AddEnvironmentValidator : AbstractValidator<AddEnvironment>
    {
        public AddEnvironmentValidator()
        {
            RuleFor(command => command.ExpectedProjectVersion)
                .GreaterThanOrEqualTo(0)
                .WithErrorCode(ValidationErrorCodes.ExpectedProjectVersionInvalid);

            RuleFor(command => command.ProjectId)
                .NotEmpty()
                .WithErrorCode(ValidationErrorCodes.ProjectIdNotSet);

            RuleFor(command => command.Key)
                .NotEmpty()
                .WithErrorCode(ValidationErrorCodes.KeyNotSet);

            RuleFor(command => command.Key)
                .MaximumLength(128)
                .WithErrorCode(ValidationErrorCodes.KeyTooLong);

            RuleFor(command => command.Key)
                .Matches(@"^[a-z0-9_-]*$")
                .WithErrorCode(ValidationErrorCodes.KeyHasIncorrectFormat);
        }
    }
}
