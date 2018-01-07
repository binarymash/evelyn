namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Evelyn.Core.ReadModel.ApplicationDetails;
    using Evelyn.Core.ReadModel.ApplicationList;
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using Evelyn.Core.ReadModel.Infrastructure;
    using Evelyn.Core.ReadModel.ToggleDetails;

    public class DatabaseReadModelFacade : IReadModelFacade
    {
        private readonly IDatabase<ApplicationListDto> _applications;

        private readonly IDatabase<ApplicationDetailsDto> _applicationDetails;

        private readonly IDatabase<EnvironmentDetailsDto> _environmentDetails;

        private readonly IDatabase<ToggleDetailsDto> _toggleDetails;

        public DatabaseReadModelFacade(
            IDatabase<ApplicationListDto> applications,
            IDatabase<ApplicationDetailsDto> applicationDetails,
            IDatabase<EnvironmentDetailsDto> environmentDetails,
            IDatabase<ToggleDetailsDto> toggleDetails)
        {
            _applications = applications;
            _applicationDetails = applicationDetails;
            _environmentDetails = environmentDetails;
            _toggleDetails = toggleDetails;
        }

        public async Task<IEnumerable<ApplicationListDto>> GetApplications()
        {
            return await _applications.Get();
        }

        public async Task<ApplicationDetailsDto> GetApplicationDetails(Guid applicationId)
        {
            return await _applicationDetails.Get(applicationId);
        }

        public async Task<EnvironmentDetailsDto> GetEnvironmentDetails(Guid environmentId)
        {
            return await _environmentDetails.Get(environmentId);
        }

        public async Task<ToggleDetailsDto> GetToggleDetails(Guid toggleId)
        {
            return await _toggleDetails.Get(toggleId);
        }
    }
}
