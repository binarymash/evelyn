namespace Evelyn.Core.WriteModel.Project.Commands
{
    using FluentValidation;

    public class DeleteEnvironmentValidator : AbstractValidator<DeleteEnvironment>
    {
        public DeleteEnvironmentValidator()
        {
            RuleFor(command => command.ExpectedEnvironmentVersion)
                .GreaterThanOrEqualTo(0)
                .WithErrorCode(ValidationErrorCodes.ExpectedEnvironmentVersionInvalid);

            RuleFor(command => command.ProjectId)
                .NotEmpty()
                .WithErrorCode(ValidationErrorCodes.ProjectIdNotSet);

            RuleFor(command => command.Key)
                .NotEmpty()
                .WithErrorCode(ValidationErrorCodes.KeyNotSet);
        }
    }
}
