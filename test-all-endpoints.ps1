# ─────────────────────────────────────────────────────────────────────
#  test-all-endpoints.ps1  —  Exercises every TaskManager endpoint
#  to generate rich APM telemetry (transactions, errors, external spans).
#
#  Usage:
#    .\test-all-endpoints.ps1 [-BaseUrl http://localhost]
# ─────────────────────────────────────────────────────────────────────

param(
    [string]$BaseUrl = "http://localhost"
)

$Pass = 0
$Fail = 0
$Total = 0

# ── Create a session with cookies ────────────────────────────────────
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession

function Hit {
    param(
        [string]$Label,
        [string]$Url,
        [int]$Expect = 200,
        [switch]$AnyStatus
    )
    $script:Total++
    try {
        $resp = Invoke-WebRequest -Uri $Url -WebSession $session -MaximumRedirection 5 -TimeoutSec 30 -UseBasicParsing -ErrorAction Stop
        $status = $resp.StatusCode
    } catch {
        $status = [int]$_.Exception.Response.StatusCode
        if ($status -eq 0) { $status = 999 }
    }

    if ($AnyStatus -or $status -eq $Expect) {
        Write-Host "  + [$status] $Label" -ForegroundColor Green
        $script:Pass++
    } else {
        Write-Host "  x [$status] $Label  (expected $Expect)" -ForegroundColor Red
        $script:Fail++
    }
}

function Post {
    param(
        [string]$Label,
        [string]$Url,
        [string]$Body,
        [switch]$AnyStatus
    )
    $script:Total++
    try {
        $resp = Invoke-WebRequest -Uri $Url -WebSession $session -Method POST -Body $Body `
            -ContentType "application/x-www-form-urlencoded" -MaximumRedirection 5 -TimeoutSec 30 `
            -UseBasicParsing -ErrorAction Stop
        $status = $resp.StatusCode
    } catch {
        $status = [int]$_.Exception.Response.StatusCode
        if ($status -eq 0) { $status = 999 }
    }

    if ($AnyStatus -or $status -eq 200) {
        Write-Host "  + [$status] $Label" -ForegroundColor Green
        $script:Pass++
    } else {
        Write-Host "  x [$status] $Label" -ForegroundColor Red
        $script:Fail++
    }
}

function PostJson {
    param(
        [string]$Label,
        [string]$Url,
        [string]$Body,
        [int]$Expect = 200,
        [switch]$AnyStatus
    )
    $script:Total++
    try {
        $resp = Invoke-WebRequest -Uri $Url -WebSession $session -Method POST -Body $Body `
            -ContentType "application/json" -MaximumRedirection 5 -TimeoutSec 30 `
            -UseBasicParsing -ErrorAction Stop
        $status = $resp.StatusCode
        $result = $resp.Content
    } catch {
        $status = [int]$_.Exception.Response.StatusCode
        if ($status -eq 0) { $status = 999 }
        $result = $_.Exception.Response
    }

    if ($AnyStatus -or $status -eq $Expect) {
        Write-Host "  + [$status] $Label" -ForegroundColor Green
        $script:Pass++
        return $result
    } else {
        Write-Host "  x [$status] $Label  (expected $Expect)" -ForegroundColor Red
        $script:Fail++
        return $null
    }
}

function PutJson {
    param(
        [string]$Label,
        [string]$Url,
        [string]$Body,
        [int]$Expect = 200,
        [switch]$AnyStatus
    )
    $script:Total++
    try {
        $resp = Invoke-WebRequest -Uri $Url -WebSession $session -Method PUT -Body $Body `
            -ContentType "application/json" -MaximumRedirection 5 -TimeoutSec 30 `
            -UseBasicParsing -ErrorAction Stop
        $status = $resp.StatusCode
        $result = $resp.Content
    } catch {
        $status = [int]$_.Exception.Response.StatusCode
        if ($status -eq 0) { $status = 999 }
        $result = $_.Exception.Response
    }

    if ($AnyStatus -or $status -eq $Expect) {
        Write-Host "  + [$status] $Label" -ForegroundColor Green
        $script:Pass++
        return $result
    } else {
        Write-Host "  x [$status] $Label  (expected $Expect)" -ForegroundColor Red
        $script:Fail++
        return $null
    }
}

