namespace Evelyn.Management.Api.Rest.Write
{
    using System;
    using Core;

    public abstract class EvelynController : Microsoft.AspNetCore.Mvc.Controller
    {
        protected string UserId => Constants.AnonymousUser;

        protected Guid AccountId => Constants.DefaultAccount;
    }
}
