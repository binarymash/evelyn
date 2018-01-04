namespace Evelyn.Management.Api.Rest.Read
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Evelyn.Core.ReadModel;
    using Evelyn.Core.ReadModel.ApplicationDetails;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/applications")]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status500InternalServerError)]
    public class ApplicationsController : Controller
    {
        private readonly IReadModelFacade _readModelFacade;

        public ApplicationsController(IReadModelFacade readModelFacade)
        {
            _readModelFacade = readModelFacade;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ApplicationDetailsDto>), 200)]
        public async Task<ObjectResult> Get()
        {
            try
            {
                var result = await _readModelFacade.GetApplications();
                return Ok(result);
            }
            catch (Exception)
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApplicationDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status404NotFound)]
        public async Task<ObjectResult> Get(Guid id)
        {
            try
            {
                var applicationDetails = await _readModelFacade.GetApplicationDetails(id);
                return Ok(applicationDetails);
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