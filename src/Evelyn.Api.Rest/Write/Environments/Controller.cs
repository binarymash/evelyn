﻿namespace Evelyn.Api.Rest.Write.Environments
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CQRSlite.Commands;
    using CQRSlite.Domain.Exception;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/applications/{applicationId}/environments")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status500InternalServerError)]
    public class Controller : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ICommandHandler<Core.WriteModel.Commands.AddEnvironment> _handler;

        public Controller(ICommandHandler<Core.WriteModel.Commands.AddEnvironment> handler)
        {
            _handler = handler;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Guid applicationId, [FromBody]Messages.AddEnvironment message)
        {
            // TODO: validation
            try
            {
                var command = new Core.WriteModel.Commands.AddEnvironment(applicationId, message.Id, message.Name, message.Key);
                command.ExpectedVersion = message.ExpectedVersion;
                await _handler.Handle(command);
                return Accepted();
            }
            catch (ConcurrencyException)
            {
                // TODO: error handling
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
