$baseUrl = "http://localhost:8082"

# A massive list of every major endpoint in the application
$endpoints = @(
    # Core Application Routes
    "/",
    "/Home/About",
    "/Home/Ping",
    "/Dashboard",
    "/Tasks",
    "/Tasks/Create",
    "/Users",
    "/WebForms/SqlQueries.aspx",
    
    # Account Routes
    "/Account/Login",
    "/Account/Register",
    
    # Web API Endpoints (Will generate 401/405/200 depending on auth)
    "/api/DashboardApi/GetSummary",
    "/api/DashboardApi/GetRecentActivities",
    "/api/TasksApi",
    "/api/ProjectsApi",
    "/api/TimeLogsApi",
    "/api/AttachmentsApi/GetAttachments?taskId=1",
    "/api/CommentsApi/GetComments?taskId=1",
    "/api/DependenciesApi/GetDependencies?taskId=1",
    "/api/LabelsApi/GetLabels?taskId=1",
    
    # Diagnostics - External HTTP Calls
    "/Diagnostics/ExternalWeather",
    "/Diagnostics/ExternalGitHub",
    "/Diagnostics/ExternalJsonPlaceholder",
    "/Diagnostics/ExternalHttpBin",
    "/Diagnostics/ExternalFailing",
    
    # Diagnostics - Deliberate Errors and Delays
    "/Diagnostics/Error500",
    "/Diagnostics/Error404",
    "/Diagnostics/Error403",
    "/Diagnostics/Error400",
    "/Diagnostics/RandomFailure",
    "/Diagnostics/NullRef",
    "/Diagnostics/ArgError",
    "/Diagnostics/DivideByZero",
    "/Diagnostics/Timeout",
    "/Diagnostics/NestedError",
    "/Diagnostics/SlowResponse"
)

Write-Host "Starting massive endpoint traffic generation against $baseUrl..."
Write-Host "================================================================"

foreach ($endpoint in $endpoints) {
    $url = "$baseUrl$endpoint"
    try {
        # UseBasicParsing ensures PowerShell doesn't try to parse the HTML via IE engine
        $response = Invoke-WebRequest -Uri $url -UseBasicParsing -MaximumRedirection 0 -ErrorAction SilentlyContinue
        
        if ($null -ne $response) {
            Write-Host "[SUCCESS] $($response.StatusCode) - $url" -ForegroundColor Green
        } else {
            Write-Host "[REDIRECT/ERROR] - $url" -ForegroundColor Yellow
        }
    } catch {
        # Catch errors gracefully to continue the loop
        if ($_.Exception.Response) {
            $statusCode = $_.Exception.Response.StatusCode.value__
            Write-Host "[CAUGHT EXCEPTION] $statusCode - $url" -ForegroundColor Red
        } else {
            Write-Host "[FAIL] Unreachable - $url ($($_.Exception.Message))" -ForegroundColor DarkRed
        }
    }
}

Write-Host "================================================================"
Write-Host "Massive traffic generation complete!"
