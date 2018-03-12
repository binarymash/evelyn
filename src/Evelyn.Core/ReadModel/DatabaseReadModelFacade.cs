namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Threading.Tasks;
    using AccountProjects;
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using Evelyn.Core.ReadModel.Infrastructure;
    using Evelyn.Core.ReadModel.ToggleDetails;
    using ProjectDetails;

    public class DatabaseReadModelFacade : IReadModelFacade
    {
        private readonly IDatabase<Guid, AccountProjectsDto> _accountProjects;

        private readonly IDatabase<Guid, ProjectDetailsDto> _projectDetails;

        private readonly IDatabase<string, EnvironmentDetailsDto> _environmentDetails;

        private readonly IDatabase<string, ToggleDetailsDto> _toggleDetails;

        public DatabaseReadModelFacade(
            IDatabase<Guid, AccountProjectsDto> accountProjects,
            IDatabase<Guid, ProjectDetailsDto> projectDetails,
            IDatabase<string, EnvironmentDetailsDto> environmentDetails,
            IDatabase<string, ToggleDetailsDto> toggleDetails)
        {
            _accountProjects = accountProjects;
            _projectDetails = projectDetails;
            _environmentDetails = environmentDetails;
            _toggleDetails = toggleDetails;
        }

        public async Task<AccountProjectsDto> GetProjects(Guid accountId)
        {
            return await _accountProjects.Get(accountId);
        }

        public async Task<ProjectDetailsDto> GetProjectDetails(Guid projectId)
        {
            return await _projectDetails.Get(projectId);
        }

        public async Task<EnvironmentDetailsDto> GetEnvironmentDetails(string environmentKey)
        {
            return await _environmentDetails.Get(environmentKey);
        }

        public async Task<ToggleDetailsDto> GetToggleDetails(string toggleKey)
        {
            return await _toggleDetails.Get(toggleKey);
        }
    }
}
