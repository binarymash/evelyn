namespace Evelyn.Management.Api.Rest.Write.Environments
{
    using System;
    using System.Threading.Tasks;
    using CQRSlite.Commands;
    using CQRSlite.Domain.Exception;
    using FluentValidation;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Responses;

    [Route("api/projects/{projectId}/environments")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(Response<ValidationError>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Response<Error>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Response<Error>), StatusCodes.Status500InternalServerError)]
    public class Controller : EvelynController
    {
        private readonly ICommandHandler<Core.WriteModel.Project.Commands.AddEnvironment.Command> _addHandler;
        private readonly ICommandHandler<Core.WriteModel.Project.Commands.DeleteEnvironment.Command> _deleteHandler;

        public Controller(ICommandHandler<Core.WriteModel.Project.Commands.AddEnvironment.Command> addHandler, ICommandHandler<Core.WriteModel.Project.Commands.DeleteEnvironment.Command> deleteHandler)
        {
            _addHandler = addHandler;
            _deleteHandler = deleteHandler;
        }

        [Route("add")]
        [HttpPost]
        public async Task<ObjectResult> Post(Guid projectId, [FromBody]Messages.AddEnvironment message)
        {
            try
            {
                var command = new Core.WriteModel.Project.Commands.AddEnvironment.Command(UserId, projectId, message.Key, message.Name, message.ExpectedProjectVersion);
                await _addHandler.Handle(command);
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

        [Route("{environmentKey}/delete")]
        [HttpPost]
        public async Task<ObjectResult> Post(Guid projectId, string environmentKey, [FromBody]Messages.DeleteEnvironment message)
        {
            try
            {
                var command = new Core.WriteModel.Project.Commands.DeleteEnvironment.Command(UserId, projectId, environmentKey, message.ExpectedEnvironmentVersion);
                await _deleteHandler.Handle(command);
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
