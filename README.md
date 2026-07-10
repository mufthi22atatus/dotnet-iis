# TaskManager — ASP.NET MVC 5 + WebForms / .NET Framework 4.8

Production-shaped Employee Task Management sample app, built specifically to generate
rich telemetry for APM / profiler instrumentation testing on IIS.

## Architecture

- **Application**: Runs locally on .NET Framework 4.8 (IIS Express / Visual Studio)
- **Database**: SQL Server 2022 runs in Docker (container on `localhost:1433`)
- **Data Access**: EF6 (existing MVC) + Microsoft.Data.SqlClient raw ADO.NET (WebForms)

## What's inside

- ASP.NET MVC 5.2 + Web API 2 on **.NET Framework 4.8**
- **ASP.NET WebForms** page with 7 independent SQL queries (`SqlQueries.aspx`)
- Entity Framework 6.4 (Code First) with auto-seeding initializer
- **Microsoft.Data.SqlClient** for raw ADO.NET queries (SqlConnection → SqlCommand → ExecuteReader)
- **Docker SQL Server 2022** as database backend
- Custom Forms Authentication (PBKDF2 password hashing)
- Role-based authorization (Employee / Manager / Admin)
- File upload + download with disk storage
- Background jobs: due-task reminders + audit log archival (in-process timers)
- External HTTP calls: open-meteo weather, GitHub repo info
- HTTP modules: per-request logging + correlation header + security headers
- Filters: MVC + Web API exception handlers, action timing
- Caching via `MemoryCache` + `HttpRuntime.Cache`
- Session state for current-user metadata
- Serilog rolling-file logging
- Bundling/minification (`System.Web.Optimization`)
- Bootstrap 5 + jQuery 3.7 frontend with AJAX-friendly REST API
- Razor views with login, registration, CRUD, dashboard, admin

## Telemetry surfaces (for APM)

| Surface            | Where                                                                    |
|--------------------|--------------------------------------------------------------------------|
| DB spans (EF6)     | Every controller / API / service call hits `AppDbContext` via EF6       |
| **DB spans (ADO.NET)** | **`SqlQueries.aspx` → 7 independent MSSQL spans via Microsoft.Data.SqlClient** |
| HTTP spans         | `ExternalApiClient` → open-meteo, GitHub. Triggered by Dashboard + API  |
| File I/O           | `FileStorageService` (uploads), `StaleTaskCleanupJob` (CSV writes)      |
| Background jobs    | `TaskReminderJob` (every 60s), `StaleTaskCleanupJob` (every 5 min)      |
| Auth flows         | `AuthService.AuthenticateAsync` / `RegisterAsync`                       |
| Exceptions         | `/Home/Boom` deliberate throw; `MvcExceptionFilter`, `ApiExceptionFilter` |
| HTTP module        | `RequestLoggingModule` logs Begin/End for every request                 |
| Cache              | Dashboard view served from `MemoryCache` after first call               |

## Folder layout

```
TaskManager.sln
docker-compose.yml          ← Docker SQL Server 2022 setup
db/
  docker-init.sql           ← Combined schema + seed for Docker
  docker-entrypoint.sh      ← Docker startup script
  01_schema.sql             ← Manual schema script (alternative to EF init)
  02_seed.sql               ← Manual seed script
TaskManager/
  App_Start/        Route, Filter, Bundle, WebApi, Logging, DI configs
  Background/       Timer-driven jobs
  Controllers/      MVC + /Controllers/Api/* for Web API
  Data/             AppDbContext + Code-First entities + repositories
    SqlQueryService.cs  ← Microsoft.Data.SqlClient with 7 independent queries
  Filters/          Auth, exception, action filters
  Helpers/          Razor HTML helpers (badges)
  Modules/          IIS HTTP modules (RequestLogging, SecurityHeaders)
  Services/         App services (Auth, Tasks, Files, External HTTP, Cache, Audit, Notify)
  ViewModels/       Form / DTO classes
  Views/            Razor views
  WebForms/         ← NEW: ASP.NET WebForms pages
    SqlQueries.aspx       ← WebForms page with GridView/Repeater/Labels
    SqlQueries.aspx.cs    ← Code-behind (7 SQL queries)
    SqlQueries.aspx.designer.cs
  Content/, Scripts/  Static assets
  Logs/             Serilog rolling logs (created at startup)
  Uploads/          Attachment storage (created on first upload)
  Web.config        IIS config + connection strings + module registrations
  packages.config   NuGet refs (includes Microsoft.Data.SqlClient)
docs/
  SETUP.md          Local dev instructions
  IIS_DEPLOY.md     Production IIS deployment runbook
```

