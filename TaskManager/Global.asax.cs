using System;
using System.Data.Entity;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TaskManager.Background;
using TaskManager.Data;
using Microsoft.Extensions.Logging;

namespace TaskManager
{
    public class MvcApplication : HttpApplication
    {
        public static ILoggerFactory LoggerFactory { get; private set; }
        public static ILogger<MvcApplication> Logger { get; private set; }

        protected void Application_Start()
        {


            LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                LoggingConfig.Configure(builder);
            });
            
            Logger = LoggerFactory.CreateLogger<MvcApplication>();
            Logger.LogInformation("Application starting up. Env={Env}", System.Configuration.ConfigurationManager.AppSettings["Environment"] ?? "Debug");

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DependencyConfig.Configure();

            // Disable EF model-check – schema is managed via db/01_schema.sql
            System.Data.Entity.Database.SetInitializer<AppDbContext>(null);
            using (var ctx = new AppDbContext())
            {
                // Verify DB connectivity on startup
                ctx.Database.Initialize(force: false);
            }

            BackgroundJobHost.Start();

            Logger.LogInformation("Application started successfully");
        }

        protected void Application_End()
        {
            try
            {
                BackgroundJobHost.Stop();
                Logger.LogInformation("Application shutting down.");
            }
            finally
            {
                LoggerFactory?.Dispose();
            }
        }

        protected void Application_Error()
        {
            Exception exception = Server.GetLastError();
            if (exception != null)
            {
                Logger.LogError(exception, "Unhandled application error");
            }
        }

        protected void Session_Start()
        {
            Session["StartedAt"] = DateTime.UtcNow;
        }

        protected void Application_PostAuthenticateRequest()
        {
            // Decode our FormsAuthenticationTicket UserData (employeeId|role) so MVC filters
            // and views can read role/id without re-querying the DB.
            var ctx = Context;
            if (ctx?.User?.Identity is System.Web.Security.FormsIdentity fid
                && fid.Ticket?.UserData is string ud
                && ud.Contains("|"))
            {
                var parts = ud.Split('|');
                if (parts.Length >= 2)
                {
                    if (int.TryParse(parts[0], out var id))
                        ctx.Items["UserId"] = id;
                    ctx.Items["UserRole"] = parts[1];
                }
            }
        }
    }
}
