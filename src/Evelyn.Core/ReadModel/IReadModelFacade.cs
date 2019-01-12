namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Threading.Tasks;

    public interface IReadModelFacade
    {
        Task<Projections.AccountProjects.Projection> GetProjects(Guid accountId);

        Task<Projections.ProjectDetails.Projection> GetProjectDetails(Guid projectId);

        Task<Projections.EnvironmentDetails.Projection> GetEnvironmentDetails(Guid projectId, string environmentKey);

        Task<Projections.ToggleDetails.Projection> GetToggleDetails(Guid projectId, string toggleKey);

        Task<Projections.EnvironmentState.Projection> GetEnvironmentState(Guid projectId, string environmentName);

        Task<Projections.ToggleState.Projection> GetToggleState(Guid projectId, string toggleKey);

        Task<Projections.ClientEnvironmentState.Projection> GetClientEnvironmentState(Guid projectId, string environmentName);
    }
}
