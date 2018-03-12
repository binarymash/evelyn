namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Threading.Tasks;
    using AccountProjects;
    using EnvironmentDetails;
    using ProjectDetails;
    using ToggleDetails;

    public interface IReadModelFacade
    {
        Task<AccountProjectsDto> GetProjects(Guid accountId);

        Task<ProjectDetailsDto> GetProjectDetails(Guid projectId);

        Task<EnvironmentDetailsDto> GetEnvironmentDetails(string environmentKey);

        Task<ToggleDetailsDto> GetToggleDetails(string toggleKey);
    }
}
