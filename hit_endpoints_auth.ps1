$baseUrl = "http://localhost:8085"
$loginUrl = "$baseUrl/Account/Login"
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession

Write-Host "1. Fetching Login Page to extract Anti-Forgery Token..."
$loginPage = Invoke-WebRequest -Uri $loginUrl -WebSession $session -UseBasicParsing

# Extract __RequestVerificationToken
$tokenRegex = '<input name="__RequestVerificationToken" type="hidden" value="([^"]+)"'
if ($loginPage.Content -match $tokenRegex) {
    $token = $matches[1]
    Write-Host "[SUCCESS] Extracted Security Token!" -ForegroundColor Green
} else {
    Write-Host "[ERROR] Could not find Anti-Forgery Token!" -ForegroundColor Red
    exit
}

Write-Host "2. Submitting Authentication POST..."
$loginBody = @{
    "__RequestVerificationToken" = $token
    "Email" = "admin@taskmanager.local"
    "Password" = "Admin@12345"
    "RememberMe" = "false"
}

$loginResult = Invoke-WebRequest -Uri $loginUrl -Method Post -Body $loginBody -WebSession $session -UseBasicParsing -MaximumRedirection 0 -ErrorAction SilentlyContinue

if ($loginResult.StatusCode -eq 302) {
    Write-Host "[SUCCESS] Authenticated successfully! Auth Cookie captured." -ForegroundColor Green
} else {
    Write-Host "[ERROR] Login failed! Status: $($loginResult.StatusCode)" -ForegroundColor Red
    exit
}

Write-Host "3. Generating Deep Database APM Traffic..."
$authEndpoints = @(
    # Triggers heavy SELECT statements
    "/Dashboard",
    "/Tasks",
    "/Users",
    # Specific TaskDetails endpoint! (Triggers Task SELECT + Attachments SELECT)
    "/Tasks/Details/1",
    "/Tasks/Details/2"
)

foreach ($endpoint in $authEndpoints) {
    $url = "$baseUrl$endpoint"
    try {
        $response = Invoke-WebRequest -Uri $url -WebSession $session -UseBasicParsing -ErrorAction SilentlyContinue
        if ($response.StatusCode -eq 200) {
            Write-Host "[DB SELECT GENERATED] $($response.StatusCode) - $url" -ForegroundColor Cyan
        }
    } catch {
        Write-Host "[ERROR] $url" -ForegroundColor Red
    }
}

Write-Host "4. Triggering Database UPDATE query..."
# Hit the Edit page to grab the new form token
$editPageUrl = "$baseUrl/Tasks/Edit/1"
$editPage = Invoke-WebRequest -Uri $editPageUrl -WebSession $session -UseBasicParsing -ErrorAction SilentlyContinue

if ($editPage.Content -match $tokenRegex) {
    $editToken = $matches[1]
    $updateBody = @{
        "__RequestVerificationToken" = $editToken
        "Id" = "1"
        "Title" = "Automated APM Update Test"
        "Status" = "InProgress"
        "Priority" = "High"
        "EstimatedHours" = "5"
    }
    
    $updateResult = Invoke-WebRequest -Uri $editPageUrl -Method Post -Body $updateBody -WebSession $session -UseBasicParsing -MaximumRedirection 0 -ErrorAction SilentlyContinue
    if ($updateResult.StatusCode -eq 302) {
        Write-Host "[DB UPDATE GENERATED] Task 1 Modified!" -ForegroundColor Magenta
    }
}

Write-Host "========================================================"
Write-Host "Authenticated traffic generation complete! Check Atatus." -ForegroundColor Green

