namespace Evelyn.Management.Api.Rest.Read
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.Projections.ToggleDetails;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/projects/{projectId}/toggles")]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status500InternalServerError)]
    public class TogglesController : Controller
    {
        private readonly IReadModelFacade _readModelFacade;

        public TogglesController(IReadModelFacade readModelFacade)
        {
            _readModelFacade = readModelFacade;
        }

        [HttpGet("{toggleKey}")]
        [ProducesResponseType(typeof(ToggleDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status404NotFound)]
        public async Task<ObjectResult> Get(Guid projectId, string toggleKey)
        {
            try
            {
                var result = await _readModelFacade.GetToggleDetails(projectId, toggleKey);
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
