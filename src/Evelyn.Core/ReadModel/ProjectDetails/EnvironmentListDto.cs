namespace Evelyn.Core.ReadModel.ProjectDetails
{
    public class EnvironmentListDto
    {
        public EnvironmentListDto(string key, string name)
        {
            Key = key;
            Name = name;
        }

        public string Key { get; }

        public string Name { get; }
    }
}
