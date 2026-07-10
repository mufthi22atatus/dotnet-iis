$ErrorActionPreference = "Continue"
$base = "http://localhost:8082"

Write-Host "=== TaskManager API Test ===" -ForegroundColor Cyan

# Step 1: Get login page + anti-forgery token
Write-Host "`n[1] Fetching login page..." -ForegroundColor Yellow
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
$loginPage = Invoke-WebRequest -Uri "$base/Account/Login" -WebSession $session -UseBasicParsing -TimeoutSec 30
$token = ([regex]'__RequestVerificationToken.*?value="([^"]+)"').Match($loginPage.Content).Groups[1].Value
Write-Host "  Token: $($token.Substring(0, 20))..."

# Step 2: Login
Write-Host "`n[2] Logging in as admin..." -ForegroundColor Yellow
$body = "__RequestVerificationToken=$([uri]::EscapeDataString($token))&Email=admin@taskmanager.local&Password=Admin@12345"
$loginResult = Invoke-WebRequest -Uri "$base/Account/Login" -Method POST -Body $body -ContentType "application/x-www-form-urlencoded" -WebSession $session -UseBasicParsing -MaximumRedirection 5 -TimeoutSec 30
Write-Host "  Login status: $($loginResult.StatusCode)"
$cookies = $session.Cookies.GetCookies($base) | Select-Object Name
Write-Host "  Cookies: $($cookies.Name -join ', ')"

# Step 3: Test API endpoints
Write-Host "`n[3] Testing API endpoints..." -ForegroundColor Yellow 
$endpoints = @(
    @{ Method="GET";  Url="$base/api/tasks" }
    @{ Method="GET";  Url="$base/api/users" }
    @{ Method="GET";  Url="$base/api/dashboard/summary" }
    @{ Method="GET";  Url="$base/api/dashboard/by-status" }
    @{ Method="GET";  Url="$base/api/tasks/1/labels" }
    @{ Method="GET";  Url="$base/api/tasks/1/watchers" }
    @{ Method="GET";  Url="$base/api/tasks/1/timelogs" }
    @{ Method="GET";  Url="$base/api/tasks/1/comments" }
    @{ Method="GET";  Url="$base/api/tasks/1/dependencies" }
    @{ Method="GET";  Url="$base/api/tasks/1/attachments" }
    @{ Method="GET";  Url="$base/api/reports/daily" }
    @{ Method="GET";  Url="$base/api/reports/weekly" }
    @{ Method="GET";  Url="$base/api/reports/comprehensive" }
    @{ Method="GET";  Url="$base/api/search/tasks?q=backup" }
    @{ Method="GET";  Url="$base/api/search/by-status/0" }
)

$pass = 0; $fail = 0
foreach ($ep in $endpoints) {
    try {
        $r = Invoke-WebRequest -Uri $ep.Url -Method $ep.Method -WebSession $session -UseBasicParsing -TimeoutSec 15
        $shortUrl = $ep.Url.Replace($base, "")
        Write-Host "  $($ep.Method) $shortUrl => $($r.StatusCode) OK" -ForegroundColor Green
        $pass++
    } catch {
        $sc = $_.Exception.Response.StatusCode.value__
        $shortUrl = $ep.Url.Replace($base, "")
        Write-Host "  $($ep.Method) $shortUrl => $sc FAIL" -ForegroundColor Red
        $fail++
    }
}

# Step 4: Test workflow - create a full task
Write-Host "`n[4] Testing workflow: create-full-task..." -ForegroundColor Yellow
$taskBody = @{
    Title = "Test Task from API"
    Description = "Created via automated API test"
    Priority = 1
    AssignedToId = 2
    Labels = @("api-test", "automated")
    WatcherIds = @(1, 2)
} | ConvertTo-Json
try {
    $r = Invoke-WebRequest -Uri "$base/api/workflow/create-full-task" -Method POST -Body $taskBody -ContentType "application/json" -WebSession $session -UseBasicParsing -TimeoutSec 15
    Write-Host "  POST /api/workflow/create-full-task => $($r.StatusCode) OK" -ForegroundColor Green
    $pass++
    $newTask = $r.Content | ConvertFrom-Json
    Write-Host "  Created task ID: $($newTask.id)" -ForegroundColor Cyan
} catch {
    $sc = $_.Exception.Response.StatusCode.value__
    Write-Host "  POST /api/workflow/create-full-task => $sc FAIL" -ForegroundColor Red
    $fail++
}

Write-Host "`n=== Results: $pass passed, $fail failed ===" -ForegroundColor Cyan
