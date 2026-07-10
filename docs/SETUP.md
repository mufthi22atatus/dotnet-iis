# Local setup

## Prerequisites

| Tool                           | Version                          |
|--------------------------------|----------------------------------|
| Windows 10 / 11 or Server 2019+| 64-bit                           |
| Visual Studio                  | 2019 16.11+ or 2022 17.x         |
| .NET Framework Developer Pack  | 4.8                              |
| SQL Server                     | LocalDB (bundled with VS) **or** Express 2017+ |
| IIS or IIS Express             | IIS 10+ for production deployment |
| NuGet CLI                      | 6.x (or VS-bundled)              |

## NuGet packages used

| Package                                | Version |
|----------------------------------------|---------|
| EntityFramework                        | 6.4.4   |
| Microsoft.AspNet.Mvc                   | 5.2.9   |
| Microsoft.AspNet.Razor                 | 3.2.9   |
| Microsoft.AspNet.WebApi                | 5.2.9   |
| Microsoft.AspNet.WebApi.Client         | 5.2.9   |
| Microsoft.AspNet.WebApi.Core           | 5.2.9   |
| Microsoft.AspNet.WebApi.WebHost        | 5.2.9   |
| Microsoft.AspNet.WebPages              | 3.3.0   |
| Microsoft.AspNet.Web.Optimization      | 1.1.3   |
| Newtonsoft.Json                        | 13.0.3  |
| Serilog                                | 4.3.1   |
| Serilog.Extensions.Logging             | 8.0.0   |
| Serilog.Sinks.File                     | 7.0.0   |
| NLog                                   | 6.1.0   |
| NLog.Extensions.Logging                | 6.1.0   |
| System.Configuration.ConfigurationManager | 6.0.0 |
| Antlr                                  | 3.5.0.2 |
| WebGrease                              | 1.6.0   |
| jQuery                                 | 3.7.1   |
| bootstrap                              | 5.3.2   |
| Microsoft.Web.Infrastructure           | 2.0.0   |

`packages.config` already pins these; **right-click solution → Restore NuGet Packages** is enough.

## Local run (IIS Express via Visual Studio)

1. Open `TaskManager.sln` in Visual Studio.
2. Restore NuGet packages.
3. Confirm `Web.config` connection string targets LocalDB:
   ```xml
   Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=TaskManagerDb;Integrated Security=True;...
   ```
4. Press **F5**. On first launch, EF Code-First will:
   - create the database
   - run `Data\DbInitializer.cs` to seed 5 users and 5 tasks
5. Browser opens at `http://localhost:8080/`. Log in as `admin@taskmanager.local` / `Admin@12345`.

## Manual DB provisioning (alternative)

If you prefer to provision SQL by hand instead of letting EF do it:

```powershell
sqlcmd -S "(LocalDB)\MSSQLLocalDB" -i db\01_schema.sql
sqlcmd -S "(LocalDB)\MSSQLLocalDB" -i db\02_seed.sql
```

The seed script writes placeholder password hashes; let the EF initializer take over for working creds, or paste in real PBKDF2 hashes you compute via `PasswordHasher.Hash`.

## Switching to SQL Server Express

Edit `Web.config`:
```xml
<add name="AppDbContext"
     connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=TaskManagerDb;Integrated Security=True;MultipleActiveResultSets=True"
     providerName="System.Data.SqlClient" />
```

Make sure your SQL Server Express service runs as a user that owns the directory if you are using file-attached databases.

## Configuration knobs (`Web.config` `<appSettings>`)

| Key                              | Purpose                              | Default            |
|----------------------------------|--------------------------------------|--------------------|
| `App:UploadRoot`                 | Disk dir for uploaded attachments    | `~/Uploads`        |
| `App:MaxUploadSizeBytes`         | Per-file upload limit                | 10 MB              |
| `App:LogRoot`                    | Serilog log directory                | `~/Logs`           |
| `External:WeatherApiUrl`         | open-meteo base URL                  | open-meteo current |
| `External:GitHubApiUrl`          | GitHub API base                      | api.github.com     |
| `Jobs:Enabled`                   | Master switch for background timers  | `true`             |
| `Jobs:ReminderIntervalSeconds`   | Reminder tick rate                   | `60`               |
| `Jobs:CleanupIntervalSeconds`    | Audit cleanup tick rate              | `300`              |
| `Cache:DashboardSeconds`         | Dashboard cache TTL                  | `30`               |

## Debug vs Release

- **Debug**: `compilation debug="true"`, source maps, no transforms beyond `Web.Debug.config` adding an `Environment=Debug` flag.
- **Release**: `compilation debug="false"`, custom errors `RemoteOnly`, HSTS header injected, prod connection string token. Use `msbuild /p:Configuration=Release /p:DeployOnBuild=true /p:PublishProfile=...` or the VS Publish wizard.

## Logs

Logging goes through `Microsoft.Extensions.Logging` (`AppLogger.Create<T>()`), backed by one of three
providers chosen at startup via the `LOGGING_PROVIDER` environment variable:

| `LOGGING_PROVIDER` value | Backend                                    | Log file                                  |
|--------------------------|---------------------------------------------|--------------------------------------------|
| unset / `ilogger` (default) | Built-in `SimpleFileLoggerProvider`      | `TaskManager/Logs/taskmanager-YYYYMMDD.log` |
| `serilog`                | Serilog + `Serilog.Sinks.File`, daily rolling, 14 day retention | `TaskManager/Logs/taskmanager-YYYYMMDD.log` |
| `nlog`                   | NLog `FileTarget`, daily archive, 14 files kept | `TaskManager/Logs/taskmanager.log` (archived to `taskmanager-YYYYMMDD.log`) |

The active provider is logged on startup (`Logging provider configured: {LoggingProvider}`).

Tail with PowerShell:
```powershell
Get-Content TaskManager\Logs\taskmanager-20260508.log -Wait -Tail 50
```

## Telemetry knobs

The app already produces dense telemetry. To increase load:

- Hit `/Home/Boom` repeatedly to generate exception traces
- POST/PUT to `/api/tasks` in a loop with curl
- Lower `Jobs:ReminderIntervalSeconds` to `10` for chatty background spans
