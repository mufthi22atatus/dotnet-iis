<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SqlQueries.aspx.cs" Inherits="TaskManager.WebForms.SqlQueries" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>SQL Queries Dashboard — TaskManager WebForms</title>
    <meta name="description" content="TaskManager SQL Queries page demonstrating Microsoft.Data.SqlClient with 7 independent MSSQL spans for APM tracing" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css"
          integrity="sha384-T3c6CoIi6uLrA9TneNEoa7RxnatzjcDSCmG1MXxSR1GAsXEV/Dwwykc2MPK8M2HN" crossorigin="anonymous" />
    <style>
        body { padding-top: 70px; background-color: #f8f9fa; }
        .navbar-brand { font-weight: 600; }
        .query-card { margin-bottom: 1.5rem; border: none; box-shadow: 0 2px 8px rgba(0,0,0,.08); }
        .query-card .card-header { font-weight: 600; font-size: 0.95rem; }
        .span-badge { font-size: 0.7rem; vertical-align: middle; }
        .table thead th { font-size: .8rem; text-transform: uppercase; letter-spacing: .04em; color: #555; }
        .table tbody td { font-size: .85rem; }
        .stat-card { text-align: center; padding: 1.2rem; }
        .stat-card .stat-value { font-size: 2rem; font-weight: 700; color: #0d6efd; }
        .stat-card .stat-label { font-size: 0.8rem; color: #666; text-transform: uppercase; }
        .error-alert { margin-top: 0.5rem; }
        .page-header { margin-bottom: 2rem; }
        .page-header h1 { font-weight: 700; }
        .page-header .text-muted { font-size: 0.9rem; }
        .repeater-item { display: inline-block; margin: 0.25rem; }
        footer { padding: 1rem 0; margin-top: 2rem; border-top: 1px solid #dee2e6; }
    </style>
</head>
<body>
    <!-- Navigation bar consistent with MVC layout -->
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark fixed-top">
        <div class="container">
            <a class="navbar-brand" href="/">TaskManager</a>
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#nav">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div id="nav" class="collapse navbar-collapse">
                <ul class="navbar-nav me-auto">
                    <li class="nav-item"><a class="nav-link" href="/Tasks">Tasks</a></li>
                    <li class="nav-item"><a class="nav-link" href="/Dashboard">Dashboard</a></li>
                    <li class="nav-item"><a class="nav-link" href="/Diagnostics">Diagnostics</a></li>
                    <li class="nav-item"><a class="nav-link active" href="/WebForms/SqlQueries.aspx">SQL Queries</a></li>
                </ul>
            </div>
        </div>
    </nav>

    <form id="form1" runat="server">
        <div class="container">

            <!-- Page Header -->
            <div class="page-header">
                <h1>SQL Queries Dashboard <span class="badge bg-primary span-badge">WebForms</span></h1>
                <p class="text-muted">
                    This page executes <strong>7 independent SQL queries</strong> using
                    <code>Microsoft.Data.SqlClient</code> with separate <code>SqlConnection</code> → 
                    <code>SqlCommand</code> → <code>ExecuteReader()</code> calls.
                    Each query generates a distinct <strong>MSSQL span</strong> in APM/tracing tools.
                </p>
                <asp:Label ID="lblTimestamp" runat="server" CssClass="badge bg-secondary" />
                <asp:Label ID="lblQueryCount" runat="server" CssClass="badge bg-success" />
                <asp:Label ID="lblTotalTime" runat="server" CssClass="badge bg-info text-dark" />
            </div>

            <!-- ============================================================ -->
            <!-- SECTION 1: Summary Statistics (from multiple queries)        -->
            <!-- ============================================================ -->
            <div class="row mb-4">
                <div class="col-md-3">
                    <div class="card query-card">
                        <div class="stat-card">
                            <asp:Label ID="lblTotalEmployees" runat="server" CssClass="stat-value" Text="0" />
                            <div class="stat-label">Total Employees</div>
                        </div>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="card query-card">
                        <div class="stat-card">
                            <asp:Label ID="lblTotalTasks" runat="server" CssClass="stat-value" Text="0" />
                            <div class="stat-label">Total Tasks</div>
                        </div>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="card query-card">
                        <div class="stat-card">
                            <asp:Label ID="lblOverdueTasks" runat="server" CssClass="stat-value text-danger" Text="0" />
                            <div class="stat-label">Overdue Tasks</div>
                        </div>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="card query-card">
                        <div class="stat-card">
                            <asp:Label ID="lblRecentComments" runat="server" CssClass="stat-value text-success" Text="0" />
                            <div class="stat-label">Recent Comments</div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- ============================================================ -->
            <!-- QUERY 1: Employee Summary — GridView                         -->
            <!-- ** MSSQL SPAN 1 ** generated by SqlQueryService              -->
            <!-- ============================================================ -->
            <div class="card query-card">
                <div class="card-header bg-primary text-white">
                    <span class="badge bg-warning text-dark span-badge me-1">SPAN 1</span>
                    Query 1: Employee Summary
                </div>
                <div class="card-body">
                    <asp:Label ID="lblEmployeeError" runat="server" CssClass="alert alert-danger error-alert" Visible="false" />
                    <div class="table-responsive">
                        <asp:GridView ID="gvEmployees" runat="server" CssClass="table table-striped table-hover table-sm"
                            AutoGenerateColumns="true" EmptyDataText="No employee data found."
                            GridLines="None" AllowPaging="false" />
                    </div>
                </div>
            </div>

            <!-- ============================================================ -->
            <!-- QUERY 2: Task Overview — GridView                            -->
            <!-- ** MSSQL SPAN 2 ** generated by SqlQueryService              -->
            <!-- ============================================================ -->
            <div class="card query-card">
                <div class="card-header bg-success text-white">
                    <span class="badge bg-warning text-dark span-badge me-1">SPAN 2</span>
                    Query 2: Task Overview
                </div>
                <div class="card-body">
                    <asp:Label ID="lblTaskError" runat="server" CssClass="alert alert-danger error-alert" Visible="false" />
                    <div class="table-responsive">
                        <asp:GridView ID="gvTasks" runat="server" CssClass="table table-striped table-hover table-sm"
                            AutoGenerateColumns="true" EmptyDataText="No task data found."
                            GridLines="None" AllowPaging="false" />
                    </div>
                </div>
            </div>

            <!-- ============================================================ -->
            <!-- QUERY 3: Task Status Counts — Repeater                       -->
            <!-- ** MSSQL SPAN 3 ** generated by SqlQueryService              -->
            <!-- ============================================================ -->
            <div class="card query-card">
                <div class="card-header bg-info text-dark">
                    <span class="badge bg-warning text-dark span-badge me-1">SPAN 3</span>
                    Query 3: Task Status Distribution
                </div>
                <div class="card-body">
                    <asp:Label ID="lblStatusError" runat="server" CssClass="alert alert-danger error-alert" Visible="false" />
                    <div class="row">
                        <asp:Repeater ID="rptStatusCounts" runat="server">
                            <ItemTemplate>
                                <div class="col-md-2 col-sm-4 mb-3">
                                    <div class="card text-center">
                                        <div class="card-body p-3">
                                            <h3 class="mb-1 text-primary"><%# Eval("TaskCount") %></h3>
                                            <small class="text-muted"><%# Eval("StatusName") %></small>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>

            <!-- ============================================================ -->
            <!-- QUERY 4: Recent Audit Logs — GridView                        -->
            <!-- ** MSSQL SPAN 4 ** generated by SqlQueryService              -->
            <!-- ============================================================ -->
            <div class="card query-card">
                <div class="card-header bg-secondary text-white">
                    <span class="badge bg-warning text-dark span-badge me-1">SPAN 4</span>
                    Query 4: Recent Audit Logs (Top 20)
                </div>
                <div class="card-body">
                    <asp:Label ID="lblAuditError" runat="server" CssClass="alert alert-danger error-alert" Visible="false" />
                    <div class="table-responsive">
                        <asp:GridView ID="gvAuditLogs" runat="server" CssClass="table table-striped table-hover table-sm"
                            AutoGenerateColumns="true" EmptyDataText="No audit log entries found."
                            GridLines="None" AllowPaging="false" />
                    </div>
                </div>
            </div>

            <!-- ============================================================ -->
            <!-- QUERY 5: Overdue Tasks Report — GridView                     -->
            <!-- ** MSSQL SPAN 5 ** generated by SqlQueryService              -->
            <!-- ============================================================ -->
            <div class="card query-card">
                <div class="card-header bg-danger text-white">
                    <span class="badge bg-warning text-dark span-badge me-1">SPAN 5</span>
                    Query 5: Overdue Tasks Report
                </div>
                <div class="card-body">
                    <asp:Label ID="lblOverdueError" runat="server" CssClass="alert alert-danger error-alert" Visible="false" />
                    <div class="table-responsive">
                        <asp:GridView ID="gvOverdueTasks" runat="server" CssClass="table table-striped table-hover table-sm"
                            AutoGenerateColumns="true" EmptyDataText="No overdue tasks — great job!"
                            GridLines="None" AllowPaging="false" />
                    </div>
                </div>
            </div>

            <!-- ============================================================ -->
            <!-- QUERY 6: Department Workload — GridView + Labels             -->
            <!-- ** MSSQL SPAN 6 ** generated by SqlQueryService              -->
            <!-- ============================================================ -->
            <div class="card query-card">
                <div class="card-header bg-warning text-dark">
                    <span class="badge bg-dark span-badge me-1">SPAN 6</span>
                    Query 6: Department Workload
                </div>
                <div class="card-body">
                    <asp:Label ID="lblDeptError" runat="server" CssClass="alert alert-danger error-alert" Visible="false" />
                    <div class="table-responsive">
                        <asp:GridView ID="gvDepartmentWorkload" runat="server" CssClass="table table-striped table-hover table-sm"
                            AutoGenerateColumns="true" EmptyDataText="No department workload data."
                            GridLines="None" AllowPaging="false" />
                    </div>
                    <div class="mt-2">
                        <asp:Label ID="lblDeptSummary" runat="server" CssClass="text-muted" />
                    </div>
                </div>
            </div>

            <!-- ============================================================ -->
            <!-- QUERY 7: Task Comment Activity — GridView                    -->
            <!-- ** MSSQL SPAN 7 ** generated by SqlQueryService              -->
            <!-- ============================================================ -->
            <div class="card query-card">
                <div class="card-header bg-dark text-white">
                    <span class="badge bg-warning text-dark span-badge me-1">SPAN 7</span>
                    Query 7: Recent Comment Activity (Top 15)
                </div>
                <div class="card-body">
                    <asp:Label ID="lblCommentError" runat="server" CssClass="alert alert-danger error-alert" Visible="false" />
                    <div class="table-responsive">
                        <asp:GridView ID="gvComments" runat="server" CssClass="table table-striped table-hover table-sm"
                            AutoGenerateColumns="true" EmptyDataText="No comment activity found."
                            GridLines="None" AllowPaging="false" />
                    </div>
                </div>
            </div>

            <!-- Footer -->
            <footer class="text-muted small">
                &copy; <%= DateTime.UtcNow.Year %> TaskManager — ASP.NET WebForms (.NET Framework 4.8)
                | Microsoft.Data.SqlClient | 7 MSSQL Spans per request
            </footer>

        </div>
    </form>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"
            integrity="sha384-C6RzsynM9kWDrMNeT87bh95OGNyZPhcTNXj1NW7RuBCsyN/o0jlpcV8Qyq46cDfL" crossorigin="anonymous">
    </script>
</body>
</html>
