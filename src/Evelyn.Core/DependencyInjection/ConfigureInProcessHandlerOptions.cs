// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using Evelyn.Core.ReadModel.ProjectDetails;
    using Evelyn.Core.ReadModel.ProjectList;
    using Evelyn.Core.ReadModel.ToggleDetails;
    using Evelyn.Core.WriteModel.Handlers;
    using Microsoft.Extensions.Options;

    public class ConfigureInProcessHandlerOptions : IConfigureOptions<HandlerOptions>
    {
        public void Configure(HandlerOptions options)
        {
            options.Handlers.AddRange(new[]
            {
                typeof(ProjectCommandHandler),
                typeof(ProjectDetailsHandler),
                typeof(ProjectListHandler),
                typeof(EnvironmentDetailsHandler),
                typeof(ToggleDetailsHandler)
            });
        }
    }
}
