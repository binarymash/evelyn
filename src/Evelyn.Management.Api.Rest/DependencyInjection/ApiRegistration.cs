// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public class ApiRegistration : EvelynComponentRegistration
    {
        public ApiRegistration(IServiceCollection services)
            : base(services)
        {
            WithWriteModel = new WriteModelRegistration(services);
            WithReadModel = new ReadModelRegistration(services);
        }

        public WriteModelRegistration WithWriteModel { get; }

        public ReadModelRegistration WithReadModel { get; }
    }
}
