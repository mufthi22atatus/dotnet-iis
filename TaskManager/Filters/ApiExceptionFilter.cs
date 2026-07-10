using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace TaskManager.Filters
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var ex = context.Exception;
            AppLogger.Create<ApiExceptionFilter>()?.LogError(ex, "Web API exception: {Path}", context.Request?.RequestUri?.PathAndQuery);

            var status = ex is System.UnauthorizedAccessException
                ? HttpStatusCode.Unauthorized
                : HttpStatusCode.InternalServerError;

            var payload = new
            {
                error = ex.GetType().Name,
                message = ex.Message,
                requestId = context.Request?.GetCorrelationId().ToString()
            };

            context.Response = new HttpResponseMessage(status)
            {
                Content = new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json")
            };
        }
    }
}
