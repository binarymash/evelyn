namespace Evelyn.Core.ReadModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Evelyn.Core.ReadModel.ApplicationDetails;
    using Evelyn.Core.ReadModel.ApplicationList;
    using Evelyn.Core.ReadModel.EnvironmentDetails;

    public class EventStoreReadModelFacade : IReadModelFacade
    {
        private IEventStore _eventStore;

        public EventStoreReadModelFacade(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<IEnumerable<ApplicationListDto>> GetApplications()
        {
            return await Task.FromResult(new List<ApplicationListDto>());
        }

        public async Task<ApplicationDetailsDto> GetApplicationDetails(Guid applicationId)
        {
            return await Task.FromResult((ApplicationDetailsDto)null);
        }

        public async Task<EnvironmentDetailsDto> GetEnvironmentDetails(Guid environmentId)
        {
            return await Task.FromResult((EnvironmentDetailsDto)null);
        }
    }
}
