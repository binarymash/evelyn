// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Configures the read model
    /// </summary>
    public class ReadModelRegistration : EvelynComponentRegistration
    {
        public ReadModelRegistration(IServiceCollection services)
            : base(services)
        {
            WithReadStrategy = new ReadStrategyRegistration(services);
        }

        /// <summary>
        /// Gets the read strategy options
        /// </summary>
        /// <value>
        /// The read strategy options
        /// </value>
        public ReadStrategyRegistration WithReadStrategy { get; }
    }
}
