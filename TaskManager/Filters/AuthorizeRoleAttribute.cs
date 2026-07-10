using System;
using System.Web;
using System.Web.Mvc;

namespace TaskManager.Filters
{
    /// <summary>
    /// Restricts an action to one or more roles. Roles are stored as the auth ticket UserData
    /// (set in AccountController on login).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        private readonly string[] _roles;

        public AuthorizeRoleAttribute(params string[] roles)
        {
            _roles = roles ?? new string[0];
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!base.AuthorizeCore(httpContext)) return false;
            if (_roles.Length == 0) return true;

            // Items["UserRole"] is populated in Global.asax PostAuthenticateRequest via the
            // FormsAuthenticationTicket UserData blob.
            var role = httpContext.Items["UserRole"] as string;
            if (string.IsNullOrEmpty(role)) return false;
            return Array.IndexOf(_roles, role) >= 0;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User?.Identity?.IsAuthenticated == true)
            {
                filterContext.Result = new HttpStatusCodeResult(403, "Forbidden");
                return;
            }
            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}
