namespace Evelyn.Client
{
    public interface IEnvironmentStateRepository
    {
        void Set(EnvironmentState environmentState);

        bool Get(string environmentKey);
    }
}
