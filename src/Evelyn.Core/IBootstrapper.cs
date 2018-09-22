namespace Evelyn.Core
{
    using System;
    using WriteModel;

    public interface IBootstrapper
    {
        void Bootstrap(IServiceProvider serviceProvider, IStartUpCommands startUpCommands);
    }
}
