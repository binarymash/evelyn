namespace Evelyn.Management.Api.Rest.Read
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Evelyn.Core.ReadModel;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("management-api/projects/{projectId}/environments/{environmentKey}")]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status500InternalServerError)]
    [ApiExplorerSettings(GroupName = "management-api")]
    public class EnvironmentsController : Controller
    {
        private readonly IReadModelFacade _readModelFacade;

        public EnvironmentsController(IReadModelFacade readModelFacade)
        {
            _readModelFacade = readModelFacade;
        }

        [HttpGet("definition")]
        [ProducesResponseType(typeof(Evelyn.Core.ReadModel.Projections.EnvironmentDetails.Projection), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status404NotFound)]
        public async Task<ObjectResult> GetDefinition(Guid projectId, string environmentKey)
        {
            try
            {
                var result = await _readModelFacade.GetEnvironmentDetails(projectId, environmentKey);
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

        [HttpGet("state")]
        [ProducesResponseType(typeof(Evelyn.Core.ReadModel.Projections.EnvironmentState.Projection), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status404NotFound)]
        public async Task<ObjectResult> GetState(Guid projectId, string environmentKey)
        {
            try
            {
                var result = await _readModelFacade.GetEnvironmentState(projectId, environmentKey);
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
