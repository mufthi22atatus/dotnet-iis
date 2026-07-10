using System.Web.Mvc;
using TaskManager.Filters;

namespace TaskManager
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new MvcExceptionFilter());
            filters.Add(new LogActionFilter());
        }
    }
}
