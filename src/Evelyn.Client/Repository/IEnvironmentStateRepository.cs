namespace Evelyn.Client.Repository
{
    using Domain;

    public interface IEnvironmentStateRepository
    {
        void Set(EnvironmentState environmentState);

        bool Get(string environmentKey);
    }
}
