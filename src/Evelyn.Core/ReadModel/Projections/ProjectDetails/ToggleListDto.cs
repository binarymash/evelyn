namespace Evelyn.Core.ReadModel.Projections.ProjectDetails
{
    public class ToggleListDto
    {
        public ToggleListDto(string key, string name)
        {
            Key = key;
            Name = name;
        }

        public string Key { get; }

        public string Name { get; }
    }
}
