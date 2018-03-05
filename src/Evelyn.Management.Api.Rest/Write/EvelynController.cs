namespace Evelyn.Management.Api.Rest.Write
{
    public abstract class EvelynController : Microsoft.AspNetCore.Mvc.Controller
    {
        protected string UserId => Constants.AnonymousUser;

        protected string AccountId => Constants.DefaultAccount;
    }
}
