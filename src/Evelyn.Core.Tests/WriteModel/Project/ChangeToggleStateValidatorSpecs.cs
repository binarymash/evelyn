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

    public class ChangeToggleStateValidatorSpecs
    {
        private readonly Fixture _fixture;
        private readonly ChangeToggleStateValidator _validator;

        public ChangeToggleStateValidatorSpecs()
        {
            _fixture = new Fixture();
            _validator = new ChangeToggleStateValidator();
        }

        [Fact]
        public void ProjectIdWhenNominalGuidIsValid()
        {
            var command = _fixture.Create<ChangeToggleState>();

            _validator.ShouldNotHaveValidationErrorFor(c => c.ProjectId, command);
        }

        [Fact]
        public void ProjectIdWhenEmptyGuidIsInvalid()
        {
            var projectId = Guid.Empty;
            var command = _fixture.Build<ChangeToggleState>().With(c => c.ProjectId, projectId).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.ProjectId, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyNotSet &&
                error.ErrorMessage == "'Project Id' should not be empty.");
        }

        [Fact]
        public void ToggleKeyWhenNominalIsValid()
        {
            var key = TestUtilities.CreateKey(30);
            var command = _fixture.Build<ChangeToggleState>().With(c => c.ToggleKey, key).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.ToggleKey, command);
        }

        [Fact]
        public void ToggleKeyWhenNullIsInvalid()
        {
            var key = (string)null;
            var command = _fixture.Build<ChangeToggleState>().With(c => c.ToggleKey, key).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.ToggleKey, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyNotSet &&
                error.ErrorMessage == "'Toggle Key' should not be empty.");
        }

        [Fact]
        public void ToggleKeyWhenEmptyIsInvalid()
        {
            var key = string.Empty;
            var command = _fixture.Build<ChangeToggleState>().With(c => c.ToggleKey, key).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.ToggleKey, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyNotSet &&
                error.ErrorMessage == "'Toggle Key' should not be empty.");
        }

        [Fact]
        public void EnvironmentKeyWhenNominalIsValid()
        {
            var key = TestUtilities.CreateKey(30);
            var command = _fixture.Build<ChangeToggleState>().With(c => c.EnvironmentKey, key).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.EnvironmentKey, command);
        }

        [Fact]
        public void EnvironmentKeyWhenNullIsInvalid()
        {
            var key = (string)null;
            var command = _fixture.Build<ChangeToggleState>().With(c => c.EnvironmentKey, key).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.EnvironmentKey, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyNotSet &&
                error.ErrorMessage == "'Environment Key' should not be empty.");
        }

        [Fact]
        public void EnvironmentKeyWhenEmptyIsInvalid()
        {
            var key = string.Empty;
            var command = _fixture.Build<ChangeToggleState>().With(c => c.EnvironmentKey, key).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.EnvironmentKey, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyNotSet &&
                error.ErrorMessage == "'Environment Key' should not be empty.");
        }

        [Fact]
        public void ExpectedToggleStateVersionWhenNullIsValid()
        {
            var expectedVersion = (int?)null;
            var command = _fixture.Build<ChangeToggleState>().With(c => c.ExpectedToggleStateVersion, expectedVersion).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.ExpectedToggleStateVersion, command);
        }

        [Fact]
        public void ExpectedToggleStateVersionWhenPositiveIsValid()
        {
            var expectedVersion = _fixture.Create<int>();
            var command = _fixture.Build<ChangeToggleState>().With(c => c.ExpectedToggleStateVersion, expectedVersion).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.ExpectedToggleStateVersion, command);
        }

        [Fact]
        public void ExpectedToggleStateVersionWhenMaxIntIsValid()
        {
            var expectedVersion = int.MaxValue;
            var command = _fixture.Build<ChangeToggleState>().With(c => c.ExpectedToggleStateVersion, expectedVersion).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.ExpectedToggleStateVersion, command);
        }

        [Fact]
        public void ExpectedToggleStateVersionWhenNegativeIsInvalid()
        {
            var expectedVersion = _fixture.Create<int>() * -1;
            var command = _fixture.Build<ChangeToggleState>().With(c => c.ExpectedToggleStateVersion, expectedVersion).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.ExpectedToggleStateVersion, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyOutOfRange &&
                error.ErrorMessage == "'Expected Toggle State Version' must be greater than or equal to '0'.");
        }

        [Fact]
        public void ValueWhenTrueIsValid()
        {
            var value = bool.TrueString;
            var command = _fixture.Build<ChangeToggleState>().With(c => c.Value, value).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.Value, command);
        }

        [Fact]
        public void ValueWhenFalseIsValid()
        {
            var value = bool.FalseString;
            var command = _fixture.Build<ChangeToggleState>().With(c => c.Value, value).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.Value, command);
        }

        [Fact]
        public void ValueWhenNullIsInvalid()
        {
            var value = (string)null;
            var command = _fixture.Build<ChangeToggleState>().With(c => c.Value, value).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.Value, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyNotSet &&
                error.ErrorMessage == "'Value' should not be empty.");
        }

        [Fact]
        public void ValueWhenEmptyIsInvalid()
        {
            var value = string.Empty;
            var command = _fixture.Build<ChangeToggleState>().With(c => c.Value, value).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.Value, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyNotSet &&
                error.ErrorMessage == "'Value' should not be empty.");
        }

        [Fact]
        public void ValueWhenWhitespaceIsInvalid()
        {
            var value = "   ";
            var command = _fixture.Build<ChangeToggleState>().With(c => c.Value, value).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.Value, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyNotSet &&
                error.ErrorMessage == "'Value' should not be empty.");
        }

        [Fact]
        public void ValueWhenNotBooleanStringIsInvalid()
        {
            var value = TestUtilities.CreateString(10);
            var command = _fixture.Build<ChangeToggleState>().With(c => c.Value, value).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.Value, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyIncorrectFormat &&
                error.ErrorMessage == "'Value' is not in the correct format.");
        }
    }
}
