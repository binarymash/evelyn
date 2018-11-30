namespace Evelyn.Client.Rest
{
    using System;
    using System.Collections.Generic;
    using Domain;
    using Generated;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Provider;

    public class EnvironmentStateRestProvider : IEnvironmentStateProvider
    {
        private readonly Client _client;
        private readonly ILogger<EnvironmentStateRestProvider> _logger;

        public EnvironmentStateRestProvider(IOptions<EnvironmentStateRestProviderOptions> options, ILogger<EnvironmentStateRestProvider> logger)
        {
            _client = new Client(options.Value.BaseUrl);
            _logger = logger;
        }

        public EnvironmentState Invoke(Guid projectId, string environmentKey)
        {
            try
            {
                var dto = _client
                    .ApiStatesAsync(projectId, environmentKey)
                    .GetAwaiter().GetResult();

                var toggleStates = new List<ToggleState>();

                foreach (var toggleState in dto.ToggleStates)
                {
                    if (bool.TryParse(toggleState.Value, out var value))
                    {
                        toggleStates.Add(new ToggleState(toggleState.Key, value));
                    }
                }

                return new EnvironmentState(dto.Audit.Version.Value, toggleStates);
            }
            catch (SwaggerException ex)
            {
                _logger.LogWarning(ex, "Failed to get state of {@projectdId} {@environmentKey} from {@serverBaseUrl}", projectId, environmentKey, _client.BaseUrl);
                throw new SynchronizationException("FailedToGetEnvironmentState", ex);
            }
        }
    }
}
