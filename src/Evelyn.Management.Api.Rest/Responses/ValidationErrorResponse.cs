namespace Evelyn.Management.Api.Rest.Responses
{
    using System.Collections.Generic;
    using System.Linq;

    public class ValidationErrorResponse : Response<ValidationError>
    {
        public ValidationErrorResponse(IEnumerable<ValidationError> errors)
            : base(errors)
        {
        }
    }
}
