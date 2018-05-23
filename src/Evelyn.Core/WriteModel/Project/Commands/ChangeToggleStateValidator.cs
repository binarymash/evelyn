namespace Evelyn.Core.WriteModel.Project.Commands
{
    using FluentValidation;

    public class ChangeToggleStateValidator : AbstractValidator<ChangeToggleState>
    {
        public ChangeToggleStateValidator()
        {
            RuleFor(command => command.ProjectId)
                .NotEmpty()
                .WithErrorCode(ErrorCodes.PropertyNotSet);

            RuleFor(command => command.ExpectedToggleStateVersion)
                .GreaterThanOrEqualTo(0)
                .WithErrorCode(ErrorCodes.PropertyOutOfRange);

            RuleFor(command => command.ToggleKey)
                .NotEmpty()
                .WithErrorCode(ErrorCodes.PropertyNotSet);

            RuleFor(command => command.EnvironmentKey)
                .NotEmpty()
                .WithErrorCode(ErrorCodes.PropertyNotSet);

            RuleFor(command => command.Value)
                .NotEmpty()
                .WithErrorCode(ErrorCodes.PropertyNotSet);

            RuleFor(command => command.Value)
                .Must(a => a == bool.TrueString || a == bool.FalseString)
                .WithErrorCode(ErrorCodes.PropertyIncorrectFormat)
                .WithMessage("'{PropertyName}' is not in the correct format.");
        }
    }
}
