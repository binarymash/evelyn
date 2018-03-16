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
        private readonly IDatabase<Guid, AccountProjectsDto> _accountProjects;

        private readonly IDatabase<Guid, ProjectDetailsDto> _projectDetails;

        private readonly IDatabase<string, EnvironmentDetailsDto> _environmentDetails;

        private readonly IDatabase<string, ToggleDetailsDto> _toggleDetails;

        private readonly IDatabase<string, EnvironmentStateDto> _environmentStates;

        public DatabaseReadModelFacade(
            IDatabase<Guid, AccountProjectsDto> accountProjects,
            IDatabase<Guid, ProjectDetailsDto> projectDetails,
            IDatabase<string, EnvironmentDetailsDto> environmentDetails,
            IDatabase<string, ToggleDetailsDto> toggleDetails,
            IDatabase<string, EnvironmentStateDto> environmentStates)
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
