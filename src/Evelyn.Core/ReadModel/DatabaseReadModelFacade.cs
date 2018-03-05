namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using Evelyn.Core.ReadModel.Infrastructure;
    using Evelyn.Core.ReadModel.ToggleDetails;
    using ProjectDetails;
    using ProjectList;

    public class DatabaseReadModelFacade : IReadModelFacade
    {
        private readonly IDatabase<ProjectListDto> _projects;

        private readonly IDatabase<ProjectDetailsDto> _projectDetails;

        private readonly IDatabase<EnvironmentDetailsDto> _environmentDetails;

        private readonly IDatabase<ToggleDetailsDto> _toggleDetails;

        public DatabaseReadModelFacade(
            IDatabase<ProjectListDto> projects,
            IDatabase<ProjectDetailsDto> projectDetails,
            IDatabase<EnvironmentDetailsDto> environmentDetails,
            IDatabase<ToggleDetailsDto> toggleDetails)
        {
            _projects = projects;
            _projectDetails = projectDetails;
            _environmentDetails = environmentDetails;
            _toggleDetails = toggleDetails;
        }

        public async Task<IEnumerable<ProjectListDto>> GetProjects()
        {
            return await _projects.Get();
        }

        public async Task<ProjectDetailsDto> GetProjectDetails(Guid projectId)
        {
            return await _projectDetails.Get(projectId);
        }

        public async Task<EnvironmentDetailsDto> GetEnvironmentDetails(Guid environmentId)
        {
            return await _environmentDetails.Get(environmentId);
        }

        public async Task<ToggleDetailsDto> GetToggleDetails(Guid toggleId)
        {
            return await _toggleDetails.Get(toggleId);
        }
    }
}
