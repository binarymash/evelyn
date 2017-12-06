namespace Evelyn.Api.Rest.Write
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CQRSlite.Commands;
    using CQRSlite.Domain.Exception;
    using Evelyn.Core.WriteModel.Commands;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/applications")]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status500InternalServerError)]
    public class ApplicationsController : Controller
    {
        private readonly ICommandHandler<CreateApplication> _createApplicationHandler;

        public ApplicationsController(ICommandHandler<CreateApplication> createApplicationHandler)
        {
            _createApplicationHandler = createApplicationHandler;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> Post([FromBody]CreateApplication command)
        {
            // TODO: validation
            try
            {
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
