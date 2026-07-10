using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Extensions.Logging;

namespace TaskManager.Controllers
{
    /// <summary>
    /// Diagnostics controller — exposes endpoints that deliberately produce errors,
    /// exceptions, slow responses, and external HTTP calls.  Useful for generating
    /// rich APM telemetry (error rates, exception traces, external spans).
    /// </summary>
    public class DiagnosticsController : AppControllerBase
    {
        private static readonly HttpClient Http = CreateClient();
        private static readonly Random Rng = new Random();

        private static HttpClient CreateClient()
        {
            var c = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
            c.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("TaskManager", "1.0"));
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return c;
        }

        // ────────────────────────── UI page ──────────────────────────
        public ActionResult Index()
        {
            return View();
        }

        // ────────────────────────── Error routes ──────────────────────────

        /// <summary>Returns a 500 Internal Server Error.</summary>
        public ActionResult Error500()
        {
            AppLogger.Create<DiagnosticsController>()?.LogWarning("Diagnostics: deliberate 500 triggered");
            throw new InvalidOperationException("Synthetic 500 — deliberate server error for APM testing");
        }

        /// <summary>Returns a 404 Not Found.</summary>
        public ActionResult Error404()
        {
            AppLogger.Create<DiagnosticsController>()?.LogWarning("Diagnostics: deliberate 404 triggered");
            return HttpNotFound("Synthetic 404 — page not found for APM testing");
        }

        /// <summary>Returns a 403 Forbidden.</summary>
        public ActionResult Error403()
        {
            AppLogger.Create<DiagnosticsController>()?.LogWarning("Diagnostics: deliberate 403 triggered");
            return new HttpStatusCodeResult(403, "Synthetic 403 — forbidden for APM testing");
        }

        /// <summary>Returns a 400 Bad Request.</summary>
        public ActionResult Error400()
        {
            AppLogger.Create<DiagnosticsController>()?.LogWarning("Diagnostics: deliberate 400 triggered");
            return new HttpStatusCodeResult(400, "Synthetic 400 — bad request for APM testing");
        }

        // ────────────────────────── Failure-rate simulation ──────────────────────────

        /// <summary>
        /// Succeeds or fails randomly based on the given failure percentage (default 50%).
        /// Useful for generating realistic error-rate metrics.
        /// </summary>
        public ActionResult RandomFailure(int failPercent = 50)
        {
            failPercent = Math.Max(0, Math.Min(100, failPercent));
            var roll = Rng.Next(100);

            if (roll < failPercent)
            {
                AppLogger.Create<DiagnosticsController>()?.LogWarning("Diagnostics: RandomFailure rolled {Roll} (< {Pct}%) — failing", roll, failPercent);
                throw new ApplicationException(
                    $"Random failure triggered (rolled {roll}, threshold {failPercent}%)");
            }

            AppLogger.Create<DiagnosticsController>()?.LogInformation("Diagnostics: RandomFailure rolled {Roll} (>= {Pct}%) — success", roll, failPercent);
            ViewBag.Message = $"Success! Rolled {roll} (threshold was {failPercent}%)";
            return View("Result");
        }

        // ────────────────────────── Exception types ──────────────────────────

        /// <summary>Throws a NullReferenceException.</summary>
        public ActionResult NullRef()
        {
            AppLogger.Create<DiagnosticsController>()?.LogWarning("Diagnostics: deliberate NullReferenceException");
            string s = null;
            // Deliberately dereference null
            var len = s.Length;
            return Content(len.ToString());
        }

        /// <summary>Throws an ArgumentException.</summary>
        public ActionResult ArgError()
        {
            AppLogger.Create<DiagnosticsController>()?.LogWarning("Diagnostics: deliberate ArgumentException");
            throw new ArgumentException("Bad argument value for APM testing", "diagnosticParam");
        }

        /// <summary>Throws a DivideByZeroException.</summary>
        public ActionResult DivideByZero()
        {
            AppLogger.Create<DiagnosticsController>()?.LogWarning("Diagnostics: deliberate DivideByZeroException");
            var x = 0;
            var y = 1 / x;
            return Content(y.ToString());
        }

        /// <summary>Throws a TimeoutException.</summary>
        public ActionResult Timeout()
        {
            AppLogger.Create<DiagnosticsController>()?.LogWarning("Diagnostics: deliberate TimeoutException");
            throw new TimeoutException("Simulated timeout — operation took too long");
        }

        /// <summary>Throws an UnauthorizedAccessException.</summary>
        public ActionResult Unauthorized()
        {
            AppLogger.Create<DiagnosticsController>()?.LogWarning("Diagnostics: deliberate UnauthorizedAccessException");
            throw new UnauthorizedAccessException("Simulated unauthorized access for APM testing");
        }

