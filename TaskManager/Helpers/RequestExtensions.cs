using System;
using System.Web;

namespace TaskManager.Helpers
{
    public static class RequestExtensions
    {
        public static string GetClientIp(this HttpRequestBase request)
        {
            if (request == null) return string.Empty;

            /*
            // Check X-Forwarded-For header first (can contain multiple IPs comma-separated)
            var xForwardedFor = request.Headers["X-Forwarded-For"];
            if (!string.IsNullOrEmpty(xForwardedFor))
            {
                var parts = xForwardedFor.Split(',');
                if (parts.Length > 0)
                {
                    var ip = parts[0].Trim();
                    if (!string.IsNullOrEmpty(ip))
                    {
                        return ip;
                    }
                }
            }

            // Check X-Real-IP header next
            var xRealIp = request.Headers["X-Real-IP"];
            if (!string.IsNullOrEmpty(xRealIp))
            {
                return xRealIp;
            }
            */

            // Fallback to ServerVariables or UserHostAddress
            var remoteAddr = request.ServerVariables["REMOTE_ADDR"];
            if (!string.IsNullOrEmpty(remoteAddr))
            {
                return remoteAddr;
            }

            return request.UserHostAddress;
        }

        public static string GetClientIp(this HttpRequest request)
        {
            if (request == null) return string.Empty;

            /*
            var xForwardedFor = request.Headers["X-Forwarded-For"];
            if (!string.IsNullOrEmpty(xForwardedFor))
            {
                var parts = xForwardedFor.Split(',');
                if (parts.Length > 0)
                {
                    var ip = parts[0].Trim();
                    if (!string.IsNullOrEmpty(ip))
                    {
                        return ip;
                    }
                }
            }

            var xRealIp = request.Headers["X-Real-IP"];
            if (!string.IsNullOrEmpty(xRealIp))
            {
                return xRealIp;
            }
            */

            var remoteAddr = request.ServerVariables["REMOTE_ADDR"];
            if (!string.IsNullOrEmpty(remoteAddr))
            {
                return remoteAddr;
            }

            return request.UserHostAddress;
        }
    }
}