## Prerequisites

- Visual Studio 2019 / 2022
- .NET Framework 4.8 SDK
- Docker Desktop (for SQL Server)

## Quick start

### 1. Start SQL Server (Docker)

```bash
# From the solution root (dotnet-iis/)
docker-compose up -d

# Wait for SQL Server to be ready (check health)
docker-compose ps

# Verify the database was created
docker exec taskmanager-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "TaskMgr@2024!" -C \
  -Q "SELECT name FROM sys.databases WHERE name = 'TaskManagerDb'"
```

### 2. Initialize the database

```bash
# Run the init script to create tables and seed data
docker exec -i taskmanager-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "TaskMgr@2024!" -C \
  -i /docker-entrypoint-initdb.d/docker-init.sql
```

### 3. Build and run the application

1. Open `TaskManager.sln` in Visual Studio 2019 / 2022.
2. Right-click the solution → **Restore NuGet Packages**.
3. F5 to run. The application connects to Docker SQL Server on `localhost:1433`.
4. Log in as `admin@taskmanager.local` / `Admin@12345`.

### 4. Test the WebForms SQL Queries page

Navigate to: **`/WebForms/SqlQueries.aspx`**

This page executes **7 independent SQL queries**, each using:
- `SqlConnection` (new connection per query)
- `SqlCommand`
- `SqlDataReader`
- `ExecuteReader()`

Expected APM output per page request:
- **1 ASP.NET WebForms request** span
- **7 MSSQL query spans** (one per SqlQueryService method)

## Docker commands

```bash
# Start SQL Server
docker-compose up -d

# Stop SQL Server (preserves data)
docker-compose down

# Stop + remove data volume (full reset)
docker-compose down -v

# View SQL Server logs
docker-compose logs sqlserver

# Connect via sqlcmd
docker exec -it taskmanager-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "TaskMgr@2024!" -C
```

## Generating telemetry

After login, click around to exercise the surfaces:

- `/WebForms/SqlQueries.aspx` — **7 independent MSSQL spans** via Microsoft.Data.SqlClient
- `/Tasks` — DB query + list rendering (EF6)
- `/Tasks/Create` then submit — DB write + notification span + cache invalidation
- `/Tasks/Details/1` → upload a file — file I/O + DB write
- `/Dashboard` — DB aggregations + cache + external HTTP (weather)
- `/api/external/github/dotnet/runtime` — outbound HTTP span
- `/api/tasks` (GET / POST / PUT / DELETE) — JSON REST surface
- `/Home/Boom` — synthetic 500 with stack trace
- Wait 60s for the reminder job and 5 min for the cleanup job to tick

Logs land in `TaskManager/Logs/taskmanager-yyyyMMdd.log`.

## Connection strings

| Name                | Provider                  | Used By              |
|---------------------|---------------------------|----------------------|
| `AppDbContext`      | `System.Data.SqlClient`   | EF6 (MVC controllers)|
| `SqlClientConnection`| `Microsoft.Data.SqlClient`| WebForms SqlQueries  |

Both point to the same Docker SQL Server instance (`localhost:1433`, SA auth).

## Test accounts (seeded)

| Role     | Email                          | Password        |
|----------|--------------------------------|-----------------|
| Admin    | admin@taskmanager.local        | `Admin@12345`   |
| Manager  | manager@taskmanager.local      | `Manager@12345` |
| Employee | alice@taskmanager.local        | `Alice@12345`   |
| Employee | bob@taskmanager.local          | `Bob@12345`     |
| Employee | charlie@taskmanager.local      | `Charlie@12345` |


