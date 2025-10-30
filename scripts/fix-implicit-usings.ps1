# Fix duplicate Program entry point by disabling implicit usings in all service projects
Write-Host "Disabling implicit usings in service projects..." -ForegroundColor Cyan

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

            # Check if ImplicitUsings already exists
            if ($content -match '<ImplicitUsings>') {
                Write-Host "  Already has ImplicitUsings setting" -ForegroundColor Gray
                $successCount++
                continue
            }

            # Add ImplicitUsings after TargetFramework
            if ($content -match '(<TargetFramework>net9\.0</TargetFramework>)') {
                $newContent = $content -replace '(<TargetFramework>net9\.0</TargetFramework>)', "`$1`r`n`t`t<ImplicitUsings>disable</ImplicitUsings>"
                Set-Content -Path $fullPath -Value $newContent -NoNewline
                Write-Host "  Success - Added ImplicitUsings disable" -ForegroundColor Green
                $successCount++
            } else {
                Write-Host "  Failed - Could not find TargetFramework tag" -ForegroundColor Red
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
