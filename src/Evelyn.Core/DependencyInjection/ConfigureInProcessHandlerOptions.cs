// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using Microsoft.Extensions.Options;

    public class ConfigureInProcessHandlerOptions : IConfigureOptions<HandlerOptions>
    {
        public void Configure(HandlerOptions options)
        {
            options.Handlers.AddRange(new[]
            {
                typeof(Evelyn.Core.ReadModel.ProjectDetails.EventStreamPublisher),
                typeof(Evelyn.Core.ReadModel.AccountProjects.EventStreamPublisher),
                typeof(Evelyn.Core.ReadModel.EnvironmentDetails.EventStreamPublisher),
                typeof(Evelyn.Core.ReadModel.EnvironmentState.EventStreamPublisher),
                typeof(Evelyn.Core.ReadModel.ToggleDetails.EventStreamPublisher),
            });
        }
    }
}
