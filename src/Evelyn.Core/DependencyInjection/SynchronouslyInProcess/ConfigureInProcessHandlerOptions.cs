namespace Microsoft.Extensions.DependencyInjection
{
    using Evelyn.Core.ReadModel.ApplicationDetails;
    using Evelyn.Core.ReadModel.ApplicationList;
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using Evelyn.Core.ReadModel.ToggleDetails;
    using Evelyn.Core.WriteModel.Handlers;
    using Microsoft.Extensions.Options;

    public class ConfigureInProcessHandlerOptions : IConfigureOptions<HandlerOptions>
    {
        public void Configure(HandlerOptions options)
        {
            options.Handlers.AddRange(new[]
            {
                typeof(ApplicationCommandHandler),
                typeof(ApplicationDetailsHandler),
                typeof(ApplicationListHandler),
                typeof(EnvironmentDetailsHandler),
                typeof(ToggleDetailsHandler)
            });
        }
    }
}
