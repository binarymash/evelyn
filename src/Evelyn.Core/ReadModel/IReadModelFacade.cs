namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Collections.Generic;
    using Evelyn.Core.ReadModel.ApplicationDetails;
    using Evelyn.Core.ReadModel.ApplicationList;

    public interface IReadModelFacade
    {
        IEnumerable<ApplicationListDto> GetApplications();

        ApplicationDetailsDto GetApplicationDetails(Guid id);
    }
}
