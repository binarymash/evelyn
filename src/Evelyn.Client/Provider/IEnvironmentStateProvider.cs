namespace Evelyn.Client.Provider
{
    using System;
    using Domain;

    public interface IEnvironmentStateProvider
    {
        EnvironmentState Invoke(Guid projectId, string environmentKey);
    }
}