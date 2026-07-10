using System.Web.Mvc;

namespace TaskManager.Controllers
{
    /// <summary>
    /// Common helpers for authenticated MVC controllers — surfaces the current employee id
    /// and role from the auth ticket items populated in Global.asax PostAuthenticateRequest.
    /// </summary>
    [Authorize]
    public abstract class AppControllerBase : Controller
    {
        protected int CurrentUserId =>
            HttpContext?.Items["UserId"] is int id ? id : 0;

        protected string CurrentUserRole =>
            HttpContext?.Items["UserRole"] as string ?? "Employee";

        protected bool IsAdmin => CurrentUserRole == "Admin";
        protected bool IsManager => CurrentUserRole == "Manager" || IsAdmin;
    }
}
