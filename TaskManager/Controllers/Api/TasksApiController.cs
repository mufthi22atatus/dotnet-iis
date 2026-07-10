using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TaskManager.Data.Entities;
using TaskManager.Services;
using TaskStatus = TaskManager.Data.Entities.TaskStatus;
using TaskManager.ViewModels;

namespace TaskManager.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/tasks")]
    public class TasksApiController : ApiController
    {
        private ITaskService Tasks => DependencyConfig.Resolve<ITaskService>();

        private int CurrentUserId =>
            HttpContext.Current?.Items["UserId"] is int id ? id : 0;

        [HttpGet, Route("")]
        public async Task<IHttpActionResult> List(bool includeDone = false)
        {
            var data = await Tasks.ListForUserAsync(CurrentUserId, includeDone);
            return Ok(data);
        }

        [HttpGet, Route("{id:int}")]
        public async Task<IHttpActionResult> Get(int id)
        {
            var t = await Tasks.GetAsync(id);
            return t == null ? (IHttpActionResult)NotFound() : Ok(t);
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create([FromBody] TaskCreateInput input)
        {
            if (input == null || !ModelState.IsValid) return BadRequest(ModelState);
            var created = await Tasks.CreateAsync(input, CurrentUserId);
            return Created($"/api/tasks/{created.Id}", created);
        }

        [HttpPut, Route("{id:int}")]
        public async Task<IHttpActionResult> Update(int id, [FromBody] TaskUpdateInput input)
        {
            if (input == null || !ModelState.IsValid) return BadRequest(ModelState);
            var updated = await Tasks.UpdateAsync(id, input, CurrentUserId);
            return updated == null ? (IHttpActionResult)NotFound() : Ok(updated);
        }

        [HttpDelete, Route("{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var ok = await Tasks.DeleteAsync(id, CurrentUserId);
            return ok ? (IHttpActionResult)Ok() : NotFound();
        }

        [HttpPost, Route("{id:int}/assign/{assigneeId:int}")]
        public async Task<IHttpActionResult> Assign(int id, int assigneeId)
        {
            var t = await Tasks.AssignAsync(id, assigneeId, CurrentUserId);
            return t == null ? (IHttpActionResult)NotFound() : Ok(t);
        }

        [HttpPost, Route("{id:int}/status")]
        public async Task<IHttpActionResult> Status(int id, [FromBody] StatusBody body)
        {
            if (body == null) return BadRequest("status required");
            var t = await Tasks.ChangeStatusAsync(id, body.Status, CurrentUserId);
            return t == null ? (IHttpActionResult)NotFound() : Ok(t);
        }

        public class StatusBody { public TaskStatus Status { get; set; } }
    }
}
