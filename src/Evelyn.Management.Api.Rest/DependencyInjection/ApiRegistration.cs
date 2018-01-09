// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public class ApiRegistration : EvelynComponentRegistration
    {
        public ApiRegistration(IServiceCollection services)
            : base(services)
        {
        }
    }
}
