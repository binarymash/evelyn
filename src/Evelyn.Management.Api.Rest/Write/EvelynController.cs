namespace Evelyn.Management.Api.Rest.Write
{
    using System;
    using System.Linq;
    using Core;
    using Core.WriteModel;
    using CQRSlite.Domain.Exception;
    using FluentValidation;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Responses;

    public abstract class EvelynController : Microsoft.AspNetCore.Mvc.Controller
    {
        protected string UserId => Constants.AnonymousUser;

        protected Guid AccountId => Constants.DefaultAccount;

        protected BadRequestObjectResult HandleValidationException(ValidationException ex)
        {
            var response = new Response<ValidationError>(ex.Errors.Select(e => new ValidationError(e.ErrorCode, e.PropertyName, e.ErrorMessage)));
            return new BadRequestObjectResult(response);
        }

        protected BadRequestObjectResult HandleConcurrencyException(ConcurrencyException ex)
        {
            var response = new Response<Error>(new[] { new Error(ErrorCodes.ConcurrencyError, ex.Message) });
            return new BadRequestObjectResult(response);
        }

        protected ObjectResult HandleInternalError(Exception ex)
        {
            return new ObjectResult(null) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}
