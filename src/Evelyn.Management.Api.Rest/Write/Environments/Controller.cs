namespace Evelyn.Management.Api.Rest.Write.Environments
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CQRSlite.Commands;
    using CQRSlite.Domain.Exception;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/projects/{projectId}/environments")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status500InternalServerError)]
    public class Controller : EvelynController
    {
        private readonly ICommandHandler<Core.WriteModel.Project.Commands.AddEnvironment> _addHandler;
        private readonly ICommandHandler<Core.WriteModel.Project.Commands.DeleteEnvironment> _deleteHandler;

        public Controller(ICommandHandler<Core.WriteModel.Project.Commands.AddEnvironment> addHandler, ICommandHandler<Core.WriteModel.Project.Commands.DeleteEnvironment> deleteHandler)
        {
            _addHandler = addHandler;
            _deleteHandler = deleteHandler;
        }

        [Route("add")]
        [HttpPost]
        public async Task<ObjectResult> Post(Guid projectId, [FromBody]Messages.AddEnvironment message)
        {
            // TODO: validation
            try
            {
                var command = new Core.WriteModel.Project.Commands.AddEnvironment(UserId, projectId, message.Key, message.ExpectedProjectVersion);
                await _addHandler.Handle(command);
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

        [Route("{environmentKey}/delete")]
        [HttpPost]
        public async Task<ObjectResult> Post(Guid projectId, string environmentKey, [FromBody]Messages.DeleteEnvironment message)
        {
            // TODO: validation
            try
            {
                var command = new Core.WriteModel.Project.Commands.DeleteEnvironment(UserId, projectId, environmentKey, message.ExpectedEnvironmentVersion);
                await _deleteHandler.Handle(command);
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
