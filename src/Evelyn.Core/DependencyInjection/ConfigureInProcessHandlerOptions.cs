// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using Evelyn.Core.ReadModel.AccountProjects;
    using Evelyn.Core.ReadModel.EnvironmentDetails;
    using Evelyn.Core.ReadModel.ProjectDetails;
    using Evelyn.Core.ReadModel.ToggleDetails;
    using Evelyn.Core.WriteModel.Project;
    using Microsoft.Extensions.Options;

    public class ConfigureInProcessHandlerOptions : IConfigureOptions<HandlerOptions>
    {
        public void Configure(HandlerOptions options)
        {
            options.Handlers.AddRange(new[]
            {
                typeof(ProjectCommandHandler),
                typeof(Evelyn.Core.ReadModel.ProjectDetails.Handler),
                typeof(Evelyn.Core.ReadModel.AccountProjects.Handler),
                typeof(Evelyn.Core.ReadModel.EnvironmentDetails.Handler),
                typeof(Evelyn.Core.ReadModel.ToggleDetails.Handler)
            });
        }
    }
}
