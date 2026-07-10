using System;
using System.Diagnostics;
using System.Web;
using Microsoft.Extensions.Logging;
using TaskManager.Helpers;

namespace TaskManager.Modules
{
    /// <summary>
    /// IIS HTTP module that logs every request through the pipeline. Generates one log line
    /// per request including method, path, status, and elapsed ms — useful baseline for APM
    /// correlation.
    /// </summary>
    public class RequestLoggingModule : IHttpModule
    {
        private const string TimerKey = "__req_timer";
        private const string CorrelationKey = "__req_correlation";

        public void Init(HttpApplication app)
        {
            app.BeginRequest += OnBeginRequest;
            app.EndRequest += OnEndRequest;
            app.Error += OnError;
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            var ctx = ((HttpApplication)sender).Context;
            ctx.Items[TimerKey] = Stopwatch.StartNew();
            var corr = Guid.NewGuid().ToString("N").Substring(0, 12);
            ctx.Items[CorrelationKey] = corr;
            ctx.Response.Headers["X-Request-Id"] = corr;
        }

        private void OnEndRequest(object sender, EventArgs e)
        {
            var ctx = ((HttpApplication)sender).Context;
            if (ctx.Items[TimerKey] is Stopwatch sw)
            {
                sw.Stop();
                AppLogger.Create<RequestLoggingModule>()?.LogInformation("HTTP {Method} {Path} from {Ip} -> {Status} in {Ms}ms cid={Cid}",
                    ctx.Request.HttpMethod,
                    ctx.Request.Url.PathAndQuery,
                    ctx.Request.GetClientIp(),
                    ctx.Response.StatusCode,
                    sw.ElapsedMilliseconds,
                    ctx.Items[CorrelationKey]);
            }
        }

        private void OnError(object sender, EventArgs e)
        {
            var ctx = ((HttpApplication)sender).Context;
            var ex = ctx.Server.GetLastError();
            if (ex != null)
            {
                AppLogger.Create<RequestLoggingModule>()?.LogError(ex, "Pipeline error on {Method} {Path}: {Msg}",
                    ctx.Request.HttpMethod, ctx.Request.Url.PathAndQuery, ex.Message);
            }
        }

        public void Dispose() { }
    }
}
