namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using Evelyn.Core.ReadModel.ToggleDetails;
    using ProjectDetails;
    using ProjectList;

    public interface IReadModelFacade
    {
        Task<IEnumerable<ProjectListDto>> GetProjects();

        Task<ProjectDetailsDto> GetProjectDetails(Guid projectId);

        Task<EnvironmentDetailsDto> GetEnvironmentDetails(Guid environmentId);

        Task<ToggleDetailsDto> GetToggleDetails(Guid toggleId);
    }
}
