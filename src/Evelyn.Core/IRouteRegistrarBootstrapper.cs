namespace Evelyn.Core
{
    using System;

    public interface IRouteRegistrarBootstrapper
    {
        void Bootstrap(IServiceProvider serviceProvider);
    }
}
