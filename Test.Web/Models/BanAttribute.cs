using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Test.Web.Models
{
    public class BanAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return !httpContext.User.IsInRole("Disabled");
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult("ban", new RouteValueDictionary());
        }
    }
}