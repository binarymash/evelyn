namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Threading.Tasks;
    using Evelyn.Core.ReadModel.AccountProjects;
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using Evelyn.Core.ReadModel.EnvironmentState;
    using Evelyn.Core.ReadModel.Infrastructure;
    using Evelyn.Core.ReadModel.ProjectDetails;
    using Evelyn.Core.ReadModel.ToggleDetails;

    public class DatabaseReadModelFacade : IReadModelFacade
    {
        private readonly IProjectionStore<AccountProjectsDto> _accountProjects;

        private readonly IProjectionStore<ProjectDetailsDto> _projectDetails;

        private readonly IProjectionStore<EnvironmentDetailsDto> _environmentDetails;

        private readonly IProjectionStore<ToggleDetailsDto> _toggleDetails;

        private readonly IProjectionStore<EnvironmentStateDto> _environmentStates;

        public DatabaseReadModelFacade(
            IProjectionStore<AccountProjectsDto> accountProjects,
            IProjectionStore<ProjectDetailsDto> projectDetails,
            IProjectionStore<EnvironmentDetailsDto> environmentDetails,
            IProjectionStore<ToggleDetailsDto> toggleDetails,
            IProjectionStore<EnvironmentStateDto> environmentStates)
        {
            _accountProjects = accountProjects;
            _projectDetails = projectDetails;
            _environmentDetails = environmentDetails;
            _toggleDetails = toggleDetails;
            _environmentStates = environmentStates;
        }

        public async Task<AccountProjectsDto> GetProjects(Guid accountId)
        {
            return await _accountProjects.Get(AccountProjectsDto.StoreKey(accountId));
        }

        public async Task<ProjectDetailsDto> GetProjectDetails(Guid projectId)
        {
            return await _projectDetails.Get(ProjectDetailsDto.StoreKey(projectId));
        }

        public async Task<EnvironmentDetailsDto> GetEnvironmentDetails(Guid projectId, string environmentKey)
        {
            return await _environmentDetails.Get(EnvironmentDetailsDto.StoreKey(projectId, environmentKey));
        }

        public async Task<ToggleDetailsDto> GetToggleDetails(Guid projectId, string toggleKey)
        {
            return await _toggleDetails.Get(ToggleDetailsDto.StoreKey(projectId, toggleKey));
        }

        public async Task<EnvironmentStateDto> GetEnvironmentState(Guid projectId, string environmentName)
        {
            return await _environmentStates.Get(EnvironmentStateDto.StoreKey(projectId, environmentName));
        }
    }
}
