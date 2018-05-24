namespace Evelyn.Core.WriteModel.Project.Commands.DeleteToggle
{
    using FluentValidation;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
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
