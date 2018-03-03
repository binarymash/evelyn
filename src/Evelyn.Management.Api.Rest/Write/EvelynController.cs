namespace Evelyn.Management.Api.Rest.Write
{
    public abstract class EvelynController : Microsoft.AspNetCore.Mvc.Controller
    {
        protected string UserId => "AnonymousUser";

        protected string AccountId => "DefaultAccount";
    }
}
