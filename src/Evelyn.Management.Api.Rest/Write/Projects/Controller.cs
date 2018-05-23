namespace Evelyn.Management.Api.Rest.Write.Projects
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CQRSlite.Commands;
    using CQRSlite.Domain.Exception;
    using FluentValidation;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/projects")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status500InternalServerError)]
    public class Controller : EvelynController
    {
        private readonly ICommandHandler<Core.WriteModel.Account.Commands.CreateProject> _handler;

        public Controller(ICommandHandler<Core.WriteModel.Account.Commands.CreateProject> handler)
        {
            _handler = handler;
        }

        [Route("create")]
        [HttpPost]
        public async Task<ObjectResult> Post([FromBody]Messages.CreateProject message)
        {
            // TODO: validation
            try
            {
                var command = new Core.WriteModel.Account.Commands.CreateProject(UserId, AccountId, message.ProjectId, message.Name);
                await _handler.Handle(command);
                return Accepted();
            }
            catch (ValidationException ex)
            {
                return new BadRequestObjectResult(ex);
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
