using System;
using System.Web;
using System.Web.Http;
using TaskManager.Data;

namespace TaskManager.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/tasks/{taskId:int}/timelogs")]
    public class TimeLogsApiController : ApiController
    {
        private SqlQueryService Sql => DependencyConfig.Resolve<SqlQueryService>();

        private int CurrentUserId =>
            HttpContext.Current?.Items["UserId"] is int id ? id : 0;

        [HttpGet, Route("")]
        public IHttpActionResult GetTimeLogs(int taskId)
        {
            return Ok(Sql.GetTaskTimeLogs(taskId));
        }

        [HttpPost, Route("start")]
        public IHttpActionResult StartTimeLog(int taskId, [FromBody] StartLogBody body)
        {
            var startedAt = body?.StartedAt ?? DateTime.UtcNow;
            var desc = body?.Description ?? "Time log started";

            // 1. Insert time log
            var id = Sql.InsertTimeLog(taskId, CurrentUserId, startedAt, null, 0, desc);
            // 2. Insert history
            Sql.InsertTaskHistory(taskId, "TimeLogs", null, id.ToString(), CurrentUserId);
            // 3. Insert audit log
            Sql.InsertAuditLogDirect("task.timelog.start", CurrentUserId,
                $"Started time log {id} for task {taskId}", "TaskItem", taskId.ToString());

            return Ok(new { Id = id, StartedAt = startedAt });
        }

        [HttpPost, Route("{logId:int}/stop")]
        public IHttpActionResult StopTimeLog(int taskId, int logId, [FromBody] StopLogBody body)
        {
            var stoppedAt = body?.StoppedAt ?? DateTime.UtcNow;
            var duration = body?.DurationMinutes ?? 0;

            // 1. Update time log
            Sql.UpdateTimeLogStop(logId, stoppedAt, duration);
            // 2. Insert history
            Sql.InsertTaskHistory(taskId, "TimeLogs", logId.ToString(), "Stopped", CurrentUserId);
            // 3. Insert audit log
            Sql.InsertAuditLogDirect("task.timelog.stop", CurrentUserId,
                $"Stopped time log {logId} for task {taskId} (Duration: {duration}m)", "TaskItem", taskId.ToString());

            return Ok();
        }

        public class StartLogBody
        {
            public DateTime? StartedAt { get; set; }
            public string Description { get; set; }
        }

        public class StopLogBody
        {
            public DateTime? StoppedAt { get; set; }
            public int DurationMinutes { get; set; }
        }
    }
}
