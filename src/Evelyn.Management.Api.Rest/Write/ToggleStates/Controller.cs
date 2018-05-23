﻿namespace Evelyn.Management.Api.Rest.Write.ToggleStates
{
    using System;
    using System.Threading.Tasks;
    using CQRSlite.Commands;
    using CQRSlite.Domain.Exception;
    using FluentValidation;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Responses;

    [Route("api/projects/{projectId}/environments/{environmentKey}/toggles/{toggleKey}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(Response<ValidationError>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Response<Error>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Response<Error>), StatusCodes.Status500InternalServerError)]
    public class Controller : EvelynController
    {
        private readonly ICommandHandler<Core.WriteModel.Project.Commands.ChangeToggleState> _handler;

        public Controller(ICommandHandler<Core.WriteModel.Project.Commands.ChangeToggleState> handler)
        {
            _handler = handler;
        }

        [Route("change-state")]
        [HttpPost]
        public async Task<ObjectResult> Post(Guid projectId, string environmentKey, string toggleKey, [FromBody]Messages.ChangeToggleState message)
        {
            try
            {
                var command = new Core.WriteModel.Project.Commands.ChangeToggleState(UserId, projectId, environmentKey, toggleKey, message.State, message.ExpectedToggleStateVersion);
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