function DeleteReq {
    param(
        [string]$Label,
        [string]$Url,
        [int]$Expect = 200,
        [switch]$AnyStatus
    )
    $script:Total++
    try {
        $resp = Invoke-WebRequest -Uri $Url -WebSession $session -Method DELETE `
            -MaximumRedirection 5 -TimeoutSec 30 -UseBasicParsing -ErrorAction Stop
        $status = $resp.StatusCode
        $result = $resp.Content
    } catch {
        $status = [int]$_.Exception.Response.StatusCode
        if ($status -eq 0) { $status = 999 }
        $result = $_.Exception.Response
    }

    if ($AnyStatus -or $status -eq $Expect) {
        Write-Host "  + [$status] $Label" -ForegroundColor Green
        $script:Pass++
        return $result
    } else {
        Write-Host "  x [$status] $Label  (expected $Expect)" -ForegroundColor Red
        $script:Fail++
        return $null
    }
}

# ── Header ───────────────────────────────────────────────────────────
Write-Host ""
Write-Host "===========================================================" -ForegroundColor Cyan
Write-Host "  TaskManager - Full Endpoint Test Suite" -ForegroundColor Cyan
Write-Host "  Target: $BaseUrl" -ForegroundColor Cyan
Write-Host "===========================================================" -ForegroundColor Cyan

# ── Login ────────────────────────────────────────────────────────────
Write-Host "`n> Authentication" -ForegroundColor Yellow
$loginPage = Invoke-WebRequest -Uri "$BaseUrl/Account/Login" -WebSession $session -UseBasicParsing -TimeoutSec 30
$token = ([regex]'__RequestVerificationToken.*?value="([^"]+)"').Match($loginPage.Content).Groups[1].Value
$loginBody = "__RequestVerificationToken=$([uri]::EscapeDataString($token))&Email=admin@taskmanager.local&Password=Admin@12345"
Post -Label "Login as admin" -Url "$BaseUrl/Account/Login" -Body $loginBody -AnyStatus

# ── Core Pages ───────────────────────────────────────────────────────
Write-Host "`n> Core Pages" -ForegroundColor Yellow
Hit -Label "Home page"          -Url "$BaseUrl/"
Hit -Label "Home/Index"         -Url "$BaseUrl/Home/Index"
Hit -Label "Home/About"         -Url "$BaseUrl/Home/About"
Hit -Label "Dashboard"          -Url "$BaseUrl/Dashboard"
Hit -Label "Tasks list"         -Url "$BaseUrl/Tasks"
Hit -Label "Tasks/Create form"  -Url "$BaseUrl/Tasks/Create"
Hit -Label "Users list"         -Url "$BaseUrl/Users"

# ── REST API ─────────────────────────────────────────────────────────
Write-Host "`n> REST API" -ForegroundColor Yellow
Hit -Label "GET /api/tasks"                      -Url "$BaseUrl/api/tasks"
Hit -Label "GET /api/users"                      -Url "$BaseUrl/api/users"
Hit -Label "GET /api/external/weather"           -Url "$BaseUrl/api/external/weather"
Hit -Label "GET /api/external/github"            -Url "$BaseUrl/api/external/github/dotnet/runtime"

# ── Workflow API ─────────────────────────────────────────────────────
Write-Host "`n> Workflow API" -ForegroundColor Yellow
$createTaskBody = '{"Title":"Automated Test Task","Description":"Created by testing script","Priority":2,"AssignedToId":3,"ProjectId":1,"DueDate":"2026-06-20","Tag":"test","EstimatedHours":4,"Labels":["test-label-1","test-label-2"],"WatcherIds":[2,3]}'
$createdTask = PostJson -Label "Workflow - Create Full Task" -Url "$BaseUrl/api/workflow/create-full-task" -Body $createTaskBody -Expect 200

$taskId = 1
if ($createdTask) {
    try {
        $taskObj = ConvertFrom-Json $createdTask
        if ($taskObj -is [Array]) {
            $taskId = $taskObj[0].Id
        } elseif ($taskObj.Rows) {
            $taskId = $taskObj.Rows[0].Id
        } else {
            $taskId = $taskObj.Id
        }
    } catch {
        Write-Host "  ! Failed to parse Task ID from response, using default 1." -ForegroundColor Yellow
    }
}
Write-Host "  Using Task ID: $taskId" -ForegroundColor Gray

PostJson -Label "Workflow - Close Task" -Url "$BaseUrl/api/workflow/$taskId/close" -Body "{}" -Expect 200
PostJson -Label "Workflow - Reopen Task" -Url "$BaseUrl/api/workflow/$taskId/reopen" -Body "{}" -Expect 200
PostJson -Label "Workflow - Assign Task" -Url "$BaseUrl/api/workflow/$taskId/assign/4" -Body "{}" -Expect 200
PostJson -Label "Workflow - Reassign Task" -Url "$BaseUrl/api/workflow/$taskId/reassign/5" -Body "{}" -Expect 200
PostJson -Label "Workflow - Change Priority" -Url "$BaseUrl/api/workflow/$taskId/change-priority" -Body '{"Priority":3}' -Expect 200
PostJson -Label "Workflow - Change Status" -Url "$BaseUrl/api/workflow/$taskId/change-status" -Body '{"Status":1}' -Expect 200
PostJson -Label "Workflow - Change Due Date" -Url "$BaseUrl/api/workflow/$taskId/change-due-date" -Body '{"DueDate":"2026-07-01"}' -Expect 200
PostJson -Label "Workflow - Bulk Update Tasks" -Url "$BaseUrl/api/workflow/bulk-update" -Body '{"TaskIds":[' + $taskId + '],"Status":2,"Priority":1}' -Expect 200

# ── Dashboard API ────────────────────────────────────────────────────
Write-Host "`n> Dashboard API" -ForegroundColor Yellow
Hit -Label "Dashboard Summary" -Url "$BaseUrl/api/dashboard/summary"
Hit -Label "Open count"        -Url "$BaseUrl/api/dashboard/open-count"
Hit -Label "Closed count"      -Url "$BaseUrl/api/dashboard/closed-count"
Hit -Label "Overdue count"     -Url "$BaseUrl/api/dashboard/overdue-count"
Hit -Label "High priority"     -Url "$BaseUrl/api/dashboard/high-priority-count"
Hit -Label "By status"         -Url "$BaseUrl/api/dashboard/by-status"
Hit -Label "By user"           -Url "$BaseUrl/api/dashboard/by-user"
Hit -Label "Recent activities" -Url "$BaseUrl/api/dashboard/recent-activities"
Hit -Label "Recent comments"   -Url "$BaseUrl/api/dashboard/recent-comments"

# ── Reports API ──────────────────────────────────────────────────────
Write-Host "`n> Reports API" -ForegroundColor Yellow
Hit -Label "Daily report"         -Url "$BaseUrl/api/reports/daily"
Hit -Label "Weekly report"        -Url "$BaseUrl/api/reports/weekly"
Hit -Label "Monthly report"       -Url "$BaseUrl/api/reports/monthly"
Hit -Label "User productivity"     -Url "$BaseUrl/api/reports/user-productivity?userId=3"
Hit -Label "Project summary"      -Url "$BaseUrl/api/reports/project-summary?projectId=1"
Hit -Label "Time tracking summary" -Url "$BaseUrl/api/reports/time-tracking?userId=3&taskId=$taskId"
Hit -Label "Comprehensive report"  -Url "$BaseUrl/api/reports/comprehensive"

# ── Search API ───────────────────────────────────────────────────────
Write-Host "`n> Search API" -ForegroundColor Yellow
Hit -Label "Search tasks"     -Url "$BaseUrl/api/search/tasks?keyword=Automated"
Hit -Label "Filter by status" -Url "$BaseUrl/api/search/by-status/1"
Hit -Label "Filter by priority" -Url "$BaseUrl/api/search/by-priority/1"
Hit -Label "Filter by assignee" -Url "$BaseUrl/api/search/by-assignee/3"
Hit -Label "Filter by project" -Url "$BaseUrl/api/search/by-project/1"
Hit -Label "Filter by date"   -Url "$BaseUrl/api/search/by-date-range?from=2026-01-01&to=2026-12-31"
Hit -Label "Advanced search"  -Url "$BaseUrl/api/search/advanced?status=1&priority=1&projectId=1"

# ── Labels API ───────────────────────────────────────────────────────
Write-Host "`n> Labels API" -ForegroundColor Yellow
Hit -Label "Get Labels" -Url "$BaseUrl/api/tasks/$taskId/labels"
PostJson -Label "Add Label" -Url "$BaseUrl/api/tasks/$taskId/labels" -Body '{"Label":"HotFix"}' -Expect 200
DeleteReq -Label "Remove Label" -Url "$BaseUrl/api/tasks/$taskId/labels/HotFix" -Expect 200

# ── Watchers API ─────────────────────────────────────────────────────
Write-Host "`n> Watchers API" -ForegroundColor Yellow
Hit -Label "Get Watchers" -Url "$BaseUrl/api/tasks/$taskId/watchers"
PostJson -Label "Add Watcher" -Url "$BaseUrl/api/tasks/$taskId/watchers" -Body '{"EmployeeId":3}' -Expect 200
DeleteReq -Label "Remove Watcher" -Url "$BaseUrl/api/tasks/$taskId/watchers/3" -Expect 200

# ── Dependencies API ─────────────────────────────────────────────────
Write-Host "`n> Dependencies API" -ForegroundColor Yellow
Hit -Label "Get Dependencies" -Url "$BaseUrl/api/tasks/$taskId/dependencies"
PostJson -Label "Add Dependency" -Url "$BaseUrl/api/tasks/$taskId/dependencies" -Body '{"DependsOnTaskId":2}' -Expect 200
DeleteReq -Label "Remove Dependency" -Url "$BaseUrl/api/tasks/$taskId/dependencies/2" -Expect 200

# ── TimeLogs API ─────────────────────────────────────────────────────
Write-Host "`n> TimeLogs API" -ForegroundColor Yellow
Hit -Label "Get TimeLogs" -Url "$BaseUrl/api/tasks/$taskId/timelogs"
$startedLog = PostJson -Label "Start TimeLog" -Url "$BaseUrl/api/tasks/$taskId/timelogs/start" -Body '{"Description":"Starting work"}' -Expect 200
$logId = 1
if ($startedLog) {
    try {
        $logObj = ConvertFrom-Json $startedLog
        $logId = $logObj.Id
    } catch {}
}
PostJson -Label "Stop TimeLog" -Url "$BaseUrl/api/tasks/$taskId/timelogs/$logId/stop" -Body '{"DurationMinutes":60}' -Expect 200

# ── Comments API ─────────────────────────────────────────────────────
Write-Host "`n> Comments API" -ForegroundColor Yellow
Hit -Label "Get Comments" -Url "$BaseUrl/api/tasks/$taskId/comments"
$startedComment = PostJson -Label "Add Comment" -Url "$BaseUrl/api/tasks/$taskId/comments" -Body '{"Body":"Test Comment"}' -Expect 200
$commentId = 1
if ($startedComment) {
    try {
        $commentObj = ConvertFrom-Json $startedComment
        $commentId = $commentObj.Id
    } catch {}
}
PutJson -Label "Edit Comment" -Url "$BaseUrl/api/tasks/$taskId/comments/$commentId" -Body '{"Body":"Updated Comment"}' -Expect 200
DeleteReq -Label "Delete Comment" -Url "$BaseUrl/api/tasks/$taskId/comments/$commentId" -Expect 200

# ── Attachments API ──────────────────────────────────────────────────
Write-Host "`n> Attachments API" -ForegroundColor Yellow
Hit -Label "Get Attachments" -Url "$BaseUrl/api/tasks/$taskId/attachments"
$startedAttachment = PostJson -Label "Add Attachment" -Url "$BaseUrl/api/tasks/$taskId/attachments" -Body '{"FileName":"design.pdf","ContentType":"application/pdf","SizeBytes":1024}' -Expect 200
$attachmentId = 1
if ($startedAttachment) {
    try {
        $attachmentObj = ConvertFrom-Json $startedAttachment
        $attachmentId = $attachmentObj.Id
    } catch {}
}
DeleteReq -Label "Remove Attachment" -Url "$BaseUrl/api/tasks/$taskId/attachments/$attachmentId" -Expect 200

# ── Diagnostics: Error Routes ────────────────────────────────────────
Write-Host "`n> Diagnostics - Error Routes" -ForegroundColor Yellow
Hit -Label "400 Bad Request"    -Url "$BaseUrl/Diagnostics/Error400"    -Expect 400
Hit -Label "403 Forbidden"      -Url "$BaseUrl/Diagnostics/Error403"    -Expect 403
Hit -Label "404 Not Found"      -Url "$BaseUrl/Diagnostics/Error404"    -Expect 404
Hit -Label "500 Server Error"   -Url "$BaseUrl/Diagnostics/Error500"    -Expect 500

# ── Diagnostics: Failure Rate ────────────────────────────────────────
Write-Host "`n> Diagnostics - Failure-Rate Simulation" -ForegroundColor Yellow
for ($i = 1; $i -le 5; $i++) {
    Hit -Label "RandomFailure 50% (#$i)"  -Url "$BaseUrl/Diagnostics/RandomFailure?failPercent=50"  -AnyStatus
}
Hit -Label "RandomFailure 100%"   -Url "$BaseUrl/Diagnostics/RandomFailure?failPercent=100"  -Expect 500
Hit -Label "RandomFailure 0%"     -Url "$BaseUrl/Diagnostics/RandomFailure?failPercent=0"    -Expect 200

# ── Diagnostics: Exceptions ──────────────────────────────────────────
Write-Host "`n> Diagnostics - Exception Types" -ForegroundColor Yellow
Hit -Label "NullReferenceException"       -Url "$BaseUrl/Diagnostics/NullRef"       -Expect 500
Hit -Label "ArgumentException"            -Url "$BaseUrl/Diagnostics/ArgError"      -Expect 500
Hit -Label "DivideByZeroException"        -Url "$BaseUrl/Diagnostics/DivideByZero"  -Expect 500
Hit -Label "TimeoutException"             -Url "$BaseUrl/Diagnostics/Timeout"       -Expect 500
Hit -Label "UnauthorizedAccessException"  -Url "$BaseUrl/Diagnostics/Unauthorized"  -Expect 500
Hit -Label "Nested Exception"             -Url "$BaseUrl/Diagnostics/NestedError"   -Expect 500

# ── Diagnostics: Slow Responses ──────────────────────────────────────
Write-Host "`n> Diagnostics - Slow Responses" -ForegroundColor Yellow
Hit -Label "SlowResponse 1s"   -Url "$BaseUrl/Diagnostics/SlowResponse?delayMs=1000"
Hit -Label "SlowResponse 3s"   -Url "$BaseUrl/Diagnostics/SlowResponse?delayMs=3000"

# ── Diagnostics: External HTTP Requests ──────────────────────────────
Write-Host "`n> Diagnostics - External HTTP Requests" -ForegroundColor Yellow
Hit -Label "Weather API (open-meteo)"     -Url "$BaseUrl/Diagnostics/ExternalWeather"
Hit -Label "GitHub API (dotnet/runtime)"  -Url "$BaseUrl/Diagnostics/ExternalGitHub"
Hit -Label "JSONPlaceholder"              -Url "$BaseUrl/Diagnostics/ExternalJsonPlaceholder"
Hit -Label "httpbin.org (2s delay)"       -Url "$BaseUrl/Diagnostics/ExternalHttpBin"
Hit -Label "Failing external call"        -Url "$BaseUrl/Diagnostics/ExternalFailing"   -Expect 500

# ── Diagnostics Page (UI) ───────────────────────────────────────────
Write-Host "`n> Diagnostics UI" -ForegroundColor Yellow
Hit -Label "Diagnostics Index page" -Url "$BaseUrl/Diagnostics"

# ── SQL Queries (WebForms) ───────────────────────────────────────────
Write-Host "`n> SQL Queries (WebForms / Raw ADO.NET)" -ForegroundColor Yellow
Hit -Label "SQL Queries Dashboard"  -Url "$BaseUrl/WebForms/SqlQueries.aspx"

# ── Legacy error endpoint ───────────────────────────────────────────
Write-Host "`n> Legacy Error Endpoint" -ForegroundColor Yellow
Hit -Label "Home/Boom (synthetic 500)" -Url "$BaseUrl/Home/Boom" -Expect 500

# ── Summary ──────────────────────────────────────────────────────────
Write-Host ""
Write-Host "===========================================================" -ForegroundColor Cyan
if ($Fail -eq 0) {
    Write-Host "  All $Total tests passed! ($Pass/$Total)" -ForegroundColor Green
} else {
    Write-Host "  $Pass passed, $Fail failed out of $Total total" -ForegroundColor Yellow
}
Write-Host "===========================================================" -ForegroundColor Cyan
Write-Host ""
