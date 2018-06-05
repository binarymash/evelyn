namespace Evelyn.Management.Api.Rest.Write.Projects.Messages
{
    public class DeleteProject
    {
        public DeleteProject(int expectedProjectVersion)
        {
            ExpectedProjectVersion = expectedProjectVersion;
        }

        public int ExpectedProjectVersion { get; set; }
    }
}
