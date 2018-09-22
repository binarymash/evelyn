namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Threading.Tasks;
    using Projections.AccountProjects;
    using Projections.EnvironmentDetails;
    using Projections.EnvironmentState;
    using Projections.ProjectDetails;
    using Projections.ToggleDetails;

    public interface IReadModelFacade
    {
        Task<AccountProjectsDto> GetProjects(Guid accountId);

        Task<ProjectDetailsDto> GetProjectDetails(Guid projectId);

        Task<EnvironmentDetailsDto> GetEnvironmentDetails(Guid projectId, string environmentKey);

        Task<ToggleDetailsDto> GetToggleDetails(Guid projectId, string toggleKey);

        Task<EnvironmentStateDto> GetEnvironmentState(Guid projectId, string environmentName);
    }
}
