namespace Evelyn.Management.Api.Rest.Read
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.ReadModel.EnvironmentState;
    using Evelyn.Core.ReadModel;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/states/{projectId}/{environmentName}")]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status500InternalServerError)]
    public class EnvironmentStatesController : Controller
    {
        private readonly IReadModelFacade _readModelFacade;

        public EnvironmentStatesController(IReadModelFacade readModelFacade)
        {
            _readModelFacade = readModelFacade;
        }

        [HttpGet]
        [ProducesResponseType(typeof(EnvironmentStateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status404NotFound)]
        public async Task<ObjectResult> Get(Guid projectId, string environmentName)
        {
            try
            {
                var result = await _readModelFacade.GetEnvironmentState(projectId, environmentName);
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
