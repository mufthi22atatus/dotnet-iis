using System.Web;
using System.Web.Http;
using TaskManager.Data;

namespace TaskManager.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/tasks/{taskId:int}/comments")]
    public class CommentsApiController : ApiController
    {
        private SqlQueryService Sql => DependencyConfig.Resolve<SqlQueryService>();

        private int CurrentUserId =>
            HttpContext.Current?.Items["UserId"] is int id ? id : 0;

        [HttpGet, Route("")]
        public IHttpActionResult GetComments(int taskId)
        {
            return Ok(Sql.GetTaskComments(taskId));
        }

        [HttpPost, Route("")]
        public IHttpActionResult AddComment(int taskId, [FromBody] CommentBody body)
        {
            if (body == null || string.IsNullOrWhiteSpace(body.Body))
                return BadRequest("Comment body is required.");

            // 1. Insert comment
            var id = Sql.InsertComment(taskId, CurrentUserId, body.Body.Trim());
            // 2. Insert history
            Sql.InsertTaskHistory(taskId, "Comments", null, id.ToString(), CurrentUserId);
            // 3. Insert audit log
            Sql.InsertAuditLogDirect("task.comment.add", CurrentUserId,
                $"Added comment {id} to task {taskId}", "TaskItem", taskId.ToString());

            return Ok(new { Id = id, Body = body.Body });
        }

        [HttpPut, Route("{commentId:int}")]
        public IHttpActionResult EditComment(int taskId, int commentId, [FromBody] CommentBody body)
        {
            if (body == null || string.IsNullOrWhiteSpace(body.Body))
                return BadRequest("Comment body is required.");

            // 1. Update comment
            Sql.UpdateComment(commentId, body.Body.Trim());
            // 2. Insert history
            Sql.InsertTaskHistory(taskId, "Comments", commentId.ToString(), "Edited", CurrentUserId);
            // 3. Insert audit log
            Sql.InsertAuditLogDirect("task.comment.edit", CurrentUserId,
                $"Edited comment {commentId} on task {taskId}", "TaskItem", taskId.ToString());

            return Ok();
        }

        [HttpDelete, Route("{commentId:int}")]
        public IHttpActionResult DeleteComment(int taskId, int commentId)
        {
            // 1. Delete comment
            Sql.DeleteComment(commentId);
            // 2. Insert history
            Sql.InsertTaskHistory(taskId, "Comments", commentId.ToString(), null, CurrentUserId);
            // 3. Insert audit log
            Sql.InsertAuditLogDirect("task.comment.delete", CurrentUserId,
                $"Deleted comment {commentId} from task {taskId}", "TaskItem", taskId.ToString());

            return Ok();
        }

        public class CommentBody { public string Body { get; set; } }
    }
}
