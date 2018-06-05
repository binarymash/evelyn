namespace Evelyn.Core.WriteModel.Project.Commands.DeleteProject
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
        }
    }
}
