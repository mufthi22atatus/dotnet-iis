using System;
using System.Web;

namespace TaskManager.Modules
{
    public class SecurityHeadersModule : IHttpModule
    {
        public void Init(HttpApplication app)
        {
            app.PreSendRequestHeaders += OnPreSendHeaders;
        }

        private void OnPreSendHeaders(object sender, EventArgs e)
        {
            var ctx = HttpContext.Current;
            if (ctx == null) return;
            var headers = ctx.Response.Headers;

            void Set(string k, string v) { if (headers[k] == null) headers[k] = v; }

            Set("X-Content-Type-Options", "nosniff");
            Set("X-Frame-Options", "SAMEORIGIN");
            Set("Referrer-Policy", "strict-origin-when-cross-origin");
            Set("X-XSS-Protection", "0");
            headers.Remove("Server");
            headers.Remove("X-AspNet-Version");
            headers.Remove("X-AspNetMvc-Version");
            headers.Remove("X-Powered-By");
        }

        public void Dispose() { }
    }
}
