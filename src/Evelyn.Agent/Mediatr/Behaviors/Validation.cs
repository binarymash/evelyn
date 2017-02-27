namespace Evelyn.Agent.Mediatr.Behaviors
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BinaryMash.Responses;
    using FluentValidation;
    using FluentValidation.Results;
    using MediatR;

    public class Validation<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TResponse : Response
    {
        private IEnumerable<IValidator<TRequest>> validators;

        public Validation(IEnumerable<IValidator<TRequest>> validators)
        {
            this.validators = validators ?? new List<IValidator<TRequest>>();
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next)
        {
            if (request == null)
            {
                return BuildResponse
                    .WithType<TResponse>()
                    .AndWithErrors(new Error("InvalidRequest", "The request must not be null"))
                    .Create();
            }

            var errors = new List<ValidationFailure>();

            foreach (var validator in this.validators)
            {
                var validationResult = await validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    errors.AddRange(validationResult.Errors);
                }
            }

            if (errors.Any())
            {
                var errorMessage = string.Join("\r\n", errors.Select(e => e.ErrorMessage));
                return BuildResponse
                    .WithType<TResponse>()
                    .AndWithErrors(new Error("InvalidRequest", errorMessage))
                    .Create();
            }

            return await next().ConfigureAwait(false);
        }
    }
}
