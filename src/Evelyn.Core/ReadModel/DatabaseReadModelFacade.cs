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
        private readonly IProjectionStore<Guid, AccountProjectsDto> _accountProjects;

        private readonly IProjectionStore<Guid, ProjectDetailsDto> _projectDetails;

        private readonly IProjectionStore<string, EnvironmentDetailsDto> _environmentDetails;

        private readonly IProjectionStore<string, ToggleDetailsDto> _toggleDetails;

        private readonly IProjectionStore<string, EnvironmentStateDto> _environmentStates;

        public DatabaseReadModelFacade(
            IProjectionStore<Guid, AccountProjectsDto> accountProjects,
            IProjectionStore<Guid, ProjectDetailsDto> projectDetails,
            IProjectionStore<string, EnvironmentDetailsDto> environmentDetails,
            IProjectionStore<string, ToggleDetailsDto> toggleDetails,
            IProjectionStore<string, EnvironmentStateDto> environmentStates)
        {
            _accountProjects = accountProjects;
            _projectDetails = projectDetails;
            _environmentDetails = environmentDetails;
            _toggleDetails = toggleDetails;
            _environmentStates = environmentStates;
        }

        public async Task<AccountProjectsDto> GetProjects(Guid accountId)
        {
            return await _accountProjects.Get(accountId);
        }

        public async Task<ProjectDetailsDto> GetProjectDetails(Guid projectId)
        {
            return await _projectDetails.Get(projectId);
        }

        public async Task<EnvironmentDetailsDto> GetEnvironmentDetails(Guid projectId, string environmentKey)
        {
            return await _environmentDetails.Get($"{projectId}-{environmentKey}");
        }

        public async Task<ToggleDetailsDto> GetToggleDetails(Guid projectId, string toggleKey)
        {
            return await _toggleDetails.Get($"{projectId}-{toggleKey}");
        }

        public async Task<EnvironmentStateDto> GetEnvironmentState(Guid projectId, string environmentName)
        {
            return await _environmentStates.Get($"{projectId}-{environmentName}");
        }
    }
}
