// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Configures the read model
    /// </summary>
    public class ReadModelOptions : EvelynComponentOptions
    {
        public ReadModelOptions(IServiceCollection services)
            : base(services)
        {
            WithReadStrategy = new ReadStrategyOptions(services);
        }

        /// <summary>
        /// Configures how we will retrieve the read model.
        /// </summary>
        /// <value>
        /// The read strategy options
        /// </value>
        public ReadStrategyOptions WithReadStrategy { get; }
    }
}
