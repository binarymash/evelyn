namespace Evelyn.Core.WriteModel
{
    public class ValidationErrorCodes
    {
        public const string ProjectIdNotSet = "ProjectIdNotSet";
        public const string NameNotSet = "NameNotSet";
        public const string KeyNotSet = "KeyNotSet";
        public const string ValueNotSet = "ValueNotSet";

        public const string NameTooLong = "NameTooLong";
        public const string KeyTooLong = "KeyTooLong";

        public const string ExpectedVersionInvalid = "ExpectedVersionInvalid";
        public const string ExpectedProjectVersionInvalid = "ExpectedProjectVersionInvalid";
        public const string ExpectedToggleStateVersionInvalid = "ExpectedToggleStateVersionInvalid";
        public const string ExpectedEnvironmentVersionInvalid = "ExpectedEnvironmentVersionInvalid";

        public const string KeyHasIncorrectFormat = "KeyHasIncorrectFormat";
        public const string ValueHasIncorrectFormat = "ValueHasIncorrectFormat";
    }
}
