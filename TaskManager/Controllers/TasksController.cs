using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Extensions.Logging;
using TaskManager.Data;
using TaskManager.Data.Entities;
using TaskManager.Helpers;
using TaskManager.Services;
using TaskManager.ViewModels;
using TaskStatus = TaskManager.Data.Entities.TaskStatus;
namespace TaskManager.Controllers
{
    public class TasksController : AppControllerBase
    {
        private ITaskService Tasks => DependencyConfig.Resolve<ITaskService>();
        private IFileStorageService Files => DependencyConfig.Resolve<IFileStorageService>();
        private IAuditService Audit => DependencyConfig.Resolve<IAuditService>();

        public async Task<ActionResult> Index(bool includeDone = false)
        {

            var list = IsManager
                ? await Tasks.ListAllAsync()
                : await Tasks.ListForUserAsync(CurrentUserId, includeDone);

            ViewBag.IncludeDone = includeDone;
            ViewBag.IsManager = IsManager;
            return View(list);
        }

        public async Task<ActionResult> Details(int id)
        {
            var task = await Tasks.GetAsync(id);
            if (task == null) return HttpNotFound();
            return View(task);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            await PopulateAssigneeListAsync(null);
            return View(new TaskCreateInput());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TaskCreateInput input)
        {
            if (!ModelState.IsValid)
            {
                await PopulateAssigneeListAsync(input.AssignedToId);
                return View(input);
            }

            var task = await Tasks.CreateAsync(input, CurrentUserId);
            return RedirectToAction("Details", new { id = task.Id });
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var task = await Tasks.GetAsync(id);
            if (task == null) return HttpNotFound();
            if (!CanEdit(task)) return new HttpStatusCodeResult(403);

            var input = new TaskUpdateInput
            {
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                Tag = task.Tag,
                EstimatedHours = task.EstimatedHours,
                LoggedHours = task.LoggedHours
            };
            ViewBag.TaskId = id;
            return View(input);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, TaskUpdateInput input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.TaskId = id;
                return View(input);
            }

            var existing = await Tasks.GetAsync(id);
            if (existing == null) return HttpNotFound();
            if (!CanEdit(existing)) return new HttpStatusCodeResult(403);

            var updated = await Tasks.UpdateAsync(id, input, CurrentUserId);
            if (updated == null) return HttpNotFound();
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var existing = await Tasks.GetAsync(id);
            if (existing == null) return HttpNotFound();
            if (!IsManager && existing.CreatedById != CurrentUserId)
                return new HttpStatusCodeResult(403);

            await Tasks.DeleteAsync(id, CurrentUserId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeStatus(int id, TaskStatus status)
        {
            var task = await Tasks.GetAsync(id);
            if (task == null) return HttpNotFound();
            if (!CanEdit(task)) return new HttpStatusCodeResult(403);

            await Tasks.ChangeStatusAsync(id, status, CurrentUserId);
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(int id, HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                TempData["UploadError"] = "Pick a file first.";
                return RedirectToAction("Details", new { id });
            }

            try
            {
                using (var stream = file.InputStream)
                {
                    var stored = await Files.SaveAsync(file.FileName, file.ContentType, stream);

                    using (var ctx = new AppDbContext())
                    {
                        var attachment = new TaskAttachment
                        {
                            TaskItemId = id,
                            FileName = stored.FileName,
                            ContentType = stored.ContentType,
                            StoredPath = stored.StoredPath,
                            SizeBytes = stored.SizeBytes,
                            UploadedById = CurrentUserId,
                            UploadedAt = DateTime.UtcNow
                        };
                        ctx.TaskAttachments.Add(attachment);
                        await ctx.SaveChangesAsync();
                    }

                    await Audit.RecordAsync("attachment.upload", CurrentUserId, null, Request.GetClientIp(),
                        $"Uploaded {stored.FileName} ({stored.SizeBytes}b)", "TaskItem", id.ToString());
                }
            }
            catch (Exception ex)
            {
                AppLogger.Create<TasksController>()?.LogError(ex, "Upload failed for task {Id}", id);
                TempData["UploadError"] = ex.Message;
            }

            return RedirectToAction("Details", new { id });
        }

        public async Task<ActionResult> DownloadAttachment(int attachmentId)
        {
            using (var ctx = new AppDbContext())
            {
                var attachment = await ctx.TaskAttachments.FindAsync(attachmentId);
                if (attachment == null) return HttpNotFound();
                var stream = Files.OpenRead(attachment.StoredPath);
                return File(stream, attachment.ContentType, attachment.FileName);
            }
        }

        private bool CanEdit(TaskItem t)
            => IsManager || t.CreatedById == CurrentUserId || t.AssignedToId == CurrentUserId;

        private async Task PopulateAssigneeListAsync(int? selectedId)
        {
            using (var ctx = new AppDbContext())
            {
                var users = await System.Data.Entity.QueryableExtensions.ToArrayAsync(
                    ctx.Employees.Where(e => e.IsActive).OrderBy(e => e.FullName));
                ViewBag.Assignees = new SelectList(
                    users.Select(u => new { u.Id, u.FullName }),
                    "Id", "FullName", selectedId);
            }
        }
    }
}
