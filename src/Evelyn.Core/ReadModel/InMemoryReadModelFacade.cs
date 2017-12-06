﻿namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Evelyn.Core.ReadModel.ApplicationDetails;
    using Evelyn.Core.ReadModel.ApplicationList;
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using Evelyn.Core.ReadModel.Infrastructure;

    public class InMemoryReadModelFacade : IReadModelFacade
    {
        private IDatabase<ApplicationListDto> _applications;

        private IDatabase<ApplicationDetailsDto> _applicationDetails;

        private IDatabase<EnvironmentDetailsDto> _environmentDetails;

        public InMemoryReadModelFacade(
            IDatabase<ApplicationListDto> applications,
            IDatabase<ApplicationDetailsDto> applicationDetails,
            IDatabase<EnvironmentDetailsDto> environmentDetails)
        {
            _applications = applications;
            _applicationDetails = applicationDetails;
            _environmentDetails = environmentDetails;
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
    }
}
