namespace Evelyn.Core.ReadModel.ProjectDetails
{
    using System;

    public class EnvironmentListDto
    {
        public EnvironmentListDto(string key)
        {
            Key = key;
        }

        public string Key { get; }
    }
}
