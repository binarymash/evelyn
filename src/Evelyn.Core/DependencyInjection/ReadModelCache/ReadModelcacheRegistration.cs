namespace Microsoft.Extensions.DependencyInjection
{
    public class ReadModelCacheRegistration : EvelynComponentRegistration
    {
        public ReadModelCacheRegistration(IServiceCollection services)
            : base(services)
        {
        }
    }
}
