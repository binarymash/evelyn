namespace Evelyn.Core.WriteModel.Project.Domain
{
    public class Environment
    {
        public Environment()
        {
        }

        public Environment(string key)
            : this()
        {
            Key = key;
        }

        public string Key { get; private set; }
    }
}
