using System.Web;
using System.Web.Http;
using TaskManager.Data;

namespace TaskManager.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/tasks/{taskId:int}/watchers")]
    public class WatchersApiController : ApiController
    {
        private SqlQueryService Sql => DependencyConfig.Resolve<SqlQueryService>();

        private int CurrentUserId =>
            HttpContext.Current?.Items["UserId"] is int id ? id : 0;

        [HttpGet, Route("")]
        public IHttpActionResult GetWatchers(int taskId)
        {
            return Ok(Sql.GetTaskWatchers(taskId));
        }

        [HttpPost, Route("")]
        public IHttpActionResult AddWatcher(int taskId, [FromBody] WatcherBody body)
        {
            if (body == null || body.EmployeeId <= 0)
                return BadRequest("Valid EmployeeId is required.");

            // 1. Insert watcher
            var id = Sql.InsertTaskWatcher(taskId, body.EmployeeId);
            // 2. Insert history
            Sql.InsertTaskHistory(taskId, "Watchers", null, body.EmployeeId.ToString(), CurrentUserId);
            // 3. Insert audit log
            Sql.InsertAuditLogDirect("task.watcher.add", CurrentUserId,
                $"Added watcher {body.EmployeeId} to task {taskId}", "TaskItem", taskId.ToString());

            return Ok(new { Id = id });
        }

        [HttpDelete, Route("{employeeId:int}")]
        public IHttpActionResult RemoveWatcher(int taskId, int employeeId)
        {
            // 1. Delete watcher
            Sql.DeleteTaskWatcher(taskId, employeeId);
            // 2. Insert history
            Sql.InsertTaskHistory(taskId, "Watchers", employeeId.ToString(), null, CurrentUserId);
            // 3. Insert audit log
            Sql.InsertAuditLogDirect("task.watcher.remove", CurrentUserId,
                $"Removed watcher {employeeId} from task {taskId}", "TaskItem", taskId.ToString());

            return Ok();
        }

        public class WatcherBody { public int EmployeeId { get; set; } }
    }
}
