namespace Evelyn.Management.Api.Rest.Read
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.ReadModel.Projections.ClientEnvironmentState;
    using Evelyn.Core.ReadModel;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("client-api/projects/{projectId}/environments/{environmentName}/state")]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status500InternalServerError)]
    [ApiExplorerSettings(GroupName = "client-api")]
    public class ClientEnvironmentStatesController : Controller
    {
        private readonly IReadModelFacade _readModelFacade;

        public ClientEnvironmentStatesController(IReadModelFacade readModelFacade)
        {
            _readModelFacade = readModelFacade;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Projection), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status404NotFound)]
        public async Task<ObjectResult> Get(Guid projectId, string environmentName)
        {
            try
            {
                var result = await _readModelFacade.GetClientEnvironmentState(projectId, environmentName);
                return Ok(result);
            }
            catch (ProjectionNotFoundException)
            {
                return NotFound(null);
            }
            catch (Exception)
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
