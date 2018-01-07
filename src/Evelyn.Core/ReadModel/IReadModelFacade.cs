namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Evelyn.Core.ReadModel.ApplicationDetails;
    using Evelyn.Core.ReadModel.ApplicationList;
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using Evelyn.Core.ReadModel.ToggleDetails;

    public interface IReadModelFacade
    {
        Task<IEnumerable<ApplicationListDto>> GetApplications();

        Task<ApplicationDetailsDto> GetApplicationDetails(Guid applicationId);

        Task<EnvironmentDetailsDto> GetEnvironmentDetails(Guid environmentId);

        Task<ToggleDetailsDto> GetToggleDetails(Guid toggleId);
    }
}
