namespace Evelyn.Core.WriteModel.Project.Commands
{
    using FluentValidation;

    public class ChangeToggleStateValidator : AbstractValidator<ChangeToggleState>
    {
        public ChangeToggleStateValidator()
        {
            RuleFor(command => command.ProjectId)
                .NotEmpty()
                .WithErrorCode(ValidationErrorCodes.ProjectIdNotSet);

            RuleFor(command => command.ExpectedToggleStateVersion)
                .GreaterThanOrEqualTo(0)
                .WithErrorCode(ValidationErrorCodes.ExpectedToggleStateVersionInvalid);

            RuleFor(command => command.ToggleKey)
                .NotEmpty()
                .WithErrorCode(ValidationErrorCodes.KeyNotSet);

            RuleFor(command => command.EnvironmentKey)
                .NotEmpty()
                .WithErrorCode(ValidationErrorCodes.KeyNotSet);

            RuleFor(command => command.Value)
                .NotEmpty()
                .WithErrorCode(ValidationErrorCodes.ValueNotSet);

            RuleFor(command => command.Value)
                .Must(a => a == bool.TrueString || a == bool.FalseString)
                .WithErrorCode(ValidationErrorCodes.ValueHasIncorrectFormat)
                .WithMessage("'{PropertyName}' is not in the correct format.");

        }
    }
}
