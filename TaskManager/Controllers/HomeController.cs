using System.Web.Mvc;
using Microsoft.Extensions.Logging;

namespace TaskManager.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Heartbeat = System.DateTime.UtcNow;
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Error()
        {
            AppLogger.Create<HomeController>()?.LogInformation("Error page viewed");
            return View();
        }

        // Convenience endpoint to deliberately throw — useful for APM exception trace tests.
        public ActionResult Boom()
        {
            throw new System.InvalidOperationException("Synthetic /Home/Boom failure for APM testing");
        }

        [AllowAnonymous]
        public ActionResult Ping()
        {
            return Content("ok");
        }
    }
}
