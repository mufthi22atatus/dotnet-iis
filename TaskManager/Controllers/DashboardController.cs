using System.Threading.Tasks;
using System.Web.Mvc;
using TaskManager.Services;

namespace TaskManager.Controllers
{
    public class DashboardController : AppControllerBase
    {
        private ITaskService Tasks => DependencyConfig.Resolve<ITaskService>();
        private IExternalApiClient Ext => DependencyConfig.Resolve<IExternalApiClient>();

        public async Task<ActionResult> Index()
        {
            var summary = await Tasks.BuildDashboardAsync();
            ViewBag.Weather = await TryGetWeatherAsync();
            return View(summary);
        }

        private async Task<WeatherSnapshot> TryGetWeatherAsync()
        {
            try { return await Ext.GetWeatherAsync(13.0827, 80.2707); /* Chennai */ }
            catch { return null; }
        }
    }
}
