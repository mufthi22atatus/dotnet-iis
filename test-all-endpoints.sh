#!/bin/bash
# ─────────────────────────────────────────────────────────────────────
#  test-all-endpoints.sh  —  Exercises every TaskManager endpoint
#  to generate rich APM telemetry (transactions, errors, external spans).
#
#  Usage:
#    bash test-all-endpoints.sh [BASE_URL]
#
#  Default BASE_URL: http://localhost
# ─────────────────────────────────────────────────────────────────────

BASE="${1:-http://localhost}"
COOKIE_JAR=$(mktemp)
PASS=0
FAIL=0
TOTAL=0

# ── Helpers ──────────────────────────────────────────────────────────
green()  { printf "\033[32m%s\033[0m\n" "$*"; }
red()    { printf "\033[31m%s\033[0m\n" "$*"; }
yellow() { printf "\033[33m%s\033[0m\n" "$*"; }
bold()   { printf "\033[1m%s\033[0m\n" "$*"; }

hit() {
    local label="$1"
    local url="$2"
    local expect="${3:-200}"   # expected HTTP status (use "any" to accept anything)
    TOTAL=$((TOTAL + 1))

    local status
    status=$(curl -s -o /dev/null -w "%{http_code}" -b "$COOKIE_JAR" -c "$COOKIE_JAR" \
             -L --max-time 30 "$url" 2>/dev/null)

    if [ "$expect" = "any" ] || [ "$status" = "$expect" ]; then
        green "  ✓ [$status] $label"
        PASS=$((PASS + 1))
    else
        red   "  ✗ [$status] $label  (expected $expect)"
        FAIL=$((FAIL + 1))
    fi
}

post() {
    local label="$1"
    local url="$2"
    local data="$3"
    local expect="${4:-200}"
    TOTAL=$((TOTAL + 1))

    local status
    status=$(curl -s -o /dev/null -w "%{http_code}" -b "$COOKIE_JAR" -c "$COOKIE_JAR" \
             -L --max-time 30 -X POST -d "$data" \
             -H "Content-Type: application/x-www-form-urlencoded" "$url" 2>/dev/null)

    if [ "$expect" = "any" ] || [ "$status" = "$expect" ]; then
        green "  ✓ [$status] $label"
        PASS=$((PASS + 1))
    else
        red   "  ✗ [$status] $label  (expected $expect)"
        FAIL=$((FAIL + 1))
    fi
}

post_json() {
    local label="$1"
    local url="$2"
    local data="$3"
    local expect="${4:-200}"
    TOTAL=$((TOTAL + 1))

    local resp
    local status
    resp=$(curl -s -w "\n%{http_code}" -b "$COOKIE_JAR" -c "$COOKIE_JAR" \
             -L --max-time 30 -X POST -d "$data" \
             -H "Content-Type: application/json" "$url" 2>/dev/null)
    
    status=$(echo "$resp" | tail -n1)
    local body
    body=$(echo "$resp" | sed '$d')

    if [ "$expect" = "any" ] || [ "$status" = "$expect" ]; then
        green "  ✓ [$status] $label"
        PASS=$((PASS + 1))
        echo "$body"
    else
        red   "  ✗ [$status] $label  (expected $expect)"
        FAIL=$((FAIL + 1))
        echo ""
    fi
}

put_json() {
    local label="$1"
    local url="$2"
    local data="$3"
    local expect="${4:-200}"
    TOTAL=$((TOTAL + 1))

    local resp
    local status
    resp=$(curl -s -w "\n%{http_code}" -b "$COOKIE_JAR" -c "$COOKIE_JAR" \
             -L --max-time 30 -X PUT -d "$data" \
             -H "Content-Type: application/json" "$url" 2>/dev/null)
    
    status=$(echo "$resp" | tail -n1)
    local body
    body=$(echo "$resp" | sed '$d')

    if [ "$expect" = "any" ] || [ "$status" = "$expect" ]; then
        green "  ✓ [$status] $label"
        PASS=$((PASS + 1))
        echo "$body"
    else
        red   "  ✗ [$status] $label  (expected $expect)"
        FAIL=$((FAIL + 1))
        echo ""
    fi
}

delete_req() {
    local label="$1"
    local url="$2"
    local expect="${3:-200}"
    TOTAL=$((TOTAL + 1))

    local resp
    local status
    resp=$(curl -s -w "\n%{http_code}" -b "$COOKIE_JAR" -c "$COOKIE_JAR" \
             -L --max-time 30 -X DELETE "$url" 2>/dev/null)
    
    status=$(echo "$resp" | tail -n1)
    local body
    body=$(echo "$resp" | sed '$d')

    if [ "$expect" = "any" ] || [ "$status" = "$expect" ]; then
        green "  ✓ [$status] $label"
        PASS=$((PASS + 1))
        echo "$body"
    else
        red   "  ✗ [$status] $label  (expected $expect)"
        FAIL=$((FAIL + 1))
        echo ""
    fi
}

