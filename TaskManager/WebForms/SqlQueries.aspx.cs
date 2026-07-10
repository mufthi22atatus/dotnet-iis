using System;
using System.Data;
using System.Diagnostics;
using System.Web.UI;
using Microsoft.Extensions.Logging;
using TaskManager.Data;

namespace TaskManager.WebForms
{
    /// <summary>
    /// Code-behind for SqlQueries.aspx — ASP.NET WebForms page.
    /// 
    /// Executes 7 independent SQL queries using Microsoft.Data.SqlClient via
    /// SqlQueryService. Each query opens its own SqlConnection and calls
    /// ExecuteReader(), generating a separate MSSQL span in APM tools.
    /// 
    /// Expected APM output per page request:
    ///   - 1 ASP.NET WebForms request span
    ///   - 7 MSSQL query spans (one per SqlQueryService method call)
    /// </summary>
    public partial class SqlQueries : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var stopwatch = Stopwatch.StartNew();
                int queryCount = 0;

                AppLogger.Create<SqlQueries>()?.LogInformation("SqlQueries.aspx Page_Load — executing 7 independent SQL queries");

                var service = new SqlQueryService();

                // ============================================================
                // QUERY 1: Employee Summary
                // ** MSSQL SPAN 1 ** — SqlConnection → SqlCommand → ExecuteReader()
                // ============================================================
                try
                {
                    DataTable employees = service.GetEmployeeSummary();
                    gvEmployees.DataSource = employees;
                    gvEmployees.DataBind();
                    lblTotalEmployees.Text = employees.Rows.Count.ToString();
                    queryCount++;
                    AppLogger.Create<SqlQueries>()?.LogInformation("Query 1 (Employee Summary): {Count} rows", employees.Rows.Count);
                }
                catch (Exception ex)
                {
                    lblEmployeeError.Text = $"Query 1 failed: {ex.Message}";
                    lblEmployeeError.Visible = true;
                    AppLogger.Create<SqlQueries>()?.LogError(ex, "Query 1 (Employee Summary) failed");
                }

                // ============================================================
                // QUERY 2: Task Overview
                // ** MSSQL SPAN 2 ** — SqlConnection → SqlCommand → ExecuteReader()
                // ============================================================
                try
                {
                    DataTable tasks = service.GetTaskOverview();
                    gvTasks.DataSource = tasks;
                    gvTasks.DataBind();
                    lblTotalTasks.Text = tasks.Rows.Count.ToString();
                    queryCount++;
                    AppLogger.Create<SqlQueries>()?.LogInformation("Query 2 (Task Overview): {Count} rows", tasks.Rows.Count);
                }
                catch (Exception ex)
                {
                    lblTaskError.Text = $"Query 2 failed: {ex.Message}";
                    lblTaskError.Visible = true;
                    AppLogger.Create<SqlQueries>()?.LogError(ex, "Query 2 (Task Overview) failed");
                }

                // ============================================================
                // QUERY 3: Task Status Counts
                // ** MSSQL SPAN 3 ** — SqlConnection → SqlCommand → ExecuteReader()
                // ============================================================
                try
                {
                    DataTable statusCounts = service.GetTaskStatusCounts();
                    rptStatusCounts.DataSource = statusCounts;
                    rptStatusCounts.DataBind();
                    queryCount++;
                    AppLogger.Create<SqlQueries>()?.LogInformation("Query 3 (Task Status Counts): {Count} groups", statusCounts.Rows.Count);
                }
                catch (Exception ex)
                {
                    lblStatusError.Text = $"Query 3 failed: {ex.Message}";
                    lblStatusError.Visible = true;
                    AppLogger.Create<SqlQueries>()?.LogError(ex, "Query 3 (Task Status Counts) failed");
                }

