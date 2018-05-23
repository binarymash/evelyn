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

    public class DeleteToggleValidatorSpecs
    {
        private readonly Fixture _fixture;
        private readonly DeleteToggleValidator _validator;

        public DeleteToggleValidatorSpecs()
        {
            _fixture = new Fixture();
            _validator = new DeleteToggleValidator();
        }

        [Fact]
        public void ProjectIdWhenNominalGuidIsValid()
        {
            var command = _fixture.Create<DeleteToggle>();

            _validator.ShouldNotHaveValidationErrorFor(c => c.ProjectId, command);
        }

        [Fact]
        public void ProjectIdWhenEmptyGuidIsInvalid()
        {
            var projectId = Guid.Empty;
            var command = _fixture.Build<DeleteToggle>().With(c => c.ProjectId, projectId).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.ProjectId, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyNotSet &&
                error.ErrorMessage == "'Project Id' should not be empty.");
        }

        [Fact]
        public void KeyWhenNominalIsValid()
        {
            var key = TestUtilities.CreateKey(30);
            var command = _fixture.Build<DeleteToggle>().With(c => c.Key, key).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.Key, command);
        }

        [Fact]
        public void KeyWhenNullIsInvalid()
        {
            var key = (string)null;
            var command = _fixture.Build<DeleteToggle>().With(c => c.Key, key).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.Key, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyNotSet &&
                error.ErrorMessage == "'Key' should not be empty.");
        }

        [Fact]
        public void KeyWhenEmptyIsInvalid()
        {
            var key = string.Empty;
            var command = _fixture.Build<DeleteToggle>().With(c => c.Key, key).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.Key, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyNotSet &&
                error.ErrorMessage == "'Key' should not be empty.");
        }

        [Fact]
        public void KeyWhenWhitespaceIsInvalid()
        {
            var key = "   ";
            var command = _fixture.Build<DeleteToggle>().With(c => c.Key, key).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.Key, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyNotSet &&
                error.ErrorMessage == "'Key' should not be empty.");
        }

        [Fact]
        public void ExpectedToggleVersionWhenNullIsValid()
        {
            var expectedVersopm = (int?)null;
            var command = _fixture.Build<DeleteToggle>().With(c => c.ExpectedToggleVersion, expectedVersopm).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.ExpectedToggleVersion, command);
        }

        [Fact]
        public void ExpectedToggleVersionWhenPositiveIsValid()
        {
            var expectedVersion = _fixture.Create<int>();
            var command = _fixture.Build<DeleteToggle>().With(c => c.ExpectedToggleVersion, expectedVersion).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.ExpectedToggleVersion, command);
        }

        [Fact]
        public void ExpectedToggleVersionWhenMaxIntIsValid()
        {
            var expectedVersion = int.MaxValue;
            var command = _fixture.Build<DeleteToggle>().With(c => c.ExpectedToggleVersion, expectedVersion).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.ExpectedToggleVersion, command);
        }

        [Fact]
        public void ExpectedToggleVersionWhenNegativeIsInvalid()
        {
            var expectedVersion = _fixture.Create<int>() * -1;
            var command = _fixture.Build<DeleteToggle>().With(c => c.ExpectedToggleVersion, expectedVersion).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.ExpectedToggleVersion, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyOutOfRange &&
                error.ErrorMessage == "'Expected Toggle Version' must be greater than or equal to '0'.");
        }
    }
}
