# Build & Deploy Guide — TaskManager (.NET Framework 4.8)

This document describes how to stop IIS Express, rebuild, and redeploy the application after making code changes.

---

## Prerequisites

| Tool | Path |
|------|------|
| MSBuild | `C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe` |
| IIS Express | `C:\Program Files\IIS Express\iisexpress.exe` |
| Nginx (reverse proxy) | `c:\AtatusDotNet\dotnet-framework\dotnet-iis\nginx-win\nginx.exe` |
| Project | `c:\AtatusDotNet\dotnet-framework\dotnet-iis\TaskManager\TaskManager.csproj` |

---

## Quick Reference (Copy-Paste Commands)

### All-in-one: Stop → Build → Deploy

```powershell
# 1. Stop IIS Express
taskkill /IM iisexpress.exe /F 2>$null

# 2. Build
& "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" `
  "c:\AtatusDotNet\dotnet-framework\dotnet-iis\TaskManager\TaskManager.csproj" `
  /p:Configuration=Debug /v:minimal

# 3. Start IIS Express
& "C:\Program Files\IIS Express\iisexpress.exe" `
  /path:"C:\AtatusDotNet\dotnet-framework\dotnet-iis\TaskManager" `
  /port:4000
```

---

## Step-by-Step Details

### Step 1: Stop IIS Express

```powershell
taskkill /IM iisexpress.exe /F
```

This forcefully kills all running IIS Express instances. The `/F` flag forces termination.

> **Note:** You must stop IIS Express before rebuilding, otherwise the DLLs in the `bin\` folder may be locked.

### Step 2: Build the Project

```powershell
& "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" `
  "c:\AtatusDotNet\dotnet-framework\dotnet-iis\TaskManager\TaskManager.csproj" `
  /p:Configuration=Debug `
  /v:minimal
```

**Expected output on success:**
```
TaskManager -> c:\AtatusDotNet\dotnet-framework\dotnet-iis\TaskManager\bin\TaskManager.dll
```

**If there are errors**, they will appear as:
```
error CS####: <description>
```

Fix the errors and run the build command again.

#### Build Options

| Flag | Description |
|------|-------------|
| `/p:Configuration=Debug` | Debug build (default) |
| `/p:Configuration=Release` | Release/optimized build |
| `/v:minimal` | Minimal output (less noise) |
| `/v:normal` | Normal verbosity |
| `/t:Clean;Build` | Clean before build (full rebuild) |

#### Clean + Rebuild (if needed)

```powershell
& "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" `
  "c:\AtatusDotNet\dotnet-framework\dotnet-iis\TaskManager\TaskManager.csproj" `
  /p:Configuration=Debug /v:minimal /t:Clean;Build
```

### Step 3: Start IIS Express

```powershell
& "C:\Program Files\IIS Express\iisexpress.exe" `
  /path:"C:\AtatusDotNet\dotnet-framework\dotnet-iis\TaskManager" `
  /port:4000
```

**Expected output:**
```
Starting IIS Express ...
Successfully registered URL "http://localhost:4000/" for site "Development Web Site" application "/"
Registration completed
IIS Express is running.
```

The app is now accessible at: **http://localhost:4000/**

### Step 4: (Optional) Start Nginx Reverse Proxy

If you need the Nginx reverse proxy (for real client IP forwarding):

```powershell
# Stop existing Nginx
taskkill /IM nginx.exe /F 2>$null

# Start Nginx (run from its directory)
cd c:\AtatusDotNet\dotnet-framework\dotnet-iis\nginx-win
cmd.exe /c "nginx.exe"
```

Nginx listens on **port 80** and forwards to IIS Express on **port 4000**.

---

## Verify Deployment

```powershell
# Test homepage
Invoke-WebRequest -Uri "http://localhost:4000/" -UseBasicParsing | Select-Object StatusCode

# Test via Nginx (if running)
Invoke-WebRequest -Uri "http://localhost/" -UseBasicParsing | Select-Object StatusCode

# Check Atatus agent DLL version
[System.Diagnostics.FileVersionInfo]::GetVersionInfo(
  "C:\AtatusDotNet\dotnet-framework\dotnet-iis\TaskManager\bin\Atatus.dll"
).ProductVersion
```

---

## Test All Endpoints

```powershell
# Run the full endpoint test script
powershell -ExecutionPolicy Bypass -File "c:\AtatusDotNet\dotnet-framework\dotnet-iis\test-all-endpoints.ps1"
```

---

## Troubleshooting

### Build fails with "file is locked"
Stop IIS Express first: `taskkill /IM iisexpress.exe /F`

### IIS Express won't start / port in use
```powershell
# Check what's using port 4000
netstat -ano | findstr :4000

# Kill the process by PID
taskkill /PID <pid> /F
```

### App returns 500 error after deploy
Check the Serilog log file:
```powershell
Get-Content "c:\AtatusDotNet\dotnet-framework\dotnet-iis\TaskManager\App_Data\logs\*.log" -Tail 50
```

### Agent shows wrong version
Verify the `.csproj` HintPath points to the correct package:
```powershell
Select-String -Path "c:\AtatusDotNet\dotnet-framework\dotnet-iis\TaskManager\TaskManager.csproj" -Pattern "Atatus"
```
