namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Threading.Tasks;
    using Evelyn.Core.ReadModel.Projections;

    public class DatabaseReadModelFacade : IReadModelFacade
    {
        private readonly IProjectionStore<Projections.AccountProjects.Projection> _accountProjects;

        private readonly IProjectionStore<Projections.ProjectDetails.Projection> _projectDetails;

        private readonly IProjectionStore<Projections.EnvironmentDetails.Projection> _environmentDetails;

        private readonly IProjectionStore<Projections.ToggleDetails.Projection> _toggleDetails;

        private readonly IProjectionStore<Projections.EnvironmentState.Projection> _environmentStates;

        public DatabaseReadModelFacade(
            IProjectionStore<Projections.AccountProjects.Projection> accountProjects,
            IProjectionStore<Projections.ProjectDetails.Projection> projectDetails,
            IProjectionStore<Projections.EnvironmentDetails.Projection> environmentDetails,
            IProjectionStore<Projections.ToggleDetails.Projection> toggleDetails,
            IProjectionStore<Projections.EnvironmentState.Projection> environmentStates)
        {
            _accountProjects = accountProjects;
            _projectDetails = projectDetails;
            _environmentDetails = environmentDetails;
            _toggleDetails = toggleDetails;
            _environmentStates = environmentStates;
        }

        public async Task<Projections.AccountProjects.Projection> GetProjects(Guid accountId)
        {
            return await _accountProjects.Get(Projections.AccountProjects.Projection.StoreKey(accountId));
        }

        public async Task<Projections.ProjectDetails.Projection> GetProjectDetails(Guid projectId)
        {
            return await _projectDetails.Get(Projections.ProjectDetails.Projection.StoreKey(projectId));
        }

        public async Task<Projections.EnvironmentDetails.Projection> GetEnvironmentDetails(Guid projectId, string environmentKey)
        {
            return await _environmentDetails.Get(Projections.EnvironmentDetails.Projection.StoreKey(projectId, environmentKey));
        }

        public async Task<Projections.ToggleDetails.Projection> GetToggleDetails(Guid projectId, string toggleKey)
        {
            return await _toggleDetails.Get(Projections.ToggleDetails.Projection.StoreKey(projectId, toggleKey));
        }

        public async Task<Projections.EnvironmentState.Projection> GetEnvironmentState(Guid projectId, string environmentName)
        {
            return await _environmentStates.Get(Projections.EnvironmentState.Projection.StoreKey(projectId, environmentName));
        }
    }
}
