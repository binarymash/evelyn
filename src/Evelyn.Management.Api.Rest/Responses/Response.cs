namespace Evelyn.Management.Api.Rest.Responses
{
    using System.Collections.Generic;
    using System.Linq;

    public abstract class Response<T>
        where T : Error
    {
        private readonly IList<T> _errors;

        public Response(IEnumerable<T> errors)
        {
            _errors = errors?.ToList() ?? new List<T>();
        }

        public IEnumerable<T> Errors => _errors.ToList();
    }
}