# ── Login ────────────────────────────────────────────────────────────
bold ""
bold "═══════════════════════════════════════════════════════════"
bold "  TaskManager — Full Endpoint Test Suite"
bold "  Target: $BASE"
bold "═══════════════════════════════════════════════════════════"

bold ""
bold "▸ Authentication"
post "Login as admin" "$BASE/Account/Login" "Email=admin@taskmanager.local&Password=Admin@12345" "any"

# ── Core Pages ───────────────────────────────────────────────────────
bold ""
bold "▸ Core Pages"
hit "Home page"          "$BASE/"
hit "Home/Index"         "$BASE/Home/Index"
hit "Home/About"         "$BASE/Home/About"
hit "Dashboard"          "$BASE/Dashboard"
hit "Tasks list"         "$BASE/Tasks"
hit "Tasks/Create form"  "$BASE/Tasks/Create"
hit "Users list"         "$BASE/Users"

# ── REST API ─────────────────────────────────────────────────────────
bold ""
bold "▸ REST API"
hit "GET /api/tasks"                     "$BASE/api/tasks"
hit "GET /api/users"                     "$BASE/api/users"
hit "GET /api/external/weather"          "$BASE/api/external/weather"
hit "GET /api/external/github"           "$BASE/api/external/github/dotnet/runtime"

# ── Workflow API ─────────────────────────────────────────────────────
bold ""
bold "▸ Workflow API"
createTaskBody='{"Title":"Automated Test Task","Description":"Created by testing script","Priority":2,"AssignedToId":3,"ProjectId":1,"DueDate":"2026-06-20","Tag":"test","EstimatedHours":4,"Labels":["test-label-1","test-label-2"],"WatcherIds":[2,3]}'
createdTask=$(post_json "Workflow - Create Full Task" "$BASE/api/workflow/create-full-task" "$createTaskBody" "200")

taskId=$(echo "$createdTask" | grep -o -E '"Id":[0-9]+' | head -n1 | cut -d: -f2)
if [ -z "$taskId" ]; then
    taskId=$(echo "$createdTask" | grep -o -E '"id":[0-9]+' | head -n1 | cut -d: -f2)
fi
if [ -z "$taskId" ]; then
    taskId=1
fi
echo "  Using Task ID: $taskId"

post_json "Workflow - Close Task" "$BASE/api/workflow/$taskId/close" "{}" "200" > /dev/null
post_json "Workflow - Reopen Task" "$BASE/api/workflow/$taskId/reopen" "{}" "200" > /dev/null
post_json "Workflow - Assign Task" "$BASE/api/workflow/$taskId/assign/4" "{}" "200" > /dev/null
post_json "Workflow - Reassign Task" "$BASE/api/workflow/$taskId/reassign/5" "{}" "200" > /dev/null
post_json "Workflow - Change Priority" "$BASE/api/workflow/$taskId/change-priority" '{"Priority":3}' "200" > /dev/null
post_json "Workflow - Change Status" "$BASE/api/workflow/$taskId/change-status" '{"Status":1}' "200" > /dev/null
post_json "Workflow - Change Due Date" "$BASE/api/workflow/$taskId/change-due-date" '{"DueDate":"2026-07-01"}' "200" > /dev/null
post_json "Workflow - Bulk Update Tasks" "$BASE/api/workflow/bulk-update" "{\"TaskIds\":[$taskId],\"Status\":2,\"Priority\":1}" "200" > /dev/null

# ── Dashboard API ────────────────────────────────────────────────────
bold ""
bold "▸ Dashboard API"
hit "Dashboard Summary" "$BASE/api/dashboard/summary"
hit "Open count"        "$BASE/api/dashboard/open-count"
hit "Closed count"      "$BASE/api/dashboard/closed-count"
hit "Overdue count"     "$BASE/api/dashboard/overdue-count"
hit "High priority"     "$BASE/api/dashboard/high-priority-count"
hit "By status"         "$BASE/api/dashboard/by-status"
hit "By user"           "$BASE/api/dashboard/by-user"
hit "Recent activities" "$BASE/api/dashboard/recent-activities"
hit "Recent comments"   "$BASE/api/dashboard/recent-comments"

