namespace Evelyn.Management.Api.Rest.Write.ToggleStates
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

    [Route("management-api/projects/{projectId}/environments/{environmentKey}/toggles/{toggleKey}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [ApiExplorerSettings(GroupName = "management-api")]
    public class Controller : EvelynController
    {
        private readonly ICommandHandler<Core.WriteModel.Project.Commands.ChangeToggleState.Command> _handler;

        public Controller(ILogger<Controller> logger, ICommandHandler<Core.WriteModel.Project.Commands.ChangeToggleState.Command> handler)
            : base(logger)
        {
            _handler = handler;
        }

        [Route("change-state")]
        [HttpPost]
        public async Task<ObjectResult> Post(Guid projectId, string environmentKey, string toggleKey, [FromBody]Messages.ChangeToggleState message)
        {
            try
            {
                var command = new Core.WriteModel.Project.Commands.ChangeToggleState.Command(UserId, projectId, environmentKey, toggleKey, message.State, message.ExpectedToggleStateVersion);
                await _handler.Handle(command);
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
