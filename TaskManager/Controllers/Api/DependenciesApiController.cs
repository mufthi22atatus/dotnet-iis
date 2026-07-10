using System.Web;
using System.Web.Http;
using TaskManager.Data;

namespace TaskManager.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/tasks/{taskId:int}/dependencies")]
    public class DependenciesApiController : ApiController
    {
        private SqlQueryService Sql => DependencyConfig.Resolve<SqlQueryService>();

        private int CurrentUserId =>
            HttpContext.Current?.Items["UserId"] is int id ? id : 0;

        [HttpGet, Route("")]
        public IHttpActionResult GetDependencies(int taskId)
        {
            return Ok(Sql.GetTaskDependencies(taskId));
        }

        [HttpPost, Route("")]
        public IHttpActionResult AddDependency(int taskId, [FromBody] DependencyBody body)
        {
            if (body == null || body.DependsOnTaskId <= 0)
                return BadRequest("Valid DependsOnTaskId is required.");

            var type = string.IsNullOrWhiteSpace(body.DependencyType) ? "BlockedBy" : body.DependencyType.Trim();

            // 1. Insert dependency
            var id = Sql.InsertTaskDependency(taskId, body.DependsOnTaskId, type, CurrentUserId);
            // 2. Insert history
            Sql.InsertTaskHistory(taskId, "Dependencies", null, body.DependsOnTaskId.ToString(), CurrentUserId);
            // 3. Insert audit log
            Sql.InsertAuditLogDirect("task.dependency.add", CurrentUserId,
                $"Added dependency on task {body.DependsOnTaskId} to task {taskId}", "TaskItem", taskId.ToString());

            return Ok(new { Id = id });
        }

        [HttpDelete, Route("{dependsOnId:int}")]
        public IHttpActionResult RemoveDependency(int taskId, int dependsOnId)
        {
            // 1. Delete dependency
            Sql.DeleteTaskDependency(taskId, dependsOnId);
            // 2. Insert history
            Sql.InsertTaskHistory(taskId, "Dependencies", dependsOnId.ToString(), null, CurrentUserId);
            // 3. Insert audit log
            Sql.InsertAuditLogDirect("task.dependency.remove", CurrentUserId,
                $"Removed dependency on task {dependsOnId} from task {taskId}", "TaskItem", taskId.ToString());

            return Ok();
        }

        public class DependencyBody
        {
            public int DependsOnTaskId { get; set; }
            public string DependencyType { get; set; }
        }
    }
}
