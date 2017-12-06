namespace Microsoft.Extensions.DependencyInjection
{
    public class ReadStrategyRegistration : EvelynComponentRegistration
    {
        public ReadStrategyRegistration(IServiceCollection services)
            : base(services)
        {
        }
    }
}
