namespace Evelyn.Core.WriteModel.Project.Commands.AddToggle
{
    using FluentValidation;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(command => command.ExpectedProjectVersion)
                .GreaterThanOrEqualTo(0)
                .WithErrorCode(ErrorCodes.PropertyOutOfRange);

            RuleFor(command => command.ProjectId)
                .NotEmpty()
                .WithErrorCode(ErrorCodes.PropertyNotSet);

            RuleFor(command => command.Name)
                .NotEmpty()
                .WithErrorCode(ErrorCodes.PropertyNotSet);

            RuleFor(command => command.Name)
                .MaximumLength(128)
                .WithErrorCode(ErrorCodes.PropertyTooLong);

            RuleFor(command => command.Key)
                .NotEmpty()
                .WithErrorCode(ErrorCodes.PropertyNotSet);

            RuleFor(command => command.Key)
                .MaximumLength(128)
                .WithErrorCode(ErrorCodes.PropertyTooLong);

            RuleFor(command => command.Key)
                .Matches(@"^[a-z0-9_-]*$")
                .WithErrorCode(ErrorCodes.PropertyIncorrectFormat);
        }
    }
}
