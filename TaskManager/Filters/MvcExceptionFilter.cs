using System.Web.Mvc;
using Microsoft.Extensions.Logging;

namespace TaskManager.Filters
{
    public class MvcExceptionFilter : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled) return;

            var ex = filterContext.Exception;
            AppLogger.Create<MvcExceptionFilter>()?.LogError(ex, "MVC unhandled exception in {Controller}.{Action}: {Message}",
                filterContext.RouteData.Values["controller"],
                filterContext.RouteData.Values["action"],
                ex.Message);

            filterContext.ExceptionHandled = true;
            filterContext.Result = new ViewResult
            {
                ViewName = "Error",
                ViewData = new ViewDataDictionary<HandleErrorInfo>(
                    new HandleErrorInfo(ex,
                        filterContext.RouteData.Values["controller"]?.ToString() ?? "Home",
                        filterContext.RouteData.Values["action"]?.ToString() ?? "Index"))
            };
            filterContext.HttpContext.Response.StatusCode = 500;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}
