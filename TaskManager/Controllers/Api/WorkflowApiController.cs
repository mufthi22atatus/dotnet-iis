using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TaskManager.Services;

namespace TaskManager.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/workflow")]
    public class WorkflowApiController : ApiController
    {
        private ITaskWorkflowService Workflow => DependencyConfig.Resolve<ITaskWorkflowService>();

        private int CurrentUserId =>
            HttpContext.Current?.Items["UserId"] is int id ? id : 0;

        [HttpPost, Route("create-full-task")]
        public async Task<IHttpActionResult> CreateFullTask([FromBody] CreateFullTaskInput input)
        {
            if (input == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await Workflow.CreateFullTaskAsync(input, CurrentUserId);
            return Ok(result);
        }

        [HttpPost, Route("{id:int}/close")]
        public async Task<IHttpActionResult> CloseTask(int id)
        {
            var result = await Workflow.CloseTaskAsync(id, CurrentUserId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost, Route("{id:int}/reopen")]
        public async Task<IHttpActionResult> ReopenTask(int id)
        {
            var result = await Workflow.ReopenTaskAsync(id, CurrentUserId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost, Route("{id:int}/assign/{assigneeId:int}")]
        public async Task<IHttpActionResult> AssignTask(int id, int assigneeId)
        {
            var result = await Workflow.AssignTaskAsync(id, assigneeId, CurrentUserId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost, Route("{id:int}/reassign/{assigneeId:int}")]
        public async Task<IHttpActionResult> ReassignTask(int id, int assigneeId)
        {
            var result = await Workflow.ReassignTaskAsync(id, assigneeId, CurrentUserId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost, Route("{id:int}/change-priority")]
        public async Task<IHttpActionResult> ChangePriority(int id, [FromBody] PriorityBody body)
        {
            if (body == null) return BadRequest("Priority required.");
            var result = await Workflow.ChangePriorityAsync(id, body.Priority, CurrentUserId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost, Route("{id:int}/change-status")]
        public async Task<IHttpActionResult> ChangeStatus(int id, [FromBody] StatusBody body)
        {
            if (body == null) return BadRequest("Status required.");
            var result = await Workflow.ChangeStatusAsync(id, body.Status, CurrentUserId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost, Route("{id:int}/change-due-date")]
        public async Task<IHttpActionResult> ChangeDueDate(int id, [FromBody] DueDateBody body)
        {
            if (body == null) return BadRequest("DueDate required.");
            var result = await Workflow.ChangeDueDateAsync(id, body.DueDate, CurrentUserId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost, Route("bulk-update")]
        public async Task<IHttpActionResult> BulkUpdate([FromBody] BulkUpdateInput input)
        {
            if (input == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await Workflow.BulkUpdateTasksAsync(input, CurrentUserId);
            return Ok(result);
        }

        public class PriorityBody { public int Priority { get; set; } }
        public class StatusBody { public int Status { get; set; } }
        public class DueDateBody { public string DueDate { get; set; } }
    }
}