                // ============================================================
                // QUERY 4: Recent Audit Logs
                // ** MSSQL SPAN 4 ** — SqlConnection → SqlCommand → ExecuteReader()
                // ============================================================
                try
                {
                    DataTable auditLogs = service.GetRecentAuditLogs();
                    gvAuditLogs.DataSource = auditLogs;
                    gvAuditLogs.DataBind();
                    queryCount++;
                    AppLogger.Create<SqlQueries>()?.LogInformation("Query 4 (Audit Logs): {Count} rows", auditLogs.Rows.Count);
                }
                catch (Exception ex)
                {
                    lblAuditError.Text = $"Query 4 failed: {ex.Message}";
                    lblAuditError.Visible = true;
                    AppLogger.Create<SqlQueries>()?.LogError(ex, "Query 4 (Audit Logs) failed");
                }

                // ============================================================
                // QUERY 5: Overdue Tasks Report
                // ** MSSQL SPAN 5 ** — SqlConnection → SqlCommand → ExecuteReader()
                // ============================================================
                try
                {
                    DataTable overdueTasks = service.GetOverdueTasksReport();
                    gvOverdueTasks.DataSource = overdueTasks;
                    gvOverdueTasks.DataBind();
                    lblOverdueTasks.Text = overdueTasks.Rows.Count.ToString();
                    queryCount++;
                    AppLogger.Create<SqlQueries>()?.LogInformation("Query 5 (Overdue Tasks): {Count} rows", overdueTasks.Rows.Count);
                }
                catch (Exception ex)
                {
                    lblOverdueError.Text = $"Query 5 failed: {ex.Message}";
                    lblOverdueError.Visible = true;
                    AppLogger.Create<SqlQueries>()?.LogError(ex, "Query 5 (Overdue Tasks) failed");
                }

                // ============================================================
                // QUERY 6: Department Workload
                // ** MSSQL SPAN 6 ** — SqlConnection → SqlCommand → ExecuteReader()
                // ============================================================
                try
                {
                    DataTable deptWorkload = service.GetDepartmentWorkload();
                    gvDepartmentWorkload.DataSource = deptWorkload;
                    gvDepartmentWorkload.DataBind();

                    // Display summary using Labels
                    int totalDepts = deptWorkload.Rows.Count;
                    int totalActive = 0;
                    foreach (DataRow row in deptWorkload.Rows)
                    {
                        totalActive += Convert.ToInt32(row["ActiveTasks"]);
                    }
                    lblDeptSummary.Text = $"Summary: {totalDepts} departments with {totalActive} active tasks across all departments.";

                    queryCount++;
                    AppLogger.Create<SqlQueries>()?.LogInformation("Query 6 (Department Workload): {Count} departments", deptWorkload.Rows.Count);
                }
                catch (Exception ex)
                {
                    lblDeptError.Text = $"Query 6 failed: {ex.Message}";
                    lblDeptError.Visible = true;
                    AppLogger.Create<SqlQueries>()?.LogError(ex, "Query 6 (Department Workload) failed");
                }

                // ============================================================
                // QUERY 7: Task Comment Activity
                // ** MSSQL SPAN 7 ** — SqlConnection → SqlCommand → ExecuteReader()
                // ============================================================
                try
                {
                    DataTable comments = service.GetTaskCommentActivity();
                    gvComments.DataSource = comments;
                    gvComments.DataBind();
                    lblRecentComments.Text = comments.Rows.Count.ToString();
                    queryCount++;
                    AppLogger.Create<SqlQueries>()?.LogInformation("Query 7 (Comment Activity): {Count} rows", comments.Rows.Count);
                }
                catch (Exception ex)
                {
                    lblCommentError.Text = $"Query 7 failed: {ex.Message}";
                    lblCommentError.Visible = true;
                    AppLogger.Create<SqlQueries>()?.LogError(ex, "Query 7 (Comment Activity) failed");
                }

                stopwatch.Stop();

                // Update page header badges
                lblTimestamp.Text = $"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
                lblQueryCount.Text = $"{queryCount} / 7 queries succeeded";
                lblTotalTime.Text = $"Total: {stopwatch.ElapsedMilliseconds} ms";

                AppLogger.Create<SqlQueries>()?.LogInformation(
                    "SqlQueries.aspx completed — {QueryCount}/7 queries in {ElapsedMs}ms",
                    queryCount, stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
