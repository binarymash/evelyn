﻿namespace Evelyn.Core.WriteModel.Account.Commands.CreateProject
{
    using FluentValidation;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(command => command.ExpectedVersion)
                .GreaterThanOrEqualTo(0)
                .When(command => command.ExpectedVersion != null)
                .WithErrorCode(ErrorCodes.PropertyOutOfRange);

            RuleFor(command => command.Name)
                .NotEmpty()
                .WithErrorCode(ErrorCodes.PropertyNotSet);

            RuleFor(command => command.Name)
                .MaximumLength(128)
                .WithErrorCode(ErrorCodes.PropertyTooLong);

            RuleFor(command => command.ProjectId)
                .NotEmpty()
                .WithErrorCode(ErrorCodes.PropertyNotSet);
        }
    }
}
