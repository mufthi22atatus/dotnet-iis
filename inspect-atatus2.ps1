$asm = [System.Reflection.Assembly]::LoadFrom("C:\AtatusDotNet\dotnet-framework\dotnet-iis\TaskManager\bin\Atatus.dll")

Write-Host "=== ITransaction interfaces ==="
$txn = $asm.GetTypes() | Where-Object { $_.FullName -eq "Atatus.Api.ITransaction" }
$txn.GetInterfaces() | ForEach-Object { Write-Host $_.FullName }

Write-Host "`n=== IExecutionSegment methods (if exists) ==="
$seg = $asm.GetTypes() | Where-Object { $_.FullName -eq "Atatus.Api.IExecutionSegment" }
if ($seg) {
    $seg.GetMethods() | ForEach-Object { Write-Host "$($_.ReturnType.Name) $($_.Name)($(($_.GetParameters() | ForEach-Object { "$($_.ParameterType.Name) $($_.Name)" }) -join ', '))" }
} else {
    Write-Host "IExecutionSegment not found"
    # Search all interfaces for StartSpan
    Write-Host "`n=== Searching all types for StartSpan ==="
    $asm.GetTypes() | ForEach-Object { 
        $type = $_
        $_.GetMethods() | Where-Object { $_.Name -like "*StartSpan*" -or $_.Name -like "*CaptureSpan*" } | ForEach-Object {
            Write-Host "$($type.FullName).$($_.Name)($(($_.GetParameters() | ForEach-Object { "$($_.ParameterType.Name) $($_.Name)" }) -join ', '))"
        }
    }
}
