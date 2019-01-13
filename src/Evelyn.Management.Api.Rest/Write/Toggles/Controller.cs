namespace Evelyn.Management.Api.Rest.Write.Toggles
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

    [Route("management-api/projects/{projectId}/toggles")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [ApiExplorerSettings(GroupName = "management-api")]
    public class Controller : EvelynController
    {
        private readonly ICommandHandler<Core.WriteModel.Project.Commands.AddToggle.Command> _addToggleHandler;
        private readonly ICommandHandler<Core.WriteModel.Project.Commands.DeleteToggle.Command> _deleteToggleHandler;

        public Controller(ILogger<Controller> logger, ICommandHandler<Core.WriteModel.Project.Commands.AddToggle.Command> addToggleHandler, ICommandHandler<Core.WriteModel.Project.Commands.DeleteToggle.Command> deleteToggleHandler)
            : base(logger)
        {
            _addToggleHandler = addToggleHandler;
            _deleteToggleHandler = deleteToggleHandler;
        }

        [Route("add")]
        [HttpPost]
        public async Task<ObjectResult> Post(Guid projectId, [FromBody]Messages.AddToggle message)
        {
            try
            {
                var command = new Core.WriteModel.Project.Commands.AddToggle.Command(UserId, projectId, message.Key, message.Name, message.ExpectedProjectVersion);
                await _addToggleHandler.Handle(command);
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

        [Route("{toggleKey}/delete")]
        [HttpPost]
        public async Task<ObjectResult> Post(Guid projectId, string toggleKey, [FromBody]Messages.DeleteToggle message)
        {
            try
            {
                var command = new Core.WriteModel.Project.Commands.DeleteToggle.Command(UserId, projectId, toggleKey, message.ExpectedToggleVersion);
                await _deleteToggleHandler.Handle(command);
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
