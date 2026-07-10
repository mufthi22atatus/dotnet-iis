using System.Web;
using System.Web.Mvc;
using TaskManager.Data.Entities;

namespace TaskManager.Helpers
{
    public static class HtmlHelpers
    {
        public static IHtmlString StatusBadge(this HtmlHelper html, string status)
        {
            var cls = status switch
            {
                "Open" => "bg-secondary",
                "InProgress" => "bg-primary",
                "Blocked" => "bg-warning text-dark",
                "InReview" => "bg-info text-dark",
                "Done" => "bg-success",
                "Cancelled" => "bg-dark",
                _ => "bg-light text-dark"
            };
            return new HtmlString($"<span class=\"badge {cls}\">{HttpUtility.HtmlEncode(status)}</span>");
        }

        public static IHtmlString PriorityBadge(this HtmlHelper html, string priority)
        {
            var cls = priority switch
            {
                "Low" => "bg-secondary",
                "Medium" => "bg-info text-dark",
                "High" => "bg-warning text-dark",
                "Critical" => "bg-danger",
                _ => "bg-light text-dark"
            };
            return new HtmlString($"<span class=\"badge {cls}\">{HttpUtility.HtmlEncode(priority)}</span>");
        }

        public static IHtmlString StatusBadge(this HtmlHelper html, TaskStatus status)
            => StatusBadge(html, status.ToString());

        public static IHtmlString PriorityBadge(this HtmlHelper html, TaskPriority p)
            => PriorityBadge(html, p.ToString());
    }
}
