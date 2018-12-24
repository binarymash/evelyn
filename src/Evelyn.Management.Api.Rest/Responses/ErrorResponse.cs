namespace Evelyn.Management.Api.Rest.Responses
{
    using System.Collections.Generic;
    using System.Linq;

    public class ErrorResponse : Response<Error>
    {
        public ErrorResponse(IEnumerable<Error> errors)
            : base(errors)
        {
        }
    }
}
