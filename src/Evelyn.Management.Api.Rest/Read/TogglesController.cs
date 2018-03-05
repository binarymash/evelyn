namespace Evelyn.Management.Api.Rest.Read
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.ToggleDetails;
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

        [HttpGet("{toggleId}")]
        [ProducesResponseType(typeof(ToggleDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status404NotFound)]
        public async Task<ObjectResult> Get(Guid toggleId)
        {
            try
            {
                var result = await _readModelFacade.GetToggleDetails(toggleId);
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
