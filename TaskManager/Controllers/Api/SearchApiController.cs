using System;
using System.Web.Http;
using TaskManager.Data;

namespace TaskManager.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/search")]
    public class SearchApiController : ApiController
    {
        private SqlQueryService Sql => DependencyConfig.Resolve<SqlQueryService>();

        [HttpGet, Route("tasks")]
        public IHttpActionResult Search(string keyword = "", int page = 1, int pageSize = 20)
        {
            return Ok(Sql.SearchTasks(keyword, page, pageSize));
        }

        [HttpGet, Route("by-status/{status:int}")]
        public IHttpActionResult ByStatus(int status, int page = 1, int pageSize = 20)
        {
            return Ok(Sql.FilterByStatus(status, page, pageSize));
        }

        [HttpGet, Route("by-priority/{priority:int}")]
        public IHttpActionResult ByPriority(int priority, int page = 1, int pageSize = 20)
        {
            return Ok(Sql.FilterByPriority(priority, page, pageSize));
        }

        [HttpGet, Route("by-assignee/{assigneeId:int}")]
        public IHttpActionResult ByAssignee(int assigneeId, int page = 1, int pageSize = 20)
        {
            return Ok(Sql.FilterByAssignee(assigneeId, page, pageSize));
        }

        [HttpGet, Route("by-project/{projectId:int}")]
        public IHttpActionResult ByProject(int projectId, int page = 1, int pageSize = 20)
        {
            return Ok(Sql.FilterByProject(projectId, page, pageSize));
        }

        [HttpGet, Route("by-date-range")]
        public IHttpActionResult ByDateRange(DateTime from, DateTime to, int page = 1, int pageSize = 20)
        {
            return Ok(Sql.FilterByDateRange(from, to, page, pageSize));
        }

        [HttpGet, Route("advanced")]
        public IHttpActionResult Advanced(int status, int priority, int projectId, int page = 1, int pageSize = 20)
        {
            // Call status filter + priority filter + project filter -> 3 MSSQL spans in one request!
            var statusFiltered = Sql.FilterByStatus(status, page, pageSize);
            var priorityFiltered = Sql.FilterByPriority(priority, page, pageSize);
            var projectFiltered = Sql.FilterByProject(projectId, page, pageSize);

            return Ok(new
            {
                ByStatus = statusFiltered,
                ByPriority = priorityFiltered,
                ByProject = projectFiltered
            });
        }
    }
}
