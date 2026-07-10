using System;

namespace TaskManager.ViewModels
{
    public class DashboardCount
    {
        public string Bucket { get; set; }
        public int Count { get; set; }
    }

    public class RecentTaskRow
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string AssignedTo { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class DashboardViewModel
    {
        public DateTime GeneratedAtUtc { get; set; }
        public int Total { get; set; }
        public int Open { get; set; }
        public int InProgress { get; set; }
        public int Done { get; set; }
        public int Overdue { get; set; }
        public DashboardCount[] ByPriority { get; set; }
        public DashboardCount[] ByStatus { get; set; }
        public RecentTaskRow[] Recent { get; set; }
    }
}
