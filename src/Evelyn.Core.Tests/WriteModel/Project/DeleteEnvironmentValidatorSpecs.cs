namespace Evelyn.Core.Tests.WriteModel.Project
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Core.WriteModel;
    using Core.WriteModel.Project.Commands;
    using FluentAssertions;
    using FluentValidation.TestHelper;
    using Xunit;

    public class DeleteEnvironmentValidatorSpecs
    {
        private readonly Fixture _fixture;
        private readonly DeleteEnvironmentValidator _validator;

        public DeleteEnvironmentValidatorSpecs()
        {
            _fixture = new Fixture();
            _validator = new DeleteEnvironmentValidator();
        }

        [Fact]
        public void ProjectIdWhenNominalGuidIsValid()
        {
            var command = _fixture.Create<DeleteEnvironment>();

            _validator.ShouldNotHaveValidationErrorFor(c => c.ProjectId, command);
        }

        [Fact]
        public void ProjectIdWhenEmptyGuidIsInvalid()
        {
            var projectId = Guid.Empty;
            var command = _fixture.Build<DeleteEnvironment>().With(c => c.ProjectId, projectId).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.ProjectId, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ValidationErrorCodes.ProjectIdNotSet &&
                error.ErrorMessage == "'Project Id' should not be empty.");
        }

        [Fact]
        public void KeyWhenNominalIsValid()
        {
            var key = TestUtilities.CreateKey(30);
            var command = _fixture.Build<DeleteEnvironment>().With(c => c.Key, key).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.Key, command);
        }

        [Fact]
        public void KeyWhenNullIsInvalid()
        {
            var key = (string)null;
            var command = _fixture.Build<DeleteEnvironment>().With(c => c.Key, key).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.Key, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ValidationErrorCodes.KeyNotSet &&
                error.ErrorMessage == "'Key' should not be empty.");
        }

        [Fact]
        public void KeyWhenEmptyIsInvalid()
        {
            var key = string.Empty;
            var command = _fixture.Build<DeleteEnvironment>().With(c => c.Key, key).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.Key, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ValidationErrorCodes.KeyNotSet &&
                error.ErrorMessage == "'Key' should not be empty.");
        }

        [Fact]
        public void KeyWhenWhitespaceIsInvalid()
        {
            var key = "   ";
            var command = _fixture.Build<DeleteEnvironment>().With(c => c.Key, key).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.Key, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ValidationErrorCodes.KeyNotSet &&
                error.ErrorMessage == "'Key' should not be empty.");
        }

        [Fact]
        public void ExpectedEnvironmentVersionWhenNullIsValid()
        {
            var expectedVersion = (int?)null;
            var command = _fixture.Build<DeleteEnvironment>().With(c => c.ExpectedEnvironmentVersion, expectedVersion).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.ExpectedEnvironmentVersion, command);
        }

        [Fact]
        public void ExpectedEnvironmentVersionWhenPositiveIsValid()
        {
            var expectedVersion = _fixture.Create<int>();
            var command = _fixture.Build<DeleteEnvironment>().With(c => c.ExpectedEnvironmentVersion, expectedVersion).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.ExpectedEnvironmentVersion, command);
        }

        [Fact]
        public void ExpectedEnvironmentVersionWhenMaxIntIsValid()
        {
            var expectedVersion = int.MaxValue;
            var command = _fixture.Build<DeleteEnvironment>().With(c => c.ExpectedEnvironmentVersion, expectedVersion).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.ExpectedEnvironmentVersion, command);
        }

        [Fact]
        public void ExpectedEnvironmentVersionWhenNegativeIsInvalid()
        {
            var expectedVersion = _fixture.Create<int>() * -1;
            var command = _fixture.Build<DeleteEnvironment>().With(c => c.ExpectedEnvironmentVersion, expectedVersion).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.ExpectedEnvironmentVersion, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ValidationErrorCodes.ExpectedEnvironmentVersionInvalid &&
                error.ErrorMessage == "'Expected Environment Version' must be greater than or equal to '0'.");
        }


    }
}
