using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Extensions.Logging;
using TaskManager.Helpers;
using TaskManager.Services;
using TaskManager.ViewModels;

namespace TaskManager.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private IAuthService Auth => DependencyConfig.Resolve<IAuthService>();

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            if (Request.IsAuthenticated) return RedirectToLocal(returnUrl);
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var ip = Request.GetClientIp();
            var result = await Auth.AuthenticateAsync(model.Email, model.Password, ip);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "Login failed");
                return View(model);
            }

            IssueAuthCookie(result.Employee.Id, result.Employee.Email, result.Employee.Role, model.RememberMe);
            Session["EmployeeId"] = result.Employee.Id;
            Session["EmployeeName"] = result.Employee.FullName;
            Session["Role"] = result.Employee.Role;

            return RedirectToLocal(model.ReturnUrl);
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await Auth.RegisterAsync(model.FullName, model.Email, model.Password, model.Department);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "Registration failed");
                return View(model);
            }

            IssueAuthCookie(result.Employee.Id, result.Employee.Email, result.Employee.Role, false);
            Session["EmployeeId"] = result.Employee.Id;
            Session["EmployeeName"] = result.Employee.FullName;
            Session["Role"] = result.Employee.Role;

            return RedirectToAction("Index", "Tasks");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            AppLogger.Create<AccountController>()?.LogInformation("User signed out");
            return RedirectToAction("Login");
        }

        private void IssueAuthCookie(int employeeId, string email, string role, bool persist)
        {
            var ticket = new FormsAuthenticationTicket(
                version: 2,
                name: email,
                issueDate: DateTime.UtcNow,
                expiration: DateTime.UtcNow.AddHours(persist ? 24 * 14 : 2),
                isPersistent: persist,
                userData: $"{employeeId}|{role}",
                cookiePath: FormsAuthentication.FormsCookiePath);

            var encrypted = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted)
            {
                HttpOnly = true,
                Secure = Request.IsSecureConnection,
                Expires = persist ? ticket.Expiration : DateTime.MinValue
            };
            Response.Cookies.Add(cookie);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Tasks");
        }
    }
}
