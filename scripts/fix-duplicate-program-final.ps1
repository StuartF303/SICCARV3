# Fix duplicate Program entry point by adding explicit compile exclusion
Write-Host "Adding Program.cs compile exclusion to service projects..." -ForegroundColor Cyan

$projects = @(
    "src/Services/Action/ActionService/ActionService.csproj",
    "src/Services/Wallet/WalletService/WalletService.csproj",
    "src/Services/Register/RegisterService/RegisterService.csproj",
    "src/Services/Validator/ValidatorService/ValidatorService.csproj",
    "src/Services/Blueprint/BlueprintService/BlueprintService.csproj",
    "src/Services/Peer/PeerService/PeerService.csproj"
)

$successCount = 0
$failCount = 0

foreach ($project in $projects) {
    $fullPath = Join-Path (Join-Path $PSScriptRoot "..") $project
    if (Test-Path $fullPath) {
        Write-Host "Processing: $project" -ForegroundColor Yellow

        try {
            $content = Get-Content $fullPath -Raw

            # Check if already has the fix
            if ($content -match '<EnableDefaultCompileItems>') {
                Write-Host "  Already has EnableDefaultCompileItems" -ForegroundColor Gray
                $successCount++
                continue
            }

            # Add EnableDefaultCompileItems set to true after ImplicitUsings
            if ($content -match '(<ImplicitUsings>disable</ImplicitUsings>)') {
                $newContent = $content -replace '(<ImplicitUsings>disable</ImplicitUsings>)', "`$1`r`n`t`t<EnableDefaultCompileItems>true</EnableDefaultCompileItems>"
                Set-Content -Path $fullPath -Value $newContent -NoNewline
                Write-Host "  Success" -ForegroundColor Green
                $successCount++
            } else {
                Write-Host "  Failed - Could not find ImplicitUsings tag" -ForegroundColor Red
                $failCount++
            }
        } catch {
            Write-Host "  Error: $_" -ForegroundColor Red
            $failCount++
        }
    } else {
        Write-Host "  File not found: $fullPath" -ForegroundColor Magenta
        $failCount++
    }
}

Write-Host ""
Write-Host "Fix Summary:" -ForegroundColor Cyan
Write-Host "  Successful: $successCount" -ForegroundColor Green
Write-Host "  Failed: $failCount" -ForegroundColor Red
