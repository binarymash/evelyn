namespace Evelyn.Client.Rest
{
    using System;
    using System.Collections.Generic;
    using Domain;
    using Generated;
    using Microsoft.Extensions.Options;
    using Provider;

    public class EnvironmentStateRestProvider : IEnvironmentStateProvider
    {
        private readonly Client _client;

        public EnvironmentStateRestProvider(IOptions<EnvironmentStateRestProviderOptions> options)
        {
            _client = new Client(options.Value.BaseUrl);
        }

        public EnvironmentState Invoke(Guid projectId, string environmentKey)
        {
            try
            {
                var dto = _client
                    .ApiStatesByProjectIdByEnvironmentNameGetAsync(projectId, environmentKey)
                    .GetAwaiter().GetResult();

                var toggleStates = new List<ToggleState>();

                foreach (var toggleState in dto.ToggleStates)
                {
                    if (bool.TryParse(toggleState.Value, out var value))
                    {
                        toggleStates.Add(new ToggleState(toggleState.Key, value));
                    }
                }

                return new EnvironmentState(dto.Version.Value, toggleStates);
            }
            catch (SwaggerException ex)
            {
                throw new SynchronizationException("FailedToGetEnvironmentState", ex);
            }
        }
    }
}