        /// <summary>Nested exception with inner exception chain.</summary>
        public ActionResult NestedError()
        {
            AppLogger.Create<DiagnosticsController>()?.LogWarning("Diagnostics: deliberate nested exception");
            try
            {
                throw new InvalidOperationException("Root cause: database connection failed");
            }
            catch (Exception inner)
            {
                throw new ApplicationException("Service layer failure wrapping DB error", inner);
            }
        }

        // ────────────────────────── Slow responses ──────────────────────────

        /// <summary>Simulates a slow response (configurable delay in ms, default 3000).</summary>
        public async Task<ActionResult> SlowResponse(int delayMs = 3000)
        {
            delayMs = Math.Max(100, Math.Min(30000, delayMs));
            AppLogger.Create<DiagnosticsController>()?.LogInformation("Diagnostics: SlowResponse sleeping for {Delay}ms", delayMs);
            await System.Threading.Tasks.Task.Delay(delayMs);

            ViewBag.Message = $"Responded after {delayMs}ms delay";
            return View("Result");
        }

        // ────────────────────────── External HTTP requests ──────────────────────────

        /// <summary>Calls the Open-Meteo weather API.</summary>
        public async Task<ActionResult> ExternalWeather(double lat = 13.0827, double lon = 80.2707)
        {
            var url = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current_weather=true";
            AppLogger.Create<DiagnosticsController>()?.LogInformation("Diagnostics: external GET {Url}", url);

            using (var resp = await Http.GetAsync(url))
            {
                var body = await resp.Content.ReadAsStringAsync();
                ViewBag.Message = $"Weather API returned {resp.StatusCode}";
                ViewBag.ResponseBody = body;
                return View("ExternalResult");
            }
        }

        /// <summary>Calls the GitHub API for repo info.</summary>
        public async Task<ActionResult> ExternalGitHub(string owner = "dotnet", string repo = "runtime")
        {
            var url = $"https://api.github.com/repos/{Uri.EscapeDataString(owner)}/{Uri.EscapeDataString(repo)}";
            AppLogger.Create<DiagnosticsController>()?.LogInformation("Diagnostics: external GET {Url}", url);

            using (var req = new HttpRequestMessage(HttpMethod.Get, url))
            {
                req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
                using (var resp = await Http.SendAsync(req))
                {
                    var body = await resp.Content.ReadAsStringAsync();
                    ViewBag.Message = $"GitHub API returned {resp.StatusCode}";
                    ViewBag.ResponseBody = body;
                    return View("ExternalResult");
                }
            }
        }

        /// <summary>Calls JSONPlaceholder — a free fake REST API.</summary>
        public async Task<ActionResult> ExternalJsonPlaceholder()
        {
            var url = "https://jsonplaceholder.typicode.com/posts/1";
            AppLogger.Create<DiagnosticsController>()?.LogInformation("Diagnostics: external GET {Url}", url);

            using (var resp = await Http.GetAsync(url))
            {
                var body = await resp.Content.ReadAsStringAsync();
                ViewBag.Message = $"JSONPlaceholder returned {resp.StatusCode}";
                ViewBag.ResponseBody = body;
                return View("ExternalResult");
            }
        }

        /// <summary>Calls httpbin.org to test external request tracing.</summary>
        public async Task<ActionResult> ExternalHttpBin()
        {
            var url = "https://httpbin.org/delay/2";
            AppLogger.Create<DiagnosticsController>()?.LogInformation("Diagnostics: external GET {Url}", url);

            using (var resp = await Http.GetAsync(url))
            {
                var body = await resp.Content.ReadAsStringAsync();
                ViewBag.Message = $"httpbin.org returned {resp.StatusCode}";
                ViewBag.ResponseBody = body;
                return View("ExternalResult");
            }
        }

        /// <summary>Calls a non-existent domain to generate a failed external request.</summary>
        public async Task<ActionResult> ExternalFailing()
        {
            var url = "https://this-domain-does-not-exist-abc123.com/api/test";
            AppLogger.Create<DiagnosticsController>()?.LogWarning("Diagnostics: external GET to failing URL {Url}", url);

            try
            {
                using (var resp = await Http.GetAsync(url))
                {
                    var body = await resp.Content.ReadAsStringAsync();
                    ViewBag.Message = $"Unexpected success: {resp.StatusCode}";
                    ViewBag.ResponseBody = body;
                    return View("ExternalResult");
                }
            }
            catch (Exception ex)
            {
                AppLogger.Create<DiagnosticsController>()?.LogError(ex, "Diagnostics: external request to failing URL threw");
                throw;
            }
        }
    }
}
