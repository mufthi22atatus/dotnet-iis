using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace TaskManager.Data
{
    /// <summary>
    /// Production-style data access service using Microsoft.Data.SqlClient.
    /// Each method opens an independent SqlConnection and executes a query via
    /// SqlCommand + SqlDataReader + ExecuteReader(). This ensures each call
    /// produces a separate MSSQL span in APM/tracing tools.
    /// 
    /// Connection string is read from Web.config ("SqlClientConnection").
    /// </summary>
    public class SqlQueryService
    {
        private readonly string _connectionString;

        public SqlQueryService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["SqlClientConnection"]?.ConnectionString
                ?? throw new ConfigurationErrorsException(
                    "Missing connection string 'SqlClientConnection' in Web.config.");
        }

        public SqlQueryService(string connectionString)
        {
            _connectionString = connectionString
                ?? throw new ArgumentNullException(nameof(connectionString));
        }

        // ════════════════════════════════════════════════════════════════════
        //  EXISTING QUERIES (7 methods — unchanged)
        // ════════════════════════════════════════════════════════════════════

        #region Existing Queries

        // ====================================================================
        // QUERY 1: Employee Summary
        // ====================================================================
        public DataTable GetEmployeeSummary()
        {
            const string sql = @"
                SELECT 
                    e.Id,
                    e.FullName,
                    e.Email,
                    e.Role,
                    e.Department,
                    CASE WHEN e.IsActive = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
                    e.CreatedAt,
                    e.LastLoginAt,
                    e.FailedLoginCount
                FROM dbo.Employees e
                ORDER BY e.FullName";

            return ExecuteQuery(sql, "EmployeeSummary", "GetEmployeeSummary");
        }

        // ====================================================================
        // QUERY 2: Task Overview with Creator/Assignee Names
        // ====================================================================
        public DataTable GetTaskOverview()
        {
            const string sql = @"
                SELECT 
                    t.Id,
                    t.Title,
                    CASE t.Status
                        WHEN 0 THEN 'Open'
                        WHEN 1 THEN 'In Progress'
                        WHEN 2 THEN 'Blocked'
                        WHEN 3 THEN 'In Review'
                        WHEN 4 THEN 'Done'
                        WHEN 5 THEN 'Cancelled'
                        ELSE 'Unknown'
                    END AS StatusText,
                    CASE t.Priority
                        WHEN 0 THEN 'Low'
                        WHEN 1 THEN 'Medium'
                        WHEN 2 THEN 'High'
                        WHEN 3 THEN 'Critical'
                        ELSE 'Unknown'
                    END AS PriorityText,
                    creator.FullName  AS CreatedBy,
                    assignee.FullName AS AssignedTo,
                    p.Name            AS ProjectName,
                    t.CreatedAt,
                    t.DueDate,
                    t.Tag,
                    t.EstimatedHours,
                    t.LoggedHours
                FROM dbo.Tasks t
                INNER JOIN dbo.Employees creator ON t.CreatedById = creator.Id
                LEFT  JOIN dbo.Employees assignee ON t.AssignedToId = assignee.Id
                LEFT  JOIN dbo.Projects p ON t.ProjectId = p.Id
                ORDER BY t.CreatedAt DESC";

            return ExecuteQuery(sql, "TaskOverview", "GetTaskOverview");
        }

        // ====================================================================
        // QUERY 3: Task Status Counts (Aggregation)
        // ====================================================================
        public DataTable GetTaskStatusCounts()
        {
            const string sql = @"
                SELECT 
                    CASE Status
                        WHEN 0 THEN 'Open'
                        WHEN 1 THEN 'In Progress'
                        WHEN 2 THEN 'Blocked'
                        WHEN 3 THEN 'In Review'
                        WHEN 4 THEN 'Done'
                        WHEN 5 THEN 'Cancelled'
                        ELSE 'Unknown'
                    END AS StatusName,
                    COUNT(*) AS TaskCount
                FROM dbo.Tasks
                GROUP BY Status
                ORDER BY Status";

            return ExecuteQuery(sql, "TaskStatusCounts", "GetTaskStatusCounts");
        }

        // ====================================================================
        // QUERY 4: Recent Audit Logs
        // ====================================================================
        public DataTable GetRecentAuditLogs()
        {
            const string sql = @"
                SELECT TOP 20
                    al.Id,
                    al.EventType,
                    al.EntityName,
                    al.EntityId,
                    al.ActorEmail,
                    al.IpAddress,
                    al.Message,
                    al.CreatedAt
                FROM dbo.AuditLogs al
                ORDER BY al.CreatedAt DESC";

            return ExecuteQuery(sql, "RecentAuditLogs", "GetRecentAuditLogs");
        }

        // ====================================================================
        // QUERY 5: Overdue Tasks Report
        // ====================================================================
        public DataTable GetOverdueTasksReport()
        {
            const string sql = @"
                SELECT 
                    t.Id,
                    t.Title,
                    CASE t.Priority
                        WHEN 0 THEN 'Low'
                        WHEN 1 THEN 'Medium'
                        WHEN 2 THEN 'High'
                        WHEN 3 THEN 'Critical'
                        ELSE 'Unknown'
                    END AS PriorityText,
                    assignee.FullName AS AssignedTo,
                    t.DueDate,
                    DATEDIFF(day, t.DueDate, SYSUTCDATETIME()) AS DaysOverdue,
                    t.EstimatedHours,
                    t.LoggedHours
                FROM dbo.Tasks t
                LEFT JOIN dbo.Employees assignee ON t.AssignedToId = assignee.Id
                WHERE t.DueDate < SYSUTCDATETIME()
                  AND t.Status NOT IN (4, 5)
                ORDER BY t.DueDate ASC";

            return ExecuteQuery(sql, "OverdueTasksReport", "GetOverdueTasksReport");
        }

        // ====================================================================
        // QUERY 6: Department Workload (Aggregation)
        // ====================================================================
        public DataTable GetDepartmentWorkload()
        {
            const string sql = @"
                SELECT 
                    ISNULL(e.Department, 'Unassigned') AS Department,
                    COUNT(t.Id) AS ActiveTasks,
                    SUM(t.EstimatedHours) AS TotalEstimatedHours,
                    SUM(t.LoggedHours) AS TotalLoggedHours,
                    AVG(t.EstimatedHours) AS AvgEstimatedHours
                FROM dbo.Tasks t
                INNER JOIN dbo.Employees e ON t.AssignedToId = e.Id
                WHERE t.Status NOT IN (4, 5)
                GROUP BY e.Department
                ORDER BY ActiveTasks DESC";

            return ExecuteQuery(sql, "DepartmentWorkload", "GetDepartmentWorkload");
        }

        // ====================================================================
        // QUERY 7: Task Comment Activity (Join Query)
        // ====================================================================
        public DataTable GetTaskCommentActivity()
        {
            const string sql = @"
                SELECT TOP 15
                    tc.Id AS CommentId,
                    t.Title AS TaskTitle,
                    e.FullName AS Author,
                    tc.Body AS Comment,
                    tc.CreatedAt
                FROM dbo.TaskComments tc
                INNER JOIN dbo.Tasks t ON tc.TaskItemId = t.Id
                INNER JOIN dbo.Employees e ON tc.AuthorId = e.Id
                ORDER BY tc.CreatedAt DESC";

            return ExecuteQuery(sql, "TaskCommentActivity", "GetTaskCommentActivity");
        }

        #endregion

        // ════════════════════════════════════════════════════════════════════
        //  DASHBOARD QUERIES (8 methods — each produces 1 MSSQL span)
        // ════════════════════════════════════════════════════════════════════

        #region Dashboard Queries

        public int GetOpenTasksCount()
        {
            const string sql = "SELECT COUNT(*) FROM dbo.Tasks WHERE Status IN (0, 1, 2, 3)";
            return ExecuteScalar(sql, "GetOpenTasksCount");
        }

        public int GetClosedTasksCount()
        {
            const string sql = "SELECT COUNT(*) FROM dbo.Tasks WHERE Status IN (4, 5)";
            return ExecuteScalar(sql, "GetClosedTasksCount");
        }

        public int GetOverdueTasksCount()
        {
            const string sql = @"SELECT COUNT(*) FROM dbo.Tasks 
                WHERE DueDate < SYSUTCDATETIME() AND Status NOT IN (4, 5)";
            return ExecuteScalar(sql, "GetOverdueTasksCount");
        }

        public int GetHighPriorityTasksCount()
        {
            const string sql = "SELECT COUNT(*) FROM dbo.Tasks WHERE Priority IN (2, 3) AND Status NOT IN (4, 5)";
            return ExecuteScalar(sql, "GetHighPriorityTasksCount");
        }

        public DataTable GetTasksByStatusBreakdown()
        {
            const string sql = @"
                SELECT 
                    CASE Status
                        WHEN 0 THEN 'Open' WHEN 1 THEN 'InProgress' WHEN 2 THEN 'Blocked'
                        WHEN 3 THEN 'InReview' WHEN 4 THEN 'Done' WHEN 5 THEN 'Cancelled'
                        ELSE 'Unknown'
                    END AS StatusName,
                    COUNT(*) AS Count
                FROM dbo.Tasks
                GROUP BY Status
                ORDER BY Status";

            return ExecuteQuery(sql, "TasksByStatus", "GetTasksByStatusBreakdown");
        }

        public DataTable GetTasksByUser()
        {
            const string sql = @"
                SELECT 
                    e.Id AS EmployeeId,
                    e.FullName,
                    e.Department,
                    COUNT(t.Id) AS TotalTasks,
                    SUM(CASE WHEN t.Status IN (0,1,2,3) THEN 1 ELSE 0 END) AS ActiveTasks,
                    SUM(CASE WHEN t.Status = 4 THEN 1 ELSE 0 END) AS CompletedTasks,
                    SUM(CASE WHEN t.DueDate < SYSUTCDATETIME() AND t.Status NOT IN (4,5) THEN 1 ELSE 0 END) AS OverdueTasks
                FROM dbo.Employees e
                LEFT JOIN dbo.Tasks t ON t.AssignedToId = e.Id
                WHERE e.IsActive = 1
                GROUP BY e.Id, e.FullName, e.Department
                ORDER BY TotalTasks DESC";

            return ExecuteQuery(sql, "TasksByUser", "GetTasksByUser");
        }

        public DataTable GetRecentActivities()
        {
            const string sql = @"
                SELECT TOP 20
                    th.Id,
                    th.TaskItemId,
                    t.Title AS TaskTitle,
                    th.FieldName,
                    th.OldValue,
                    th.NewValue,
                    e.FullName AS ChangedBy,
                    th.ChangedAt
                FROM dbo.TaskHistory th
                INNER JOIN dbo.Tasks t ON th.TaskItemId = t.Id
                INNER JOIN dbo.Employees e ON th.ChangedById = e.Id
                ORDER BY th.ChangedAt DESC";

            return ExecuteQuery(sql, "RecentActivities", "GetRecentActivities");
        }

        public DataTable GetRecentCommentsDetailed()
        {
            const string sql = @"
                SELECT TOP 15
                    tc.Id,
                    tc.TaskItemId,
                    t.Title AS TaskTitle,
                    e.FullName AS Author,
                    e.Department,
                    tc.Body,
                    tc.CreatedAt
                FROM dbo.TaskComments tc
                INNER JOIN dbo.Tasks t ON tc.TaskItemId = t.Id
                INNER JOIN dbo.Employees e ON tc.AuthorId = e.Id
                ORDER BY tc.CreatedAt DESC";

            return ExecuteQuery(sql, "RecentCommentsDetailed", "GetRecentCommentsDetailed");
        }

        #endregion

        // ════════════════════════════════════════════════════════════════════
        //  REPORTING QUERIES (6 methods — each produces 1 MSSQL span)
        // ════════════════════════════════════════════════════════════════════

        #region Reporting Queries

        public DataTable GetDailyReport(DateTime date)
        {
            const string sql = @"
                SELECT 
                    'Created' AS Metric, COUNT(*) AS Count
                FROM dbo.Tasks WHERE CAST(CreatedAt AS DATE) = @date
                UNION ALL
                SELECT 
                    'Closed' AS Metric, COUNT(*) AS Count
                FROM dbo.Tasks WHERE CAST(CompletedAt AS DATE) = @date
                UNION ALL
                SELECT
                    'Comments' AS Metric, COUNT(*) AS Count
                FROM dbo.TaskComments WHERE CAST(CreatedAt AS DATE) = @date
                UNION ALL
                SELECT
                    'TimeLogged' AS Metric, ISNULL(SUM(DurationMinutes), 0) AS Count
                FROM dbo.TimeLogs WHERE CAST(StartedAt AS DATE) = @date";

            return ExecuteQuery(sql, "DailyReport", "GetDailyReport",
                new SqlParameter("@date", SqlDbType.Date) { Value = date.Date });
        }

        public DataTable GetWeeklyReport(DateTime weekStart)
        {
            var weekEnd = weekStart.AddDays(7);
            const string sql = @"
                SELECT 
                    CAST(t.CreatedAt AS DATE) AS Day,
                    COUNT(*) AS TasksCreated,
                    SUM(CASE WHEN t.Status = 4 THEN 1 ELSE 0 END) AS TasksClosed,
                    (SELECT COUNT(*) FROM dbo.TaskComments tc 
                     WHERE CAST(tc.CreatedAt AS DATE) = CAST(t.CreatedAt AS DATE)) AS Comments
                FROM dbo.Tasks t
                WHERE t.CreatedAt >= @weekStart AND t.CreatedAt < @weekEnd
                GROUP BY CAST(t.CreatedAt AS DATE)
                ORDER BY Day";

            return ExecuteQuery(sql, "WeeklyReport", "GetWeeklyReport",
                new SqlParameter("@weekStart", SqlDbType.DateTime2) { Value = weekStart },
                new SqlParameter("@weekEnd", SqlDbType.DateTime2) { Value = weekEnd });
        }

        public DataTable GetMonthlyReport(int year, int month)
        {
            const string sql = @"
                SELECT 
                    (SELECT COUNT(*) FROM dbo.Tasks WHERE YEAR(CreatedAt) = @year AND MONTH(CreatedAt) = @month) AS TasksCreated,
                    (SELECT COUNT(*) FROM dbo.Tasks WHERE YEAR(CompletedAt) = @year AND MONTH(CompletedAt) = @month) AS TasksClosed,
                    (SELECT COUNT(*) FROM dbo.TaskComments WHERE YEAR(CreatedAt) = @year AND MONTH(CreatedAt) = @month) AS TotalComments,
                    (SELECT ISNULL(SUM(DurationMinutes),0) FROM dbo.TimeLogs WHERE YEAR(StartedAt) = @year AND MONTH(StartedAt) = @month) AS TotalMinutesLogged,
                    (SELECT COUNT(*) FROM dbo.AuditLogs WHERE YEAR(CreatedAt) = @year AND MONTH(CreatedAt) = @month) AS AuditEvents";

            return ExecuteQuery(sql, "MonthlyReport", "GetMonthlyReport",
                new SqlParameter("@year", SqlDbType.Int) { Value = year },
                new SqlParameter("@month", SqlDbType.Int) { Value = month });
        }

        public DataTable GetUserProductivity(int? userId = null)
        {
            var sql = @"
                SELECT 
                    e.Id AS EmployeeId,
                    e.FullName,
                    e.Department,
                    COUNT(DISTINCT t.Id) AS TotalTasksAssigned,
                    SUM(CASE WHEN t.Status = 4 THEN 1 ELSE 0 END) AS TasksCompleted,
                    SUM(t.LoggedHours) AS TotalHoursLogged,
                    ISNULL(AVG(CASE WHEN t.Status = 4 AND t.CompletedAt IS NOT NULL 
                        THEN DATEDIFF(hour, t.CreatedAt, t.CompletedAt) END), 0) AS AvgCompletionHours,
                    (SELECT COUNT(*) FROM dbo.TaskComments tc WHERE tc.AuthorId = e.Id) AS CommentsWritten,
                    (SELECT ISNULL(SUM(tl.DurationMinutes),0) FROM dbo.TimeLogs tl WHERE tl.EmployeeId = e.Id) AS TimeLoggedMinutes
                FROM dbo.Employees e
                LEFT JOIN dbo.Tasks t ON t.AssignedToId = e.Id
                WHERE e.IsActive = 1" +
                (userId.HasValue ? " AND e.Id = @userId" : "") + @"
                GROUP BY e.Id, e.FullName, e.Department
                ORDER BY TasksCompleted DESC";

            var parameters = userId.HasValue
                ? new[] { new SqlParameter("@userId", SqlDbType.Int) { Value = userId.Value } }
                : new SqlParameter[0];

            return ExecuteQuery(sql, "UserProductivity", "GetUserProductivity", parameters);
        }

        public DataTable GetProjectSummary(int? projectId = null)
        {
            var sql = @"
                SELECT 
                    p.Id AS ProjectId,
                    p.Name AS ProjectName,
                    owner.FullName AS Owner,
                    COUNT(t.Id) AS TotalTasks,
                    SUM(CASE WHEN t.Status IN (0,1,2,3) THEN 1 ELSE 0 END) AS OpenTasks,
                    SUM(CASE WHEN t.Status = 4 THEN 1 ELSE 0 END) AS CompletedTasks,
                    SUM(CASE WHEN t.Status = 5 THEN 1 ELSE 0 END) AS CancelledTasks,
                    SUM(t.EstimatedHours) AS TotalEstimatedHours,
                    SUM(t.LoggedHours) AS TotalLoggedHours,
                    SUM(CASE WHEN t.DueDate < SYSUTCDATETIME() AND t.Status NOT IN (4,5) THEN 1 ELSE 0 END) AS OverdueTasks
                FROM dbo.Projects p
                INNER JOIN dbo.Employees owner ON p.OwnerId = owner.Id
                LEFT JOIN dbo.Tasks t ON t.ProjectId = p.Id
                WHERE p.IsActive = 1" +
                (projectId.HasValue ? " AND p.Id = @projectId" : "") + @"
                GROUP BY p.Id, p.Name, owner.FullName
                ORDER BY TotalTasks DESC";

            var parameters = projectId.HasValue
                ? new[] { new SqlParameter("@projectId", SqlDbType.Int) { Value = projectId.Value } }
                : new SqlParameter[0];

            return ExecuteQuery(sql, "ProjectSummary", "GetProjectSummary", parameters);
        }

        public DataTable GetTimeTrackingSummary(int? userId = null, int? taskId = null)
        {
            var sql = @"
                SELECT 
                    tl.TaskItemId,
                    t.Title AS TaskTitle,
                    e.FullName AS Employee,
                    COUNT(tl.Id) AS Sessions,
                    SUM(tl.DurationMinutes) AS TotalMinutes,
                    MIN(tl.StartedAt) AS FirstSession,
                    MAX(ISNULL(tl.StoppedAt, tl.StartedAt)) AS LastSession
                FROM dbo.TimeLogs tl
                INNER JOIN dbo.Tasks t ON tl.TaskItemId = t.Id
                INNER JOIN dbo.Employees e ON tl.EmployeeId = e.Id
                WHERE 1=1" +
                (userId.HasValue ? " AND tl.EmployeeId = @userId" : "") +
                (taskId.HasValue ? " AND tl.TaskItemId = @taskId" : "") + @"
                GROUP BY tl.TaskItemId, t.Title, e.FullName
                ORDER BY TotalMinutes DESC";

            var parms = new List<SqlParameter>();
            if (userId.HasValue) parms.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = userId.Value });
            if (taskId.HasValue) parms.Add(new SqlParameter("@taskId", SqlDbType.Int) { Value = taskId.Value });

            return ExecuteQuery(sql, "TimeTrackingSummary", "GetTimeTrackingSummary", parms.ToArray());
        }

        #endregion

        // ════════════════════════════════════════════════════════════════════
        //  SEARCH & FILTER QUERIES (6 methods — each produces 1 MSSQL span)
        // ════════════════════════════════════════════════════════════════════

        #region Search & Filter Queries

        public DataTable SearchTasks(string keyword, int page = 1, int pageSize = 20)
        {
            const string sql = @"
                SELECT 
                    t.Id, t.Title, t.Description,
                    CASE t.Status WHEN 0 THEN 'Open' WHEN 1 THEN 'InProgress' WHEN 2 THEN 'Blocked' 
                        WHEN 3 THEN 'InReview' WHEN 4 THEN 'Done' WHEN 5 THEN 'Cancelled' ELSE 'Unknown' END AS StatusText,
                    CASE t.Priority WHEN 0 THEN 'Low' WHEN 1 THEN 'Medium' WHEN 2 THEN 'High' WHEN 3 THEN 'Critical' ELSE 'Unknown' END AS PriorityText,
                    assignee.FullName AS AssignedTo,
                    p.Name AS ProjectName,
                    t.DueDate, t.CreatedAt
                FROM dbo.Tasks t
                LEFT JOIN dbo.Employees assignee ON t.AssignedToId = assignee.Id
                LEFT JOIN dbo.Projects p ON t.ProjectId = p.Id
                WHERE (t.Title LIKE @keyword OR t.Description LIKE @keyword OR t.Tag LIKE @keyword)
                ORDER BY t.CreatedAt DESC
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

            return ExecuteQuery(sql, "SearchTasks", "SearchTasks",
                new SqlParameter("@keyword", SqlDbType.NVarChar, 200) { Value = "%" + (keyword ?? "") + "%" },
                new SqlParameter("@offset", SqlDbType.Int) { Value = (page - 1) * pageSize },
                new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize });
        }

        public DataTable FilterByStatus(int status, int page = 1, int pageSize = 20)
        {
            const string sql = @"
                SELECT 
                    t.Id, t.Title,
                    CASE t.Priority WHEN 0 THEN 'Low' WHEN 1 THEN 'Medium' WHEN 2 THEN 'High' WHEN 3 THEN 'Critical' ELSE 'Unknown' END AS PriorityText,
                    assignee.FullName AS AssignedTo,
                    p.Name AS ProjectName,
                    t.DueDate, t.CreatedAt, t.UpdatedAt
                FROM dbo.Tasks t
                LEFT JOIN dbo.Employees assignee ON t.AssignedToId = assignee.Id
                LEFT JOIN dbo.Projects p ON t.ProjectId = p.Id
                WHERE t.Status = @status
                ORDER BY t.UpdatedAt DESC
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

            return ExecuteQuery(sql, "FilterByStatus", "FilterByStatus",
                new SqlParameter("@status", SqlDbType.Int) { Value = status },
                new SqlParameter("@offset", SqlDbType.Int) { Value = (page - 1) * pageSize },
                new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize });
        }

        public DataTable FilterByPriority(int priority, int page = 1, int pageSize = 20)
        {
            const string sql = @"
                SELECT 
                    t.Id, t.Title,
                    CASE t.Status WHEN 0 THEN 'Open' WHEN 1 THEN 'InProgress' WHEN 2 THEN 'Blocked' 
                        WHEN 3 THEN 'InReview' WHEN 4 THEN 'Done' WHEN 5 THEN 'Cancelled' ELSE 'Unknown' END AS StatusText,
                    assignee.FullName AS AssignedTo,
                    p.Name AS ProjectName,
                    t.DueDate, t.CreatedAt, t.UpdatedAt
                FROM dbo.Tasks t
                LEFT JOIN dbo.Employees assignee ON t.AssignedToId = assignee.Id
                LEFT JOIN dbo.Projects p ON t.ProjectId = p.Id
                WHERE t.Priority = @priority
                ORDER BY t.UpdatedAt DESC
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

            return ExecuteQuery(sql, "FilterByPriority", "FilterByPriority",
                new SqlParameter("@priority", SqlDbType.Int) { Value = priority },
                new SqlParameter("@offset", SqlDbType.Int) { Value = (page - 1) * pageSize },
                new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize });
        }

        public DataTable FilterByAssignee(int assigneeId, int page = 1, int pageSize = 20)
        {
            const string sql = @"
                SELECT 
                    t.Id, t.Title,
                    CASE t.Status WHEN 0 THEN 'Open' WHEN 1 THEN 'InProgress' WHEN 2 THEN 'Blocked' 
                        WHEN 3 THEN 'InReview' WHEN 4 THEN 'Done' WHEN 5 THEN 'Cancelled' ELSE 'Unknown' END AS StatusText,
                    CASE t.Priority WHEN 0 THEN 'Low' WHEN 1 THEN 'Medium' WHEN 2 THEN 'High' WHEN 3 THEN 'Critical' ELSE 'Unknown' END AS PriorityText,
                    p.Name AS ProjectName,
                    t.DueDate, t.CreatedAt, t.EstimatedHours, t.LoggedHours
                FROM dbo.Tasks t
                LEFT JOIN dbo.Projects p ON t.ProjectId = p.Id
                WHERE t.AssignedToId = @assigneeId
                ORDER BY t.Priority DESC, t.DueDate ASC
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

            return ExecuteQuery(sql, "FilterByAssignee", "FilterByAssignee",
                new SqlParameter("@assigneeId", SqlDbType.Int) { Value = assigneeId },
                new SqlParameter("@offset", SqlDbType.Int) { Value = (page - 1) * pageSize },
                new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize });
        }

        public DataTable FilterByProject(int projectId, int page = 1, int pageSize = 20)
        {
            const string sql = @"
                SELECT 
                    t.Id, t.Title,
                    CASE t.Status WHEN 0 THEN 'Open' WHEN 1 THEN 'InProgress' WHEN 2 THEN 'Blocked' 
                        WHEN 3 THEN 'InReview' WHEN 4 THEN 'Done' WHEN 5 THEN 'Cancelled' ELSE 'Unknown' END AS StatusText,
                    CASE t.Priority WHEN 0 THEN 'Low' WHEN 1 THEN 'Medium' WHEN 2 THEN 'High' WHEN 3 THEN 'Critical' ELSE 'Unknown' END AS PriorityText,
                    assignee.FullName AS AssignedTo,
                    t.DueDate, t.CreatedAt, t.EstimatedHours, t.LoggedHours
                FROM dbo.Tasks t
                LEFT JOIN dbo.Employees assignee ON t.AssignedToId = assignee.Id
                WHERE t.ProjectId = @projectId
                ORDER BY t.Priority DESC, t.DueDate ASC
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

            return ExecuteQuery(sql, "FilterByProject", "FilterByProject",
                new SqlParameter("@projectId", SqlDbType.Int) { Value = projectId },
                new SqlParameter("@offset", SqlDbType.Int) { Value = (page - 1) * pageSize },
                new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize });
        }

        public DataTable FilterByDateRange(DateTime from, DateTime to, int page = 1, int pageSize = 20)
        {
            const string sql = @"
                SELECT 
                    t.Id, t.Title,
                    CASE t.Status WHEN 0 THEN 'Open' WHEN 1 THEN 'InProgress' WHEN 2 THEN 'Blocked' 
                        WHEN 3 THEN 'InReview' WHEN 4 THEN 'Done' WHEN 5 THEN 'Cancelled' ELSE 'Unknown' END AS StatusText,
                    CASE t.Priority WHEN 0 THEN 'Low' WHEN 1 THEN 'Medium' WHEN 2 THEN 'High' WHEN 3 THEN 'Critical' ELSE 'Unknown' END AS PriorityText,
                    assignee.FullName AS AssignedTo,
                    p.Name AS ProjectName,
                    t.DueDate, t.CreatedAt
                FROM dbo.Tasks t
                LEFT JOIN dbo.Employees assignee ON t.AssignedToId = assignee.Id
                LEFT JOIN dbo.Projects p ON t.ProjectId = p.Id
                WHERE t.CreatedAt >= @from AND t.CreatedAt < @to
                ORDER BY t.CreatedAt DESC
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

            return ExecuteQuery(sql, "FilterByDateRange", "FilterByDateRange",
                new SqlParameter("@from", SqlDbType.DateTime2) { Value = from },
                new SqlParameter("@to", SqlDbType.DateTime2) { Value = to },
                new SqlParameter("@offset", SqlDbType.Int) { Value = (page - 1) * pageSize },
                new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize });
        }

        #endregion

        // ════════════════════════════════════════════════════════════════════
        //  WORKFLOW INSERT METHODS (each produces 1 MSSQL span)
        // ════════════════════════════════════════════════════════════════════

        #region Workflow Inserts

        public int InsertTaskHistory(int taskId, string fieldName, string oldValue, string newValue, int actorId)
        {
            const string sql = @"
                INSERT INTO dbo.TaskHistory (TaskItemId, FieldName, OldValue, NewValue, ChangedById, ChangedAt)
                VALUES (@taskId, @fieldName, @oldValue, @newValue, @actorId, SYSUTCDATETIME());
                SELECT SCOPE_IDENTITY();";

            return ExecuteInsert(sql, "InsertTaskHistory",
                new SqlParameter("@taskId", SqlDbType.Int) { Value = taskId },
                new SqlParameter("@fieldName", SqlDbType.NVarChar, 64) { Value = fieldName },
                new SqlParameter("@oldValue", SqlDbType.NVarChar, 500) { Value = (object)oldValue ?? DBNull.Value },
                new SqlParameter("@newValue", SqlDbType.NVarChar, 500) { Value = (object)newValue ?? DBNull.Value },
                new SqlParameter("@actorId", SqlDbType.Int) { Value = actorId });
        }

        public int InsertTaskStatusHistory(int taskId, int oldStatus, int newStatus, int actorId)
        {
            const string sql = @"
                INSERT INTO dbo.TaskStatusHistory (TaskItemId, OldStatus, NewStatus, ChangedById, ChangedAt)
                VALUES (@taskId, @oldStatus, @newStatus, @actorId, SYSUTCDATETIME());
                SELECT SCOPE_IDENTITY();";

            return ExecuteInsert(sql, "InsertTaskStatusHistory",
                new SqlParameter("@taskId", SqlDbType.Int) { Value = taskId },
                new SqlParameter("@oldStatus", SqlDbType.Int) { Value = oldStatus },
                new SqlParameter("@newStatus", SqlDbType.Int) { Value = newStatus },
                new SqlParameter("@actorId", SqlDbType.Int) { Value = actorId });
        }

        public int InsertTaskAssignment(int taskId, int assigneeId, int assignedById)
        {
            const string sql = @"
                INSERT INTO dbo.TaskAssignments (TaskItemId, AssignedToId, AssignedById, AssignedAt, IsActive)
                VALUES (@taskId, @assigneeId, @assignedById, SYSUTCDATETIME(), 1);
                SELECT SCOPE_IDENTITY();";

            return ExecuteInsert(sql, "InsertTaskAssignment",
                new SqlParameter("@taskId", SqlDbType.Int) { Value = taskId },
                new SqlParameter("@assigneeId", SqlDbType.Int) { Value = assigneeId },
                new SqlParameter("@assignedById", SqlDbType.Int) { Value = assignedById });
        }

        public void DeactivatePreviousAssignments(int taskId)
        {
            const string sql = @"
                UPDATE dbo.TaskAssignments 
                SET IsActive = 0, UnassignedAt = SYSUTCDATETIME()
                WHERE TaskItemId = @taskId AND IsActive = 1";

            ExecuteNonQuery(sql, "DeactivatePreviousAssignments",
                new SqlParameter("@taskId", SqlDbType.Int) { Value = taskId });
        }

        public int InsertTaskLabel(int taskId, string label, int actorId)
        {
            const string sql = @"
                IF NOT EXISTS (SELECT 1 FROM dbo.TaskLabels WHERE TaskItemId = @taskId AND Label = @label)
                BEGIN
                    INSERT INTO dbo.TaskLabels (TaskItemId, Label, AddedById, AddedAt)
                    VALUES (@taskId, @label, @actorId, SYSUTCDATETIME());
                    SELECT SCOPE_IDENTITY();
                END
                ELSE SELECT -1;";

            return ExecuteInsert(sql, "InsertTaskLabel",
                new SqlParameter("@taskId", SqlDbType.Int) { Value = taskId },
                new SqlParameter("@label", SqlDbType.NVarChar, 80) { Value = label },
                new SqlParameter("@actorId", SqlDbType.Int) { Value = actorId });
        }

        public void DeleteTaskLabel(int taskId, string label)
        {
            const string sql = "DELETE FROM dbo.TaskLabels WHERE TaskItemId = @taskId AND Label = @label";
            ExecuteNonQuery(sql, "DeleteTaskLabel",
                new SqlParameter("@taskId", SqlDbType.Int) { Value = taskId },
                new SqlParameter("@label", SqlDbType.NVarChar, 80) { Value = label });
        }

        public int InsertTaskWatcher(int taskId, int employeeId)
        {
            const string sql = @"
                IF NOT EXISTS (SELECT 1 FROM dbo.TaskWatchers WHERE TaskItemId = @taskId AND EmployeeId = @employeeId)
                BEGIN
                    INSERT INTO dbo.TaskWatchers (TaskItemId, EmployeeId, WatchingSince)
                    VALUES (@taskId, @employeeId, SYSUTCDATETIME());
                    SELECT SCOPE_IDENTITY();
                END
                ELSE SELECT -1;";

            return ExecuteInsert(sql, "InsertTaskWatcher",
                new SqlParameter("@taskId", SqlDbType.Int) { Value = taskId },
                new SqlParameter("@employeeId", SqlDbType.Int) { Value = employeeId });
        }

        public void DeleteTaskWatcher(int taskId, int employeeId)
        {
            const string sql = "DELETE FROM dbo.TaskWatchers WHERE TaskItemId = @taskId AND EmployeeId = @employeeId";
            ExecuteNonQuery(sql, "DeleteTaskWatcher",
                new SqlParameter("@taskId", SqlDbType.Int) { Value = taskId },
                new SqlParameter("@employeeId", SqlDbType.Int) { Value = employeeId });
        }

        public int InsertTaskDependency(int taskId, int dependsOnTaskId, string dependencyType, int actorId)
        {
            const string sql = @"
                IF NOT EXISTS (SELECT 1 FROM dbo.TaskDependencies WHERE TaskItemId = @taskId AND DependsOnTaskId = @dependsOn)
                BEGIN
                    INSERT INTO dbo.TaskDependencies (TaskItemId, DependsOnTaskId, DependencyType, CreatedById, CreatedAt)
                    VALUES (@taskId, @dependsOn, @type, @actorId, SYSUTCDATETIME());
                    SELECT SCOPE_IDENTITY();
                END
                ELSE SELECT -1;";

            return ExecuteInsert(sql, "InsertTaskDependency",
                new SqlParameter("@taskId", SqlDbType.Int) { Value = taskId },
                new SqlParameter("@dependsOn", SqlDbType.Int) { Value = dependsOnTaskId },
                new SqlParameter("@type", SqlDbType.NVarChar, 32) { Value = dependencyType },
                new SqlParameter("@actorId", SqlDbType.Int) { Value = actorId });
        }

        public void DeleteTaskDependency(int taskId, int dependsOnTaskId)
        {
            const string sql = "DELETE FROM dbo.TaskDependencies WHERE TaskItemId = @taskId AND DependsOnTaskId = @dependsOn";
            ExecuteNonQuery(sql, "DeleteTaskDependency",
                new SqlParameter("@taskId", SqlDbType.Int) { Value = taskId },
                new SqlParameter("@dependsOn", SqlDbType.Int) { Value = dependsOnTaskId });
        }

        public long InsertNotification(int recipientId, string type, string title, string message,
            string relatedEntityType = null, string relatedEntityId = null)
        {
            const string sql = @"
                INSERT INTO dbo.Notifications (RecipientId, Type, Title, Message, RelatedEntityType, RelatedEntityId, IsRead, CreatedAt)
                VALUES (@recipientId, @type, @title, @message, @entityType, @entityId, 0, SYSUTCDATETIME());
                SELECT SCOPE_IDENTITY();";

            var result = ExecuteInsert(sql, "InsertNotification",
                new SqlParameter("@recipientId", SqlDbType.Int) { Value = recipientId },
                new SqlParameter("@type", SqlDbType.NVarChar, 64) { Value = type },
                new SqlParameter("@title", SqlDbType.NVarChar, 200) { Value = title },
                new SqlParameter("@message", SqlDbType.NVarChar, 1000) { Value = (object)message ?? DBNull.Value },
                new SqlParameter("@entityType", SqlDbType.NVarChar, 64) { Value = (object)relatedEntityType ?? DBNull.Value },
                new SqlParameter("@entityId", SqlDbType.NVarChar, 64) { Value = (object)relatedEntityId ?? DBNull.Value });
            return result;
        }

        public int InsertTimeLog(int taskId, int employeeId, DateTime startedAt, DateTime? stoppedAt,
            int durationMinutes, string description)
        {
            const string sql = @"
                INSERT INTO dbo.TimeLogs (TaskItemId, EmployeeId, StartedAt, StoppedAt, DurationMinutes, Description)
                VALUES (@taskId, @employeeId, @startedAt, @stoppedAt, @duration, @description);
                SELECT SCOPE_IDENTITY();";

            return ExecuteInsert(sql, "InsertTimeLog",
                new SqlParameter("@taskId", SqlDbType.Int) { Value = taskId },
                new SqlParameter("@employeeId", SqlDbType.Int) { Value = employeeId },
                new SqlParameter("@startedAt", SqlDbType.DateTime2) { Value = startedAt },
                new SqlParameter("@stoppedAt", SqlDbType.DateTime2) { Value = (object)stoppedAt ?? DBNull.Value },
                new SqlParameter("@duration", SqlDbType.Int) { Value = durationMinutes },
                new SqlParameter("@description", SqlDbType.NVarChar, 500) { Value = (object)description ?? DBNull.Value });
        }

        public void UpdateTimeLogStop(int timeLogId, DateTime stoppedAt, int durationMinutes)
        {
            const string sql = @"
                UPDATE dbo.TimeLogs SET StoppedAt = @stoppedAt, DurationMinutes = @duration
                WHERE Id = @id";

            ExecuteNonQuery(sql, "UpdateTimeLogStop",
                new SqlParameter("@id", SqlDbType.Int) { Value = timeLogId },
                new SqlParameter("@stoppedAt", SqlDbType.DateTime2) { Value = stoppedAt },
                new SqlParameter("@duration", SqlDbType.Int) { Value = durationMinutes });
        }

        public long InsertAuditLogDirect(string eventType, int? actorId, string message,
            string entityName = null, string entityId = null)
        {
            const string sql = @"
                INSERT INTO dbo.AuditLogs (EventType, ActorId, Message, EntityName, EntityId, CreatedAt)
                VALUES (@eventType, @actorId, @message, @entityName, @entityId, SYSUTCDATETIME());
                SELECT SCOPE_IDENTITY();";

            var result = ExecuteInsert(sql, "InsertAuditLogDirect",
                new SqlParameter("@eventType", SqlDbType.NVarChar, 64) { Value = eventType },
                new SqlParameter("@actorId", SqlDbType.Int) { Value = (object)actorId ?? DBNull.Value },
                new SqlParameter("@message", SqlDbType.NVarChar, 512) { Value = (object)message ?? DBNull.Value },
                new SqlParameter("@entityName", SqlDbType.NVarChar, 64) { Value = (object)entityName ?? DBNull.Value },
                new SqlParameter("@entityId", SqlDbType.NVarChar, 64) { Value = (object)entityId ?? DBNull.Value });
            return result;
        }

        public int InsertComment(int taskId, int authorId, string body)
        {
            const string sql = @"
                INSERT INTO dbo.TaskComments (TaskItemId, AuthorId, Body, CreatedAt)
                VALUES (@taskId, @authorId, @body, SYSUTCDATETIME());
                SELECT SCOPE_IDENTITY();";

            return ExecuteInsert(sql, "InsertComment",
                new SqlParameter("@taskId", SqlDbType.Int) { Value = taskId },
                new SqlParameter("@authorId", SqlDbType.Int) { Value = authorId },
                new SqlParameter("@body", SqlDbType.NVarChar, 2000) { Value = body });
        }

        public void UpdateComment(int commentId, string body)
        {
            const string sql = "UPDATE dbo.TaskComments SET Body = @body WHERE Id = @id";
            ExecuteNonQuery(sql, "UpdateComment",
                new SqlParameter("@id", SqlDbType.Int) { Value = commentId },
                new SqlParameter("@body", SqlDbType.NVarChar, 2000) { Value = body });
        }

        public void DeleteComment(int commentId)
        {
            const string sql = "DELETE FROM dbo.TaskComments WHERE Id = @id";
            ExecuteNonQuery(sql, "DeleteComment",
                new SqlParameter("@id", SqlDbType.Int) { Value = commentId });
        }

        public int InsertAttachment(int taskId, string fileName, string contentType, string storedPath,
            long sizeBytes, int uploadedById)
        {
            const string sql = @"
                INSERT INTO dbo.TaskAttachments (TaskItemId, FileName, ContentType, StoredPath, SizeBytes, UploadedById, UploadedAt)
                VALUES (@taskId, @fileName, @contentType, @storedPath, @sizeBytes, @uploadedById, SYSUTCDATETIME());
                SELECT SCOPE_IDENTITY();";

            return ExecuteInsert(sql, "InsertAttachment",
                new SqlParameter("@taskId", SqlDbType.Int) { Value = taskId },
                new SqlParameter("@fileName", SqlDbType.NVarChar, 260) { Value = fileName },
                new SqlParameter("@contentType", SqlDbType.NVarChar, 120) { Value = contentType },
                new SqlParameter("@storedPath", SqlDbType.NVarChar, 512) { Value = storedPath },
                new SqlParameter("@sizeBytes", SqlDbType.BigInt) { Value = sizeBytes },
                new SqlParameter("@uploadedById", SqlDbType.Int) { Value = uploadedById });
        }

        public void DeleteAttachment(int attachmentId)
        {
            const string sql = "DELETE FROM dbo.TaskAttachments WHERE Id = @id";
            ExecuteNonQuery(sql, "DeleteAttachment",
                new SqlParameter("@id", SqlDbType.Int) { Value = attachmentId });
        }

        #endregion

        // ════════════════════════════════════════════════════════════════════
        //  ADDITIONAL READ METHODS (each produces 1 MSSQL span)
        // ════════════════════════════════════════════════════════════════════

        #region Additional Reads

        public DataTable GetTaskLabels(int taskId)
        {
            const string sql = @"
                SELECT tl.Id, tl.Label, e.FullName AS AddedBy, tl.AddedAt
                FROM dbo.TaskLabels tl
                INNER JOIN dbo.Employees e ON tl.AddedById = e.Id
                WHERE tl.TaskItemId = @taskId
                ORDER BY tl.AddedAt";

            return ExecuteQuery(sql, "TaskLabels", "GetTaskLabels",
                new SqlParameter("@taskId", SqlDbType.Int) { Value = taskId });
        }

        public DataTable GetTaskWatchers(int taskId)
        {
            const string sql = @"
                SELECT tw.Id, e.Id AS EmployeeId, e.FullName, e.Email, e.Department, tw.WatchingSince
                FROM dbo.TaskWatchers tw
                INNER JOIN dbo.Employees e ON tw.EmployeeId = e.Id
                WHERE tw.TaskItemId = @taskId
                ORDER BY tw.WatchingSince";

            return ExecuteQuery(sql, "TaskWatchers", "GetTaskWatchers",
                new SqlParameter("@taskId", SqlDbType.Int) { Value = taskId });
        }

        public DataTable GetTaskDependencies(int taskId)
        {
            const string sql = @"
                SELECT td.Id, td.DependsOnTaskId, dep.Title AS DependsOnTitle,
                    CASE dep.Status WHEN 0 THEN 'Open' WHEN 1 THEN 'InProgress' WHEN 2 THEN 'Blocked' 
                        WHEN 3 THEN 'InReview' WHEN 4 THEN 'Done' WHEN 5 THEN 'Cancelled' ELSE 'Unknown' END AS DependsOnStatus,
                    td.DependencyType, e.FullName AS CreatedBy, td.CreatedAt
                FROM dbo.TaskDependencies td
                INNER JOIN dbo.Tasks dep ON td.DependsOnTaskId = dep.Id
                INNER JOIN dbo.Employees e ON td.CreatedById = e.Id
                WHERE td.TaskItemId = @taskId
                ORDER BY td.CreatedAt";

            return ExecuteQuery(sql, "TaskDependencies", "GetTaskDependencies",
                new SqlParameter("@taskId", SqlDbType.Int) { Value = taskId });
        }

        public DataTable GetTaskTimeLogs(int taskId)
        {
            const string sql = @"
                SELECT tl.Id, e.FullName AS Employee, tl.StartedAt, tl.StoppedAt,
                    tl.DurationMinutes, tl.Description
                FROM dbo.TimeLogs tl
                INNER JOIN dbo.Employees e ON tl.EmployeeId = e.Id
                WHERE tl.TaskItemId = @taskId
                ORDER BY tl.StartedAt DESC";

            return ExecuteQuery(sql, "TaskTimeLogs", "GetTaskTimeLogs",
                new SqlParameter("@taskId", SqlDbType.Int) { Value = taskId });
        }

        public DataTable GetTaskComments(int taskId)
        {
            const string sql = @"
                SELECT tc.Id, e.FullName AS Author, tc.Body, tc.CreatedAt
                FROM dbo.TaskComments tc
                INNER JOIN dbo.Employees e ON tc.AuthorId = e.Id
                WHERE tc.TaskItemId = @taskId
                ORDER BY tc.CreatedAt DESC";

            return ExecuteQuery(sql, "TaskComments", "GetTaskComments",
                new SqlParameter("@taskId", SqlDbType.Int) { Value = taskId });
        }

        public DataTable GetTaskAttachments(int taskId)
        {
            const string sql = @"
                SELECT ta.Id, ta.FileName, ta.ContentType, ta.SizeBytes,
                    e.FullName AS UploadedBy, ta.UploadedAt
                FROM dbo.TaskAttachments ta
                INNER JOIN dbo.Employees e ON ta.UploadedById = e.Id
                WHERE ta.TaskItemId = @taskId
                ORDER BY ta.UploadedAt DESC";

            return ExecuteQuery(sql, "TaskAttachments", "GetTaskAttachments",
                new SqlParameter("@taskId", SqlDbType.Int) { Value = taskId });
        }

        public DataTable GetTaskAssignmentHistory(int taskId)
        {
            const string sql = @"
                SELECT ta.Id, assignee.FullName AS AssignedTo, assigner.FullName AS AssignedBy,
                    ta.AssignedAt, ta.UnassignedAt, ta.IsActive
                FROM dbo.TaskAssignments ta
                INNER JOIN dbo.Employees assignee ON ta.AssignedToId = assignee.Id
                INNER JOIN dbo.Employees assigner ON ta.AssignedById = assigner.Id
                WHERE ta.TaskItemId = @taskId
                ORDER BY ta.AssignedAt DESC";

            return ExecuteQuery(sql, "TaskAssignmentHistory", "GetTaskAssignmentHistory",
                new SqlParameter("@taskId", SqlDbType.Int) { Value = taskId });
        }

        public DataTable GetTaskById(int taskId)
        {
            const string sql = @"
                SELECT t.Id, t.Title, t.Description,
                    t.Status, t.Priority,
                    creator.FullName AS CreatedBy,
                    assignee.FullName AS AssignedTo,
                    p.Name AS ProjectName,
                    t.CreatedAt, t.UpdatedAt, t.DueDate, t.CompletedAt,
                    t.Tag, t.EstimatedHours, t.LoggedHours
                FROM dbo.Tasks t
                INNER JOIN dbo.Employees creator ON t.CreatedById = creator.Id
                LEFT JOIN dbo.Employees assignee ON t.AssignedToId = assignee.Id
                LEFT JOIN dbo.Projects p ON t.ProjectId = p.Id
                WHERE t.Id = @taskId";

            return ExecuteQuery(sql, "TaskById", "GetTaskById",
                new SqlParameter("@taskId", SqlDbType.Int) { Value = taskId });
        }

        #endregion

        // ════════════════════════════════════════════════════════════════════
        //  PRIVATE HELPERS — shared SqlConnection plumbing
        // ════════════════════════════════════════════════════════════════════

        #region Private Helpers

        private DataTable ExecuteQuery(string sql, string tableName, string callerName,
            params SqlParameter[] parameters)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        if (parameters != null && parameters.Length > 0)
                            command.Parameters.AddRange(parameters);

                        using (var reader = command.ExecuteReader())
                        {
                            var table = new DataTable(tableName);
                            table.Load(reader);
                            return table;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                AppLogger.Create<SqlQueryService>()?.LogError(ex, "SqlQueryService.{Caller} failed: {Message}", callerName, ex.Message);
                throw;
            }
        }

        private int ExecuteScalar(string sql, string callerName, params SqlParameter[] parameters)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        if (parameters != null && parameters.Length > 0)
                            command.Parameters.AddRange(parameters);

                        var result = command.ExecuteScalar();
                        return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                AppLogger.Create<SqlQueryService>()?.LogError(ex, "SqlQueryService.{Caller} failed: {Message}", callerName, ex.Message);
                throw;
            }
        }

        private int ExecuteInsert(string sql, string callerName, params SqlParameter[] parameters)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        if (parameters != null && parameters.Length > 0)
                            command.Parameters.AddRange(parameters);

                        var result = command.ExecuteScalar();
                        return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                AppLogger.Create<SqlQueryService>()?.LogError(ex, "SqlQueryService.{Caller} failed: {Message}", callerName, ex.Message);
                throw;
            }
        }

        private void ExecuteNonQuery(string sql, string callerName, params SqlParameter[] parameters)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        if (parameters != null && parameters.Length > 0)
                            command.Parameters.AddRange(parameters);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                AppLogger.Create<SqlQueryService>()?.LogError(ex, "SqlQueryService.{Caller} failed: {Message}", callerName, ex.Message);
                throw;
            }
        }

        #endregion
    }
}
