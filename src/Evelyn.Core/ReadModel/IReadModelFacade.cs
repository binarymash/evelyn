namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Collections.Generic;
    using Evelyn.Core.ReadModel.ApplicationDetails;
    using Evelyn.Core.ReadModel.ApplicationList;
    using Evelyn.Core.ReadModel.EnvironmentDetails;

    public interface IReadModelFacade
    {
        IEnumerable<ApplicationListDto> GetApplications();

        ApplicationDetailsDto GetApplicationDetails(Guid applicationId);

        EnvironmentDetailsDto GetEnvironmentDetails(Guid environmentId);
    }
}
