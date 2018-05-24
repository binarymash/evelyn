namespace Evelyn.Management.Api.Rest.Responses
{
    public class ValidationError : Error
    {
        public ValidationError(string code, string property, string message)
            : base(code, message)
        {
            Property = property;
        }

        public string Property { get; }
    }
}
