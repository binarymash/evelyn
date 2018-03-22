namespace Evelyn.Management.Api.Rest.Read
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.ReadModel.ProjectDetails;
    using Evelyn.Core.ReadModel;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Write;

    [Route("api/projects")]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status500InternalServerError)]
    public class ProjectsController : EvelynController
    {
        private readonly IReadModelFacade _readModelFacade;

        public ProjectsController(IReadModelFacade readModelFacade)
        {
            _readModelFacade = readModelFacade;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProjectDetailsDto>), 200)]
        public async Task<ObjectResult> Get()
        {
            try
            {
                var result = await _readModelFacade.GetProjects(AccountId);
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

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProjectDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status404NotFound)]
        public async Task<ObjectResult> Get(Guid id)
        {
            try
            {
                var projectDetails = await _readModelFacade.GetProjectDetails(id);
                return Ok(projectDetails);
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