namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Threading.Tasks;
    using AccountProjects;
    using EnvironmentDetails;
    using EnvironmentState;
    using ProjectDetails;
    using ToggleDetails;

    public interface IReadModelFacade
    {
        Task<AccountProjectsDto> GetProjects(Guid accountId);

        Task<ProjectDetailsDto> GetProjectDetails(Guid projectId);

        Task<EnvironmentDetailsDto> GetEnvironmentDetails(Guid projectId, string environmentKey);

        Task<ToggleDetailsDto> GetToggleDetails(Guid projectId, string toggleKey);

        Task<EnvironmentStateDto> GetEnvironmentState(Guid projectId, string environmentName);
    }
}
