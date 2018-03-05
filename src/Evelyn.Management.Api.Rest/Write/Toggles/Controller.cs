namespace Evelyn.Management.Api.Rest.Write.Toggles
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CQRSlite.Commands;
    using CQRSlite.Domain.Exception;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/applications/{applicationId}/toggles")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status500InternalServerError)]
    public class Controller : EvelynController
    {
        private readonly ICommandHandler<Core.WriteModel.Commands.AddToggle> _handler;

        public Controller(ICommandHandler<Core.WriteModel.Commands.AddToggle> handler)
        {
            _handler = handler;
        }

        [HttpPost]
        public async Task<ObjectResult> Post(Guid applicationId, [FromBody]Toggles.Messages.AddToggle message)
        {
            // TODO: validation
            try
            {
                var command = new Core.WriteModel.Commands.AddToggle(UserId, applicationId, message.Id, message.Name, message.Key, message.ExpectedVersion);
                await _handler.Handle(command);
                return Accepted();
            }
            catch (ConcurrencyException)
            {
                // TODO: error handling
                var value = new Dictionary<string, string>();
                return new BadRequestObjectResult(value);
            }
            catch (Exception)
            {
                // TODO: error handling
                return new ObjectResult(null) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
