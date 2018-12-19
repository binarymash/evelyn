namespace Evelyn.Management.Api.Rest.Write.Environments.Messages
{
    public class DeleteEnvironment
    {
        public DeleteEnvironment(int? expectedEnvironmentVersion)
        {
            ExpectedEnvironmentVersion = expectedEnvironmentVersion;
        }

        public int? ExpectedEnvironmentVersion { get; set; }
    }
}
