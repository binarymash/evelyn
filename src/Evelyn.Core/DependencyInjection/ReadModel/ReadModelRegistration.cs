namespace Microsoft.Extensions.DependencyInjection
{
    public class ReadModelRegistration : EvelynComponentRegistration
    {
        public ReadModelRegistration(IServiceCollection services)
            : base(services)
        {
            WithReadStrategy = new ReadStrategyRegistration(services);
        }

        public ReadStrategyRegistration WithReadStrategy { get; }
    }
}
