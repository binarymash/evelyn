namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Collections.Generic;
    using Evelyn.Core.ReadModel.Dtos;
    using Evelyn.Core.ReadModel.Infrastructure;

    public class InMemoryReadModelFacade : IReadModelFacade
    {
        private IDatabase<ApplicationListDto> _applications;

        private IDatabase<ApplicationDetailsDto> _applicationDetails;

        public InMemoryReadModelFacade(IDatabase<ApplicationListDto> applications, IDatabase<ApplicationDetailsDto> applicationDetails)
        {
            _applications = applications;
            _applicationDetails = applicationDetails;
        }

        public IEnumerable<ApplicationListDto> GetApplications()
        {
            return _applications.Get();
        }

        public ApplicationDetailsDto GetApplicationDetails(Guid id)
        {
            return _applicationDetails.Get(id);
        }
    }
}
