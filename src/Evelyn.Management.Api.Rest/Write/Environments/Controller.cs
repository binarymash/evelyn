namespace Evelyn.Management.Api.Rest.Write.Environments
{
    using System;
    using System.Threading.Tasks;
    using CQRSlite.Commands;
    using Evelyn.Core.WriteModel;
    using FluentValidation;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Responses;

    [Route("management-api/projects/{projectId}/environments")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [ApiExplorerSettings(GroupName = "management-api")]
    public class Controller : EvelynController
    {
        private readonly ICommandHandler<Core.WriteModel.Project.Commands.AddEnvironment.Command> _addHandler;
        private readonly ICommandHandler<Core.WriteModel.Project.Commands.DeleteEnvironment.Command> _deleteHandler;

        public Controller(ILogger<Controller> logger, ICommandHandler<Core.WriteModel.Project.Commands.AddEnvironment.Command> addHandler, ICommandHandler<Core.WriteModel.Project.Commands.DeleteEnvironment.Command> deleteHandler)
            : base(logger)
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
        [ApiExplorerSettings(GroupName = "management-api")]
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
