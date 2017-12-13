namespace Evelyn.Api.Rest.Write.Applications
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CQRSlite.Commands;
    using CQRSlite.Domain.Exception;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/applications")]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status500InternalServerError)]
    public class Controller : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ICommandHandler<Core.WriteModel.Commands.CreateApplication> _createApplicationHandler;

        public Controller(ICommandHandler<Core.WriteModel.Commands.CreateApplication> createApplicationHandler)
        {
            _createApplicationHandler = createApplicationHandler;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> Post([FromBody]CreateApplication message)
        {
            // TODO: validation
            try
            {
                var command = new Core.WriteModel.Commands.CreateApplication(message.Id, message.Name);
                await _createApplicationHandler.Handle(command);
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
