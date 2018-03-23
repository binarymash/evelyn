namespace Evelyn.Client.Rest
{
    using System;
    using System.Linq;

    public class EnvironmentStateRestProvider : IEnvironmentStateProvider
    {
        private readonly Client _client;

        public EnvironmentStateRestProvider()
        {
            _client = new Client("http://localhost:2316");            
        }

        public EnvironmentState Invoke(Guid projectId, string environmentKey)
        {
            try
            {
                var dto = _client
                    .ApiStatesByProjectIdByEnvironmentNameGetAsync(projectId, environmentKey)
                    .GetAwaiter().GetResult();

                return new EnvironmentState(dto.Version.Value, dto.ToggleStates.Select(ts => new ToggleState(ts.Key, ts.Value)));
            }
            catch (SwaggerException ex)
            {
                throw new SynchronizationException(string.Empty, ex);
            }
        }
    }
}
