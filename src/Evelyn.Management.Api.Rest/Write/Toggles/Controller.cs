﻿namespace Evelyn.Management.Api.Rest.Write.Toggles
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

    [Route("api/projects/{projectId}/toggles")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(Response<ValidationError>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Response<Error>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Response<Error>), StatusCodes.Status500InternalServerError)]
    public class Controller : EvelynController
    {
        private readonly ICommandHandler<Core.WriteModel.Project.Commands.AddToggle> _addToggleHandler;
        private readonly ICommandHandler<Core.WriteModel.Project.Commands.DeleteToggle> _deleteToggleHandler;

        public Controller(ICommandHandler<Core.WriteModel.Project.Commands.AddToggle> addToggleHandler, ICommandHandler<Core.WriteModel.Project.Commands.DeleteToggle> deleteToggleHandler)
        {
            _addToggleHandler = addToggleHandler;
            _deleteToggleHandler = deleteToggleHandler;
        }

        [Route("add")]
        [HttpPost]
        public async Task<ObjectResult> Post(Guid projectId, [FromBody]Messages.AddToggle message)
        {
            try
            {
                var command = new Core.WriteModel.Project.Commands.AddToggle(UserId, projectId, message.Key, message.Name, message.ExpectedProjectVersion);
                await _addToggleHandler.Handle(command);
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

        [Route("{toggleKey}/delete")]
        [HttpPost]
        public async Task<ObjectResult> Post(Guid projectId, string toggleKey, [FromBody]Messages.DeleteToggle message)
        {
            try
            {
                var command = new Core.WriteModel.Project.Commands.DeleteToggle(UserId, projectId, toggleKey, message.ExpectedToggleVersion);
                await _deleteToggleHandler.Handle(command);
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
