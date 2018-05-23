namespace Evelyn.Core.WriteModel.Project.Commands
{
    using FluentValidation;

    public class AddEnvironmentValidator : AbstractValidator<AddEnvironment>
    {
        public AddEnvironmentValidator()
        {
            RuleFor(command => command.ExpectedProjectVersion)
                .GreaterThanOrEqualTo(0)
                .WithErrorCode(ErrorCodes.PropertyOutOfRange);

            RuleFor(command => command.ProjectId)
                .NotEmpty()
                .WithErrorCode(ErrorCodes.PropertyNotSet);

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
