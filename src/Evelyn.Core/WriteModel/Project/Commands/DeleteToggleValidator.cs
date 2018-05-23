namespace Evelyn.Core.WriteModel.Project.Commands
{
    using FluentValidation;

    public class DeleteToggleValidator : AbstractValidator<DeleteToggle>
    {
        public DeleteToggleValidator()
        {
            RuleFor(command => command.ExpectedToggleVersion)
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
