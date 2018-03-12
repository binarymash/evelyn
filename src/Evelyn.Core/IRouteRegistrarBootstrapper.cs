namespace Evelyn.Core
{
    using System;
    using WriteModel;

    public interface IRouteRegistrarBootstrapper
    {
        void Bootstrap(IServiceProvider serviceProvider, IStartUpCommands startUpCommands);
    }
}
