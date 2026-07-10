using System.Diagnostics;
using System.Web.Mvc;
using Microsoft.Extensions.Logging;

namespace TaskManager.Filters
{
    public class LogActionFilter : ActionFilterAttribute
    {
        private const string TimerKey = "__action_timer";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var sw = Stopwatch.StartNew();
            filterContext.HttpContext.Items[TimerKey] = sw;
            AppLogger.Create<LogActionFilter>()?.LogInformation("MVC -> {Controller}.{Action}",
                filterContext.RouteData.Values["controller"],
                filterContext.RouteData.Values["action"]);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.HttpContext.Items[TimerKey] is Stopwatch sw)
            {
                sw.Stop();
                AppLogger.Create<LogActionFilter>()?.LogInformation("MVC <- {Controller}.{Action} in {Ms}ms (ex={ExType})",
                    filterContext.RouteData.Values["controller"],
                    filterContext.RouteData.Values["action"],
                    sw.ElapsedMilliseconds,
                    filterContext.Exception?.GetType().Name ?? "none");
            }
        }
    }
}
