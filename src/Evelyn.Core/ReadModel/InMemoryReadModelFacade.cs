namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Collections.Generic;
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

        public IEnumerable<ApplicationListDto> GetApplications()
        {
            return _applications.Get();
        }

        public ApplicationDetailsDto GetApplicationDetails(Guid applicationId)
        {
            return _applicationDetails.Get(applicationId);
        }

        public EnvironmentDetailsDto GetEnvironmentDetails(Guid environmentId)
        {
            return _environmentDetails.Get(environmentId);
        }
    }
}
