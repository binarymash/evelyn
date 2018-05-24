namespace Evelyn.Management.Api.Rest.Write.Projects
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CQRSlite.Commands;
    using CQRSlite.Domain.Exception;
    using FluentValidation;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Responses;

    [Route("api/projects")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(Response<ValidationError>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Response<Error>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Response<Error>), StatusCodes.Status500InternalServerError)]
    public class Controller : EvelynController
    {
        private readonly ICommandHandler<Core.WriteModel.Account.Commands.CreateProject.Command> _handler;

        public Controller(ICommandHandler<Core.WriteModel.Account.Commands.CreateProject.Command> handler)
        {
            _handler = handler;
        }

        [Route("create")]
        [HttpPost]
        public async Task<ObjectResult> Post([FromBody]Messages.CreateProject message)
        {
            try
            {
                var command = new Core.WriteModel.Account.Commands.CreateProject.Command(UserId, AccountId, message.ProjectId, message.Name);
                await _handler.Handle(command);
                return Accepted();
            }
            catch (ValidationException ex)
            {
                return HandleValidationException(ex);
            }
            catch (ConcurrencyException ex)
            {
                return HandleConcurrencyException(ex);
            }
            catch (Exception ex)
            {
                return HandleInternalError(ex);
            }
        }
    }
}
