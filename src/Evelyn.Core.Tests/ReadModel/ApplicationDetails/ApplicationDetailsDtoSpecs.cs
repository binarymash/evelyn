namespace Evelyn.Core.Tests.ReadModel.ApplicationDetails
{
    using System;
    using AutoFixture;
    using Evelyn.Core.ReadModel.ApplicationDetails;
    using Xunit;

    public class ApplicationDetailsDtoSpecs : DtoSpecs<ApplicationDetailsDto>
    {
        [Fact]
        public void Serialization()
        {
            var applicationDetails = DataFixture.Create<ApplicationDetailsDto>();
            applicationDetails.AddEnvironment(DataFixture.Create<EnvironmentListDto>(), DataFixture.Create<DateTimeOffset>(), DataFixture.Create<int>());
            applicationDetails.AddEnvironment(DataFixture.Create<EnvironmentListDto>(), DataFixture.Create<DateTimeOffset>(), DataFixture.Create<int>());

            AssertSerializationOf(applicationDetails);
        }
    }
}
