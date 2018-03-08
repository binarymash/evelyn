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
        private readonly IDatabase<string, AccountProjectsDto> _accountProjects;

        private readonly IDatabase<Guid, ProjectDetailsDto> _projectDetails;

        private readonly IDatabase<string, EnvironmentDetailsDto> _environmentDetails;

        private readonly IDatabase<Guid, ToggleDetailsDto> _toggleDetails;

        public DatabaseReadModelFacade(
            IDatabase<string, AccountProjectsDto> accountProjects,
            IDatabase<Guid, ProjectDetailsDto> projectDetails,
            IDatabase<string, EnvironmentDetailsDto> environmentDetails,
            IDatabase<Guid, ToggleDetailsDto> toggleDetails)
        {
            _accountProjects = accountProjects;
            _projectDetails = projectDetails;
            _environmentDetails = environmentDetails;
            _toggleDetails = toggleDetails;
        }

        public async Task<AccountProjectsDto> GetProjects(string accountId)
        {
            try
            {
                return await _accountProjects.Get(accountId);
            }
            catch (NotFoundException)
            {
                // hack! Remove this
                return new AccountProjectsDto(accountId);
            }
        }

        public async Task<ProjectDetailsDto> GetProjectDetails(Guid projectId)
        {
            return await _projectDetails.Get(projectId);
        }

        public async Task<EnvironmentDetailsDto> GetEnvironmentDetails(string environmentKey)
        {
            return await _environmentDetails.Get(environmentKey);
        }

        public async Task<ToggleDetailsDto> GetToggleDetails(Guid toggleId)
        {
            return await _toggleDetails.Get(toggleId);
        }
    }
}
