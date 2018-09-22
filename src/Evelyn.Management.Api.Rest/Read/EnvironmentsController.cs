namespace Evelyn.Management.Api.Rest.Read
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.Projections.EnvironmentDetails;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/projects/{projectId}/environments")]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status500InternalServerError)]
    public class EnvironmentsController : Controller
    {
        private readonly IReadModelFacade _readModelFacade;

        public EnvironmentsController(IReadModelFacade readModelFacade)
        {
            _readModelFacade = readModelFacade;
        }

        [HttpGet("{environmentKey}")]
        [ProducesResponseType(typeof(EnvironmentDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status404NotFound)]
        public async Task<ObjectResult> Get(Guid projectId, string environmentKey)
        {
            try
            {
                var result = await _readModelFacade.GetEnvironmentDetails(projectId, environmentKey);
                return Ok(result);
            }
            catch (NotFoundException)
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
