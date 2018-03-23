namespace Evelyn.Client
{
    using System;

    public interface IEnvironmentStateProvider
    {
        EnvironmentState Invoke(Guid projectId, string environmentKey);
    }
}