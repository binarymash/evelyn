namespace Evelyn.Core.WriteModel.Project.Commands
{
    using FluentValidation;

    public class DeleteEnvironmentValidator : AbstractValidator<DeleteEnvironment>
    {
        public DeleteEnvironmentValidator()
        {
            RuleFor(command => command.ExpectedEnvironmentVersion)
                .GreaterThanOrEqualTo(0)
                .WithErrorCode(ErrorCodes.PropertyOutOfRange);

            RuleFor(command => command.ProjectId)
                .NotEmpty()
                .WithErrorCode(ErrorCodes.PropertyNotSet);

            RuleFor(command => command.Key)
                .NotEmpty()
                .WithErrorCode(ErrorCodes.PropertyNotSet);
        }
    }
}
