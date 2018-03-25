namespace Evelyn.Management.Api.Rest.Write.ToggleStates
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CQRSlite.Commands;
    using CQRSlite.Domain.Exception;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/projects/{projectId}/environments/{environmentKey}/toggles/{toggleKey}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status500InternalServerError)]
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
            // TODO: validation
            try
            {
                var command = new Core.WriteModel.Project.Commands.ChangeToggleState(UserId, projectId, environmentKey, toggleKey, message.State, message.ExpectedToggleStateVersion);
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
