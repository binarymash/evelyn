namespace Evelyn.Core.Tests.WriteModel.Account
{
    using System;
    using System.Linq;
    using System.Text;
    using AutoFixture;
    using Core.WriteModel;
    using Core.WriteModel.Account.Commands;
    using FluentAssertions;
    using FluentValidation.TestHelper;
    using Xunit;

    public class CreateProjectValidatorSpecs
    {
        private readonly Fixture _fixture;
        private readonly CreateProjectValidator _validator;

        public CreateProjectValidatorSpecs()
        {
            _fixture = new Fixture();
            _validator = new CreateProjectValidator();
        }

        [Fact]
        public void ProjectNameWhenNominalIsValid()
        {
            var command = _fixture.Create<CreateProject>();

            _validator.ShouldNotHaveValidationErrorFor(c => c.Name, command);
        }

        [Fact]
        public void ProjectNameWhenNullIsInvalid()
        {
            var name = (string)null;
            var command = _fixture.Build<CreateProject>().With(c => c.Name, name).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.Name, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ValidationErrorCodes.NameNotSet &&
                error.ErrorMessage == "'Name' should not be empty.");
        }

        [Fact]
        public void ProjectNameWhenEmptyIsInvalid()
        {
            var name = string.Empty;
            var command = _fixture.Build<CreateProject>().With(c => c.Name, name).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.Name, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ValidationErrorCodes.NameNotSet &&
                error.ErrorMessage == "'Name' should not be empty.");
        }

        [Fact]
        public void ProjectNameWhenWhitespaceIsInvalid()
        {
            var name = "   ";
            var command = _fixture.Build<CreateProject>().With(c => c.Name, name).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.Name, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ValidationErrorCodes.NameNotSet &&
                error.ErrorMessage == "'Name' should not be empty.");
        }

        [Fact]
        public void ProjectNameWhen128CharactersIsValid()
        {
            var name = TestUtilities.CreateString(128);
            var command = _fixture.Build<CreateProject>().With(c => c.Name, name).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.Name, command);
        }

        [Fact]
        public void ProjectNameWhen129CharactersIsInvalid()
        {
            var name = TestUtilities.CreateString(129);
            var command = _fixture.Build<CreateProject>().With(c => c.Name, name).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.Name, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ValidationErrorCodes.NameTooLong &&
                error.ErrorMessage == "The length of 'Name' must be 128 characters or fewer. You entered 129 characters.");
        }

        [Fact]
        public void ExpectedVersionWhenNullIsValid()
        {
            var expectedVersion = (int?)null;
            var command = _fixture.Build<CreateProject>().With(c => c.ExpectedVersion, expectedVersion).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.ExpectedVersion, command);
        }

        [Fact]
        public void ExpectedVersionWhenPositiveIsValid()
        {
            var expectedVersion = _fixture.Create<int>();
            var command = _fixture.Build<CreateProject>().With(c => c.ExpectedVersion, expectedVersion).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.ExpectedVersion, command);
        }

        [Fact]
        public void ExpectedVersionWhenMaxIntIsValid()
        {
            var expectedVersion = int.MaxValue;
            var command = _fixture.Build<CreateProject>().With(c => c.ExpectedVersion, expectedVersion).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.ExpectedVersion, command);
        }

        [Fact]
        public void ExpectedVersionWhenNegativeIsInvalid()
        {
            var expectedVersion = _fixture.Create<int>() * -1;
            var command = _fixture.Build<CreateProject>().With(c => c.ExpectedVersion, expectedVersion).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.ExpectedVersion, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ValidationErrorCodes.ExpectedVersionInvalid &&
                error.ErrorMessage == "'Expected Version' must be greater than or equal to '0'.");
        }

        [Fact]
        public void ProjectIdWhenNominalGuidIsValid()
        {
            var command = _fixture.Create<CreateProject>();

            _validator.ShouldNotHaveValidationErrorFor(c => c.ProjectId, command);
        }

        [Fact]
        public void ProjectIdWhenEmptyGuidIsInvalid()
        {
            var projectId = Guid.Empty;
            var command = _fixture.Build<CreateProject>().With(c => c.ProjectId, projectId).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.ProjectId, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ValidationErrorCodes.ProjectIdNotSet &&
                error.ErrorMessage == "'Project Id' should not be empty.");
        }
    }
}
