using System.Web;
using System.Web.Http;
using TaskManager.Data;

namespace TaskManager.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/tasks/{taskId:int}/attachments")]
    public class AttachmentsApiController : ApiController
    {
        private SqlQueryService Sql => DependencyConfig.Resolve<SqlQueryService>();

        private int CurrentUserId =>
            HttpContext.Current?.Items["UserId"] is int id ? id : 0;

        [HttpGet, Route("")]
        public IHttpActionResult GetAttachments(int taskId)
        {
            return Ok(Sql.GetTaskAttachments(taskId));
        }

        [HttpPost, Route("")]
        public IHttpActionResult AddAttachment(int taskId, [FromBody] AttachmentBody body)
        {
            if (body == null || string.IsNullOrWhiteSpace(body.FileName))
                return BadRequest("FileName is required.");

            var contentType = string.IsNullOrWhiteSpace(body.ContentType) ? "application/octet-stream" : body.ContentType;
            var path = string.IsNullOrWhiteSpace(body.StoredPath) ? "/uploads/" + body.FileName : body.StoredPath;

            // 1. Insert attachment metadata
            var id = Sql.InsertAttachment(taskId, body.FileName, contentType, path, body.SizeBytes, CurrentUserId);
            // 2. Insert history
            Sql.InsertTaskHistory(taskId, "Attachments", null, body.FileName, CurrentUserId);
            // 3. Insert audit log
            Sql.InsertAuditLogDirect("task.attachment.add", CurrentUserId,
                $"Added attachment '{body.FileName}' to task {taskId}", "TaskItem", taskId.ToString());

            return Ok(new { Id = id, FileName = body.FileName });
        }

        [HttpDelete, Route("{attachmentId:int}")]
        public IHttpActionResult RemoveAttachment(int taskId, int attachmentId)
        {
            // 1. Delete attachment metadata
            Sql.DeleteAttachment(attachmentId);
            // 2. Insert history
            Sql.InsertTaskHistory(taskId, "Attachments", attachmentId.ToString(), null, CurrentUserId);
            // 3. Insert audit log
            Sql.InsertAuditLogDirect("task.attachment.remove", CurrentUserId,
                $"Removed attachment {attachmentId} from task {taskId}", "TaskItem", taskId.ToString());

            return Ok();
        }

        public class AttachmentBody
        {
            public string FileName { get; set; }
            public string ContentType { get; set; }
            public string StoredPath { get; set; }
            public long SizeBytes { get; set; }
        }
    }
}
