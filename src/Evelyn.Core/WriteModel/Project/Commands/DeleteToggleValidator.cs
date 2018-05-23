namespace Evelyn.Core.WriteModel.Project.Commands
{
    using FluentValidation;

    public class DeleteToggleValidator : AbstractValidator<DeleteToggle>
    {
        public DeleteToggleValidator()
        {
            RuleFor(command => command.ExpectedToggleVersion)
                .GreaterThanOrEqualTo(0)
                .WithErrorCode(ValidationErrorCodes.ExpectedToggleVersionInvalid);

            RuleFor(command => command.ProjectId)
                .NotEmpty()
                .WithErrorCode(ValidationErrorCodes.ProjectIdNotSet);

            RuleFor(command => command.Key)
                .NotEmpty()
                .WithErrorCode(ValidationErrorCodes.KeyNotSet);
        }
    }
}
