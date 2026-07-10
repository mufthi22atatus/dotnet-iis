using System.Collections.Generic;
using System.Web.Http;
using TaskManager.Data;

namespace TaskManager.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/dashboard")]
    public class DashboardApiController : ApiController
    {
        private SqlQueryService Sql => DependencyConfig.Resolve<SqlQueryService>();

        [HttpGet, Route("summary")]
        public IHttpActionResult GetSummary()
        {
            // Call all 8 methods -> generates 8 distinct MSSQL spans in one request!
            var openCount = Sql.GetOpenTasksCount();
            var closedCount = Sql.GetClosedTasksCount();
            var overdueCount = Sql.GetOverdueTasksCount();
            var highPriorityCount = Sql.GetHighPriorityTasksCount();
            var byStatus = Sql.GetTasksByStatusBreakdown();
            var byUser = Sql.GetTasksByUser();
            var recentActivities = Sql.GetRecentActivities();
            var recentComments = Sql.GetRecentCommentsDetailed();

            return Ok(new
            {
                OpenCount = openCount,
                ClosedCount = closedCount,
                OverdueCount = overdueCount,
                HighPriorityCount = highPriorityCount,
                ByStatus = byStatus,
                ByUser = byUser,
                RecentActivities = recentActivities,
                RecentComments = recentComments
            });
        }

        [HttpGet, Route("open-count")]
        public IHttpActionResult GetOpenCount()
        {
            return Ok(Sql.GetOpenTasksCount());
        }

        [HttpGet, Route("closed-count")]
        public IHttpActionResult GetClosedCount()
        {
            return Ok(Sql.GetClosedTasksCount());
        }

        [HttpGet, Route("overdue-count")]
        public IHttpActionResult GetOverdueCount()
        {
            return Ok(Sql.GetOverdueTasksCount());
        }

        [HttpGet, Route("high-priority-count")]
        public IHttpActionResult GetHighPriorityCount()
        {
            return Ok(Sql.GetHighPriorityTasksCount());
        }

        [HttpGet, Route("by-status")]
        public IHttpActionResult GetByStatus()
        {
            return Ok(Sql.GetTasksByStatusBreakdown());
        }

        [HttpGet, Route("by-user")]
        public IHttpActionResult GetByUser()
        {
            return Ok(Sql.GetTasksByUser());
        }

        [HttpGet, Route("recent-activities")]
        public IHttpActionResult GetRecentActivities()
        {
            return Ok(Sql.GetRecentActivities());
        }

        [HttpGet, Route("recent-comments")]
        public IHttpActionResult GetRecentComments()
        {
            return Ok(Sql.GetRecentCommentsDetailed());
        }
    }
}
