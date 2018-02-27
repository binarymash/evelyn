﻿namespace Evelyn.Management.Api.Rest.Write.ToggleStates
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CQRSlite.Commands;
    using CQRSlite.Domain.Exception;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/applications/{applicationId}/toggleState")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status500InternalServerError)]
    public class Controller : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ICommandHandler<Core.WriteModel.Commands.ChangeToggleState> _handler;

        public Controller(ICommandHandler<Core.WriteModel.Commands.ChangeToggleState> handler)
        {
            _handler = handler;
        }

        [HttpPost]
        public async Task<ObjectResult> Post(Guid applicationId, [FromBody]Messages.ChangeToggleState message)
        {
            // TODO: validation
            try
            {
                var command = new Core.WriteModel.Commands.ChangeToggleState(applicationId, message.EnvironmentId, message.ToggleId, message.State, message.ExpectedVersion);
                await _handler.Handle(command);
                return Accepted();
            }
            catch (ConcurrencyException ex)
            {
                // TODO: error handling
                return new BadRequestObjectResult(ex.Message);
            }
            catch (Exception)
            {
                // TODO: error handling
                return new ObjectResult(null) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
