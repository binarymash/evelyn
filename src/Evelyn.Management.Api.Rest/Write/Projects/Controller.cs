namespace Evelyn.Management.Api.Rest.Write.Projects
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

    [Route("api/projects")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(Response<ValidationError>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Response<Error>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Response<Error>), StatusCodes.Status500InternalServerError)]
    public class Controller : EvelynController
    {
        private readonly ICommandHandler<Core.WriteModel.Account.Commands.CreateProject.Command> _createHandler;
        private readonly ICommandHandler<Core.WriteModel.Project.Commands.DeleteProject.Command> _deleteHandler;

        public Controller(ILogger<Controller> logger, ICommandHandler<Core.WriteModel.Account.Commands.CreateProject.Command> createHandler, ICommandHandler<Core.WriteModel.Project.Commands.DeleteProject.Command> deleteHandler)
            : base(logger)
        {
            _createHandler = createHandler;
            _deleteHandler = deleteHandler;
        }

        [Route("create")]
        [HttpPost]
        public async Task<ObjectResult> Post([FromBody]Messages.CreateProject message)
        {
            try
            {
                var command = new Core.WriteModel.Account.Commands.CreateProject.Command(UserId, AccountId, message.ProjectId, message.Name);
                await _createHandler.Handle(command);
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

        [Route("{projectId}/delete")]
        [HttpPost]
        public async Task<ObjectResult> Post(Guid projectId, [FromBody]Messages.DeleteProject message)
        {
            try
            {
                var command = new Core.WriteModel.Project.Commands.DeleteProject.Command(UserId, AccountId, projectId, message.ExpectedProjectVersion);
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
