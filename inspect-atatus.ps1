$asm = [System.Reflection.Assembly]::LoadFrom("C:\AtatusDotNet\dotnet-framework\dotnet-iis\TaskManager\bin\Atatus.dll")

Write-Host "=== Public types matching Tracer/Span/Transaction/Agent ==="
$asm.GetTypes() | Where-Object { $_.IsPublic } | ForEach-Object { $_.FullName } | Select-String -Pattern "Tracer|Span|Transaction|Agent" | Sort-Object

Write-Host "`n=== ITracer interface methods ==="
$tracer = $asm.GetTypes() | Where-Object { $_.FullName -eq "Atatus.Api.ITracer" }
if ($tracer) {
    $tracer.GetMethods() | ForEach-Object { Write-Host "$($_.ReturnType.Name) $($_.Name)($(($_.GetParameters() | ForEach-Object { "$($_.ParameterType.Name) $($_.Name)" }) -join ', '))" }
} else {
    Write-Host "ITracer not found"
}

Write-Host "`n=== ITransaction interface methods ==="
$txn = $asm.GetTypes() | Where-Object { $_.FullName -eq "Atatus.Api.ITransaction" }
if ($txn) {
    $txn.GetMethods() | ForEach-Object { Write-Host "$($_.ReturnType.Name) $($_.Name)($(($_.GetParameters() | ForEach-Object { "$($_.ParameterType.Name) $($_.Name)" }) -join ', '))" }
} else {
    Write-Host "ITransaction not found"
}

Write-Host "`n=== ISpan interface methods ==="
$span = $asm.GetTypes() | Where-Object { $_.FullName -eq "Atatus.Api.ISpan" }
if ($span) {
    $span.GetMethods() | ForEach-Object { Write-Host "$($_.ReturnType.Name) $($_.Name)($(($_.GetParameters() | ForEach-Object { "$($_.ParameterType.Name) $($_.Name)" }) -join ', '))" }
} else {
    Write-Host "ISpan not found"
}

Write-Host "`n=== Agent class methods ==="
$agent = $asm.GetTypes() | Where-Object { $_.FullName -eq "Atatus.Agent" }
if ($agent) {
    $agent.GetProperties() | ForEach-Object { Write-Host "Property: $($_.PropertyType.Name) $($_.Name)" }
    $agent.GetMethods([System.Reflection.BindingFlags]::Public -bor [System.Reflection.BindingFlags]::Static) | ForEach-Object { Write-Host "Method: $($_.ReturnType.Name) $($_.Name)($(($_.GetParameters() | ForEach-Object { "$($_.ParameterType.Name) $($_.Name)" }) -join ', '))" }
} else {
    Write-Host "Agent class not found"
}
