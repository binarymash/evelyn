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
                typeof(Evelyn.Core.ReadModel.Projections.AccountProjects.ProjectionBuilder),
                typeof(Evelyn.Core.ReadModel.Projections.EnvironmentDetails.ProjectionBuilder),
                typeof(Evelyn.Core.ReadModel.Projections.EnvironmentState.ProjectionBuilder),
                typeof(Evelyn.Core.ReadModel.Projections.ProjectDetails.ProjectionBuilder),
                typeof(Evelyn.Core.ReadModel.Projections.ToggleDetails.ProjectionBuilder),
                typeof(Evelyn.Core.ReadModel.Projections.ClientEnvironmentState.ProjectionBuilder),
            });
        }
    }
}
