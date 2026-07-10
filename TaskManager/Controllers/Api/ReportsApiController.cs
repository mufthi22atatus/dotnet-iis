using System;
using System.Web.Http;
using TaskManager.Data;

namespace TaskManager.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/reports")]
    public class ReportsApiController : ApiController
    {
        private SqlQueryService Sql => DependencyConfig.Resolve<SqlQueryService>();

        [HttpGet, Route("daily")]
        public IHttpActionResult Daily(DateTime? date = null)
        {
            var targetDate = date ?? DateTime.UtcNow;
            return Ok(Sql.GetDailyReport(targetDate));
        }

        [HttpGet, Route("weekly")]
        public IHttpActionResult Weekly(DateTime? weekStart = null)
        {
            var targetWeekStart = weekStart ?? DateTime.UtcNow.AddDays(-7);
            return Ok(Sql.GetWeeklyReport(targetWeekStart));
        }

        [HttpGet, Route("monthly")]
        public IHttpActionResult Monthly(int? year = null, int? month = null)
        {
            var now = DateTime.UtcNow;
            var targetYear = year ?? now.Year;
            var targetMonth = month ?? now.Month;
            return Ok(Sql.GetMonthlyReport(targetYear, targetMonth));
        }

        [HttpGet, Route("user-productivity")]
        public IHttpActionResult UserProductivity(int? userId = null)
        {
            return Ok(Sql.GetUserProductivity(userId));
        }

        [HttpGet, Route("project-summary")]
        public IHttpActionResult ProjectSummary(int? projectId = null)
        {
            return Ok(Sql.GetProjectSummary(projectId));
        }

        [HttpGet, Route("time-tracking")]
        public IHttpActionResult TimeTracking(int? userId = null, int? taskId = null)
        {
            return Ok(Sql.GetTimeTrackingSummary(userId, taskId));
        }

        [HttpGet, Route("comprehensive")]
        public IHttpActionResult Comprehensive()
        {
            var now = DateTime.UtcNow;
            // Run all 6 reports in sequence -> produces 6 MSSQL spans!
            var daily = Sql.GetDailyReport(now);
            var weekly = Sql.GetWeeklyReport(now.AddDays(-7));
            var monthly = Sql.GetMonthlyReport(now.Year, now.Month);
            var userProductivity = Sql.GetUserProductivity();
            var projectSummary = Sql.GetProjectSummary();
            var timeTracking = Sql.GetTimeTrackingSummary();

            return Ok(new
            {
                Daily = daily,
                Weekly = weekly,
                Monthly = monthly,
                UserProductivity = userProductivity,
                ProjectSummary = projectSummary,
                TimeTracking = timeTracking
            });
        }
    }
}
