using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TaskManager.Data;
using TaskManager.Filters;
using TaskManager.ViewModels;

namespace TaskManager.Controllers
{
    [AuthorizeRole("Admin")]
    public class UsersController : AppControllerBase
    {
        public async Task<ActionResult> Index()
        {
            using (var ctx = new AppDbContext())
            {
                var users = await System.Data.Entity.QueryableExtensions.ToArrayAsync(
                    ctx.Employees.OrderBy(e => e.FullName));
                return View(users);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            using (var ctx = new AppDbContext())
            {
                var u = await ctx.Employees.FindAsync(id);
                if (u == null) return HttpNotFound();
                return View(new UserEditInput
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Department = u.Department,
                    Role = u.Role,
                    IsActive = u.IsActive
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserEditInput input)
        {
            if (!ModelState.IsValid) return View(input);

            using (var ctx = new AppDbContext())
            {
                var u = await ctx.Employees.FindAsync(input.Id);
                if (u == null) return HttpNotFound();

                u.FullName = input.FullName;
                u.Email = input.Email;
                u.Department = input.Department;
                u.Role = input.Role;
                u.IsActive = input.IsActive;
                await ctx.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disable(int id)
        {
            if (id == CurrentUserId) return new HttpStatusCodeResult(400, "Cannot disable yourself");
            using (var ctx = new AppDbContext())
            {
                var u = await ctx.Employees.FindAsync(id);
                if (u == null) return HttpNotFound();
                u.IsActive = false;
                await ctx.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
