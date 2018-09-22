// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using Microsoft.Extensions.Options;

    public class ConfigureEventStreamHandlerOptions : IConfigureOptions<EventStreamHandlerOptions>
    {
        public void Configure(EventStreamHandlerOptions options)
        {
            options.ProjectionBuilders.AddRange(new[]
            {
                typeof(Evelyn.Core.ReadModel.AccountProjects.ProjectionBuilder),
                typeof(Evelyn.Core.ReadModel.EnvironmentDetails.ProjectionBuilder),
                typeof(Evelyn.Core.ReadModel.EnvironmentState.ProjectionBuilder),
                typeof(Evelyn.Core.ReadModel.ProjectDetails.ProjectionBuilder),
                typeof(Evelyn.Core.ReadModel.ToggleDetails.ProjectionBuilder),
            });
        }
    }
}
