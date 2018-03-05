namespace Evelyn.Management.Api.Rest.Write.Projects
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CQRSlite.Commands;
    using CQRSlite.Domain.Exception;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/projects")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status500InternalServerError)]
    public class Controller : EvelynController
    {
        private readonly ICommandHandler<Core.WriteModel.Commands.CreateProject> _handler;

        public Controller(ICommandHandler<Core.WriteModel.Commands.CreateProject> handler)
        {
            _handler = handler;
        }

        [HttpPost]
        public async Task<ObjectResult> Post([FromBody]Messages.CreateProject message)
        {
            // TODO: validation
            try
            {
                var command = new Core.WriteModel.Commands.CreateProject(UserId, AccountId, message.Id, message.Name);
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
