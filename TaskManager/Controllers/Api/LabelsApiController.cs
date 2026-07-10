using System.Web;
using System.Web.Http;
using TaskManager.Data;

namespace TaskManager.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/tasks/{taskId:int}/labels")]
    public class LabelsApiController : ApiController
    {
        private SqlQueryService Sql => DependencyConfig.Resolve<SqlQueryService>();

        private int CurrentUserId =>
            HttpContext.Current?.Items["UserId"] is int id ? id : 0;

        [HttpGet, Route("")]
        public IHttpActionResult GetLabels(int taskId)
        {
            return Ok(Sql.GetTaskLabels(taskId));
        }

        [HttpPost, Route("")]
        public IHttpActionResult AddLabel(int taskId, [FromBody] LabelBody body)
        {
            if (body == null || string.IsNullOrWhiteSpace(body.Label))
                return BadRequest("Label is required.");

            // 1. Insert label
            var id = Sql.InsertTaskLabel(taskId, body.Label.Trim(), CurrentUserId);
            // 2. Insert history
            Sql.InsertTaskHistory(taskId, "Labels", null, body.Label.Trim(), CurrentUserId);
            // 3. Insert audit log
            Sql.InsertAuditLogDirect("task.label.add", CurrentUserId,
                $"Added label '{body.Label}' to task {taskId}", "TaskItem", taskId.ToString());

            return Ok(new { Id = id, Label = body.Label });
        }

        [HttpDelete, Route("{label}")]
        public IHttpActionResult RemoveLabel(int taskId, string label)
        {
            if (string.IsNullOrWhiteSpace(label))
                return BadRequest("Label is required.");

            // 1. Delete label
            Sql.DeleteTaskLabel(taskId, label.Trim());
            // 2. Insert history
            Sql.InsertTaskHistory(taskId, "Labels", label.Trim(), null, CurrentUserId);
            // 3. Insert audit log
            Sql.InsertAuditLogDirect("task.label.remove", CurrentUserId,
                $"Removed label '{label}' from task {taskId}", "TaskItem", taskId.ToString());

            return Ok();
        }

        public class LabelBody { public string Label { get; set; } }
    }
}