# ── Reports API ──────────────────────────────────────────────────────
bold ""
bold "▸ Reports API"
hit "Daily report"         "$BASE/api/reports/daily"
hit "Weekly report"        "$BASE/api/reports/weekly"
hit "Monthly report"       "$BASE/api/reports/monthly"
hit "User productivity"     "$BASE/api/reports/user-productivity?userId=3"
hit "Project summary"      "$BASE/api/reports/project-summary?projectId=1"
hit "Time tracking summary" "$BASE/api/reports/time-tracking?userId=3&taskId=$taskId"
hit "Comprehensive report"  "$BASE/api/reports/comprehensive"

# ── Search API ───────────────────────────────────────────────────────
bold ""
bold "▸ Search API"
hit "Search tasks"     "$BASE/api/search/tasks?keyword=Automated"
hit "Filter by status" "$BASE/api/search/by-status/1"
hit "Filter by priority" "$BASE/api/search/by-priority/1"
hit "Filter by assignee" "$BASE/api/search/by-assignee/3"
hit "Filter by project" "$BASE/api/search/by-project/1"
hit "Filter by date"   "$BASE/api/search/by-date-range?from=2026-01-01&to=2026-12-31"
hit "Advanced search"  "$BASE/api/search/advanced?status=1&priority=1&projectId=1"

# ── Labels API ───────────────────────────────────────────────────────
bold ""
bold "▸ Labels API"
hit "Get Labels" "$BASE/api/tasks/$taskId/labels"
post_json "Add Label" "$BASE/api/tasks/$taskId/labels" '{"Label":"HotFix"}' "200" > /dev/null
delete_req "Remove Label" "$BASE/api/tasks/$taskId/labels/HotFix" "200" > /dev/null

# ── Watchers API ─────────────────────────────────────────────────────
bold ""
bold "▸ Watchers API"
hit "Get Watchers" "$BASE/api/tasks/$taskId/watchers"
post_json "Add Watcher" "$BASE/api/tasks/$taskId/watchers" '{"EmployeeId":3}' "200" > /dev/null
delete_req "Remove Watcher" "$BASE/api/tasks/$taskId/watchers/3" "200" > /dev/null

# ── Dependencies API ─────────────────────────────────────────────────
bold ""
bold "▸ Dependencies API"
hit "Get Dependencies" "$BASE/api/tasks/$taskId/dependencies"
post_json "Add Dependency" "$BASE/api/tasks/$taskId/dependencies" '{"DependsOnTaskId":2}' "200" > /dev/null
delete_req "Remove Dependency" "$BASE/api/tasks/$taskId/dependencies/2" "200" > /dev/null

# ── TimeLogs API ─────────────────────────────────────────────────────
bold ""
bold "▸ TimeLogs API"
hit "Get TimeLogs" "$BASE/api/tasks/$taskId/timelogs"
startedLog=$(post_json "Start TimeLog" "$BASE/api/tasks/$taskId/timelogs/start" '{"Description":"Starting work"}' "200")
logId=$(echo "$startedLog" | grep -o -E '"Id":[0-9]+' | head -n1 | cut -d: -f2)
if [ -z "$logId" ]; then
    logId=$(echo "$startedLog" | grep -o -E '"id":[0-9]+' | head -n1 | cut -d: -f2)
fi
if [ -z "$logId" ]; then
    logId=1
fi
post_json "Stop TimeLog" "$BASE/api/tasks/$taskId/timelogs/$logId/stop" '{"DurationMinutes":60}' "200" > /dev/null

# ── Comments API ─────────────────────────────────────────────────────
bold ""
bold "▸ Comments API"
hit "Get Comments" "$BASE/api/tasks/$taskId/comments"
startedComment=$(post_json "Add Comment" "$BASE/api/tasks/$taskId/comments" '{"Body":"Test Comment"}' "200")
commentId=$(echo "$startedComment" | grep -o -E '"Id":[0-9]+' | head -n1 | cut -d: -f2)
if [ -z "$commentId" ]; then
    commentId=$(echo "$startedComment" | grep -o -E '"id":[0-9]+' | head -n1 | cut -d: -f2)
fi
if [ -z "$commentId" ]; then
    commentId=1
fi
put_json "Edit Comment" "$BASE/api/tasks/$taskId/comments/$commentId" '{"Body":"Updated Comment"}' "200" > /dev/null
delete_req "Delete Comment" "$BASE/api/tasks/$taskId/comments/$commentId" "200" > /dev/null

