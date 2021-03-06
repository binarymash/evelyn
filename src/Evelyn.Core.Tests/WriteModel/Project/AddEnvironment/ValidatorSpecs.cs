﻿namespace Evelyn.Core.Tests.WriteModel.Project.AddEnvironment
{
    using System;
    using System.Linq;
    using AutoFixture;
    using Core.WriteModel.Project.Commands.AddEnvironment;
    using FluentAssertions;
    using FluentValidation.TestHelper;
    using Xunit;
    using ErrorCodes = Core.WriteModel.ErrorCodes;

    public class ValidatorSpecs
    {
        private readonly Fixture _fixture;
        private readonly Validator _validator;

        public ValidatorSpecs()
        {
            _fixture = new Fixture();
            _validator = new Validator();
        }

        [Fact]
        public void KeyWhenNominalIsValid()
        {
            var key = TestUtilities.CreateKey(30);
            var command = _fixture.Build<Command>().With(c => c.Key, key).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.Key, command);
        }

        [Fact]
        public void KeyWhenNullIsInvalid()
        {
            var key = (string)null;
            var command = _fixture.Build<Command>().With(c => c.Key, key).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.Key, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyNotSet &&
                error.ErrorMessage == "'Key' must not be empty.");
        }

        [Fact]
        public void KeyWhenEmptyIsInvalid()
        {
            var key = string.Empty;
            var command = _fixture.Build<Command>().With(c => c.Key, key).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.Key, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyNotSet &&
                error.ErrorMessage == "'Key' must not be empty.");
        }

        [Fact]
        public void KeyWhenWhitespaceIsInvalid()
        {
            var key = "   ";
            var command = _fixture.Build<Command>().With(c => c.Key, key).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.Key, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyNotSet &&
                error.ErrorMessage == "'Key' must not be empty.");
        }

        [Fact]
        public void KeyWhen128CharactersIsValid()
        {
            var key = TestUtilities.CreateKey(128);
            var command = _fixture.Build<Command>().With(c => c.Key, key).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.Key, command);
        }

        [Fact]
        public void KeyWhen129CharactersIsInvalid()
        {
            var key = TestUtilities.CreateKey(129);
            var command = _fixture.Build<Command>().With(c => c.Key, key).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.Key, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyTooLong &&
                error.ErrorMessage == "The length of 'Key' must be 128 characters or fewer. You entered 129 characters.");
        }

        [Fact]
        public void KeyWhenInvalidCharactersIsInvalid()
        {
            var key = "This is not valid";
            var command = _fixture.Build<Command>().With(c => c.Key, key).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.Key, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyIncorrectFormat &&
                error.ErrorMessage == "'Key' is not in the correct format.");
        }

        [Fact]
        public void ExpectedProjectVersionWhenNullIsValid()
        {
            var expectedProjectVersion = (int?)null;
            var command = _fixture.Build<Command>().With(c => c.ExpectedProjectVersion, expectedProjectVersion).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.ExpectedProjectVersion, command);
        }

        [Fact]
        public void ExpectedProjectVersionWhenPositiveIsValid()
        {
            var expectedProjectVersion = _fixture.Create<int>();
            var command = _fixture.Build<Command>().With(c => c.ExpectedProjectVersion, expectedProjectVersion).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.ExpectedProjectVersion, command);
        }

        [Fact]
        public void ExpectedProjectVersionWhenMaxIntIsValid()
        {
            var expectedProjectVersion = int.MaxValue;
            var command = _fixture.Build<Command>().With(c => c.ExpectedProjectVersion, expectedProjectVersion).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.ExpectedProjectVersion, command);
        }

        [Fact]
        public void ExpectedProjectVersionWhenNegativeIsInvalid()
        {
            var expectedVersion = _fixture.Create<int>() * -1;
            var command = _fixture.Build<Command>().With(c => c.ExpectedProjectVersion, expectedVersion).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.ExpectedProjectVersion, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyOutOfRange &&
                error.ErrorMessage == "'Expected Project Version' must be greater than or equal to '0'.");
        }

        [Fact]
        public void ProjectIdWhenNominalGuidIsValid()
        {
            var command = _fixture.Create<Command>();

            _validator.ShouldNotHaveValidationErrorFor(c => c.ProjectId, command);
        }

        [Fact]
        public void ProjectIdWhenEmptyGuidIsInvalid()
        {
            var projectId = Guid.Empty;
            var command = _fixture.Build<Command>().With(c => c.ProjectId, projectId).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.ProjectId, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyNotSet &&
                error.ErrorMessage == "'Project Id' must not be empty.");
        }

        [Fact]
        public void NameWhenNominalIsValid()
        {
            var command = _fixture.Create<Command>();

            _validator.ShouldNotHaveValidationErrorFor(c => c.Name, command);
        }

        [Fact]
        public void NameWhenNullIsInvalid()
        {
            var name = (string)null;
            var command = _fixture.Build<Command>().With(c => c.Name, name).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.Name, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyNotSet &&
                error.ErrorMessage == "'Name' must not be empty.");
        }

        [Fact]
        public void NameWhenEmptyIsInvalid()
        {
            var name = string.Empty;
            var command = _fixture.Build<Command>().With(c => c.Name, name).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.Name, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyNotSet &&
                error.ErrorMessage == "'Name' must not be empty.");
        }

        [Fact]
        public void NameWhenWhitespaceIsInvalid()
        {
            var name = "   ";
            var command = _fixture.Build<Command>().With(c => c.Name, name).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.Name, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyNotSet &&
                error.ErrorMessage == "'Name' must not be empty.");
        }

        [Fact]
        public void NameWhen128CharactersIsValid()
        {
            var name = TestUtilities.CreateString(128);
            var command = _fixture.Build<Command>().With(c => c.Name, name).Create();

            _validator.ShouldNotHaveValidationErrorFor(c => c.Name, command);
        }

        [Fact]
        public void NameWhen129CharactersIsInvalid()
        {
            var name = TestUtilities.CreateString(129);
            var command = _fixture.Build<Command>().With(c => c.Name, name).Create();

            var errors = _validator.ShouldHaveValidationErrorFor(c => c.Name, command).ToList();
            errors.Should().Contain(error =>
                error.ErrorCode == ErrorCodes.PropertyTooLong &&
                error.ErrorMessage == "The length of 'Name' must be 128 characters or fewer. You entered 129 characters.");
        }
    }
}
