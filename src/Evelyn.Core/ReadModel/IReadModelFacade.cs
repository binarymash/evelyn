namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Collections.Generic;
    using Evelyn.Core.ReadModel.Dtos;

    public interface IReadModelFacade
    {
        IEnumerable<ApplicationListDto> GetApplications();

        ApplicationDetailsDto GetApplicationDetails(Guid id);
    }
}
