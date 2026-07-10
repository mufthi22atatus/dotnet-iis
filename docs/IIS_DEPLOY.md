# IIS deployment runbook

Target: Windows Server 2019/2022 with IIS 10, ASP.NET 4.8 enabled.

## 1. Prep the server

```powershell
# Enable IIS + ASP.NET 4.8 features (Server Manager equivalent)
Install-WindowsFeature -Name Web-Server, Web-Asp-Net45, Web-Net-Ext45, Web-ISAPI-Ext, Web-ISAPI-Filter, Web-Mgmt-Console, NET-Framework-45-ASPNET, NET-Framework-45-Core
```

Install:
- **.NET Framework 4.8** runtime (likely already present on 2022)
- **SQL Server** (Express 2019+ or full) — note the instance name (e.g. `SQLEXPRESS`)
- (Optional) **IIS URL Rewrite** module for HTTPS redirects

## 2. Build the deployment package

From a developer workstation:

```cmd
nuget restore TaskManager.sln
msbuild TaskManager.sln /p:Configuration=Release /p:DeployOnBuild=true /p:WebPublishMethod=Package /p:PackageAsSingleFile=true /p:PackageLocation=publish\TaskManager.zip
```

Or use Visual Studio: right-click `TaskManager` → **Publish** → **Folder** → `bin\publish`.

## 3. Lay out the files on the server

```
C:\inetpub\TaskManager\
  bin\
  Content\
  Scripts\
  Views\
  App_Data\          (writable)
  Uploads\           (writable, optional move to D:\Data\Uploads)
  Logs\              (writable)
  Web.config
  Global.asax
  ...
```

Recommended ACLs:
- The IIS app pool identity (default `IIS APPPOOL\TaskManager`) needs:
  - **Read** on the entire site root
  - **Modify** on `Logs\`, `Uploads\`, `App_Data\`

```powershell
icacls C:\inetpub\TaskManager\Logs    /grant "IIS APPPOOL\TaskManager:(OI)(CI)M"
icacls C:\inetpub\TaskManager\Uploads /grant "IIS APPPOOL\TaskManager:(OI)(CI)M"
icacls C:\inetpub\TaskManager\App_Data /grant "IIS APPPOOL\TaskManager:(OI)(CI)M"
```

## 4. Create the IIS application pool

- **Name**: `TaskManager`
- **.NET CLR Version**: `v4.0`
- **Managed Pipeline**: `Integrated`
- **Identity**: `ApplicationPoolIdentity` (or a domain svc account if SQL uses Windows auth)
- **Idle time-out**: `0` if you want background jobs to keep ticking
- **Recycling → Regular Time Interval**: `0` (we don't want recycles to interrupt timers in dev/test)

PowerShell:
```powershell
Import-Module WebAdministration
New-WebAppPool -Name TaskManager
Set-ItemProperty IIS:\AppPools\TaskManager -Name managedRuntimeVersion -Value "v4.0"
Set-ItemProperty IIS:\AppPools\TaskManager -Name managedPipelineMode -Value Integrated
Set-ItemProperty IIS:\AppPools\TaskManager -Name processModel.idleTimeout -Value "00:00:00"
```

## 5. Create the IIS site

```powershell
New-Website -Name "TaskManager" -PhysicalPath "C:\inetpub\TaskManager" -ApplicationPool "TaskManager" -Port 80 -Force
```

Bind HTTPS via IIS Manager → Bindings → Add → Type `https` → select your TLS cert.

## 6. Create the database

```powershell
sqlcmd -S ".\SQLEXPRESS" -i .\db\01_schema.sql
# Optional manual seed (or let EF Code First handle it)
sqlcmd -S ".\SQLEXPRESS" -i .\db\02_seed.sql
```

Grant the app pool identity SQL access:

```sql
USE master;
CREATE LOGIN [IIS APPPOOL\TaskManager] FROM WINDOWS;
USE TaskManagerDb;
CREATE USER [IIS APPPOOL\TaskManager] FOR LOGIN [IIS APPPOOL\TaskManager];
ALTER ROLE db_datareader ADD MEMBER [IIS APPPOOL\TaskManager];
ALTER ROLE db_datawriter ADD MEMBER [IIS APPPOOL\TaskManager];
ALTER ROLE db_ddladmin   ADD MEMBER [IIS APPPOOL\TaskManager]; -- needed for EF init
```

Then update `Web.config` connection string:
```xml
<add name="AppDbContext"
     connectionString="Data Source=PROD-SQL\INST;Initial Catalog=TaskManagerDb;Integrated Security=True;MultipleActiveResultSets=True"
     providerName="System.Data.SqlClient" />
```

## 7. Verify

1. `curl http://localhost/` → returns the homepage HTML.
2. `curl -I http://localhost/` → contains `X-Request-Id` and `X-Application: TaskManager`.
3. Log in and exercise the app (see telemetry surfaces below).
4. Check `C:\inetpub\TaskManager\Logs\taskmanager-YYYYMMDD.log` for startup + request lines.

## 8. APM / profiler attachment

This app is designed for instrumentation. Most APM agents (Atatus, Datadog, NewRelic,
AppDynamics, Dynatrace, App Insights) attach via either:

- **CLR profiler**: env vars on the IIS site / app pool
  ```
  COR_ENABLE_PROFILING=1
  COR_PROFILER={your-agent-clsid}
  COR_PROFILER_PATH=C:\Program Files\YourAgent\profiler.dll
  ```
  Set per-app-pool via IIS Manager → Configuration Editor → `system.applicationHost/applicationPools` →
  `[your pool] → environmentVariables`. Recycle the pool after.

- **HTTP module**: drop the agent assembly into `bin\` and add a `<system.webServer><modules>` entry. The two custom modules in `Web.config` (`RequestLoggingModule`, `SecurityHeadersModule`) demonstrate the registration pattern.

After attaching, expect:
- DB spans on every controller / API call
- Outbound HTTP spans on `/Dashboard` and `/api/external/*`
- File I/O spans on `/Tasks/{id}/Upload`
- Background jobs producing spans every 60s (reminders) and every 5 min (cleanup)
- Correlation IDs in the `X-Request-Id` header for end-to-end tracing

## 9. Troubleshooting

| Symptom                                     | Likely cause                                                        |
|---------------------------------------------|---------------------------------------------------------------------|
| `HTTP Error 500.19 - Internal Server Error` | Web.config syntax issue; check Event Viewer → Application           |
| `Could not load file or assembly`           | NuGet packages weren't restored; redo step 2                        |
| `Login failed for user 'IIS APPPOOL\...'`   | SQL grant missing; redo step 6                                       |
| Permissions error writing logs / uploads    | App pool identity lacks Modify ACL on those folders                 |
| Background jobs don't tick                  | App pool idle timeout > 0 — set to 0                                 |
| 404 on `/api/tasks`                         | `runAllManagedModulesForAllRequests` is required (already in config)|
| `customErrors` swallowing real exception    | Set `customErrors mode="Off"` in `Web.config` while debugging       |

## 10. Hardening checklist

- [ ] HTTPS-only binding + redirect (URL Rewrite or `<httpRedirect>`)
- [ ] `Web.Release.config` injects HSTS header — verify in response
- [ ] Move the connection string to an encrypted section (`aspnet_regiis -pe`)
- [ ] Rotate the seeded admin password immediately
- [ ] Limit upload size (already 10 MB by default; reduce if needed)
- [ ] Run AV scan over `Uploads/` periodically
- [ ] Forward Serilog to centralized logging (add a sink: Seq, ELK, Splunk)