# ── Attachments API ──────────────────────────────────────────────────
bold ""
bold "▸ Attachments API"
hit "Get Attachments" "$BASE/api/tasks/$taskId/attachments"
startedAttachment=$(post_json "Add Attachment" "$BASE/api/tasks/$taskId/attachments" '{"FileName":"design.pdf","ContentType":"application/pdf","SizeBytes":1024}' "200")
attachmentId=$(echo "$startedAttachment" | grep -o -E '"Id":[0-9]+' | head -n1 | cut -d: -f2)
if [ -z "$attachmentId" ]; then
    attachmentId=$(echo "$startedAttachment" | grep -o -E '"id":[0-9]+' | head -n1 | cut -d: -f2)
fi
if [ -z "$attachmentId" ]; then
    attachmentId=1
fi
delete_req "Remove Attachment" "$BASE/api/tasks/$taskId/attachments/$attachmentId" "200" > /dev/null

# ── Diagnostics: Error Routes ────────────────────────────────────────
bold ""
bold "▸ Diagnostics — Error Routes"
hit "400 Bad Request"    "$BASE/Diagnostics/Error400"    "400"
hit "403 Forbidden"      "$BASE/Diagnostics/Error403"    "403"
hit "404 Not Found"      "$BASE/Diagnostics/Error404"    "404"
hit "500 Server Error"   "$BASE/Diagnostics/Error500"    "500"

# ── Diagnostics: Failure Rate ────────────────────────────────────────
bold ""
bold "▸ Diagnostics — Failure-Rate Simulation"
for i in 1 2 3 4 5; do
    hit "RandomFailure 50% (#$i)"  "$BASE/Diagnostics/RandomFailure?failPercent=50"  "any"
done
hit "RandomFailure 100%"           "$BASE/Diagnostics/RandomFailure?failPercent=100"  "500"
hit "RandomFailure 0%"             "$BASE/Diagnostics/RandomFailure?failPercent=0"    "200"

# ── Diagnostics: Exceptions ──────────────────────────────────────────
bold ""
bold "▸ Diagnostics — Exception Types"
hit "NullReferenceException"       "$BASE/Diagnostics/NullRef"       "500"
hit "ArgumentException"            "$BASE/Diagnostics/ArgError"      "500"
hit "DivideByZeroException"        "$BASE/Diagnostics/DivideByZero"  "500"
hit "TimeoutException"             "$BASE/Diagnostics/Timeout"       "500"
hit "UnauthorizedAccessException"  "$BASE/Diagnostics/Unauthorized"  "500"
hit "Nested Exception"             "$BASE/Diagnostics/NestedError"   "500"

# ── Diagnostics: Slow Responses ──────────────────────────────────────
bold ""
bold "▸ Diagnostics — Slow Responses"
hit "SlowResponse 1s"   "$BASE/Diagnostics/SlowResponse?delayMs=1000"
hit "SlowResponse 3s"   "$BASE/Diagnostics/SlowResponse?delayMs=3000"

# ── Diagnostics: External HTTP Requests ──────────────────────────────
bold ""
bold "▸ Diagnostics — External HTTP Requests"
hit "Weather API (open-meteo)"    "$BASE/Diagnostics/ExternalWeather"
hit "GitHub API (dotnet/runtime)" "$BASE/Diagnostics/ExternalGitHub"
hit "JSONPlaceholder"             "$BASE/Diagnostics/ExternalJsonPlaceholder"
hit "httpbin.org (2s delay)"      "$BASE/Diagnostics/ExternalHttpBin"
hit "Failing external call"       "$BASE/Diagnostics/ExternalFailing"  "500"

# ── Diagnostics Page (UI) ───────────────────────────────────────────
bold ""
bold "▸ Diagnostics UI"
hit "Diagnostics Index page" "$BASE/Diagnostics"

# ── SQL Queries (WebForms) ───────────────────────────────────────────
bold ""
bold "▸ SQL Queries (WebForms / Raw ADO.NET)"
hit "SQL Queries Dashboard" "$BASE/WebForms/SqlQueries.aspx"

# ── Legacy error endpoint ───────────────────────────────────────────
bold ""
bold "▸ Legacy Error Endpoint"
hit "Home/Boom (synthetic 500)" "$BASE/Home/Boom" "500"

# ── Summary ──────────────────────────────────────────────────────────
bold ""
bold "═══════════════════════════════════════════════════════════"
if [ $FAIL -eq 0 ]; then
    green "  All $TOTAL tests passed! ($PASS/$TOTAL)"
else
    yellow "  $PASS passed, $FAIL failed out of $TOTAL total"
fi
bold "═══════════════════════════════════════════════════════════"
bold ""

# Cleanup
rm -f "$COOKIE_JAR"
exit $FAIL
