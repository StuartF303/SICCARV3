# Fix remaining Pomelo.EntityFrameworkCore.MySql references to 9.0.0
Write-Host "Upgrading remaining projects to Pomelo 9.0.0..." -ForegroundColor Cyan

$projects = @(
    "src/UI/AdminUI/AdminUiTest/AdminUiTest.csproj",
    "src/Services/Wallet/WalletService/WalletService.csproj",
    "src/Services/Register/RegisterService/RegisterService.csproj",
    "src/Services/Blueprint/BlueprintService/BlueprintService.csproj",
    "src/Services/Action/ActionService/ActionService.csproj",
    "src/Services/Peer/PeerService/PeerService.csproj",
    "src/Services/Peer/PeerUtilities/PeerUtilities.csproj",
    "src/Services/Peer/PeerCore/PeerCore.csproj",
    "src/Common/SiccarApplication/SiccarApplication.csproj"
)

$successCount = 0
$failCount = 0

foreach ($project in $projects) {
    $fullPath = Join-Path (Join-Path $PSScriptRoot "..") $project
    if (Test-Path $fullPath) {
        Write-Host "Upgrading: $project" -ForegroundColor Yellow

        try {
            dotnet add $fullPath package Pomelo.EntityFrameworkCore.MySql --version 9.0.0 2>&1 | Out-Null
            if ($LASTEXITCODE -eq 0) {
                Write-Host "  Success" -ForegroundColor Green
                $successCount++
            } else {
                Write-Host "  Failed (exit code: $LASTEXITCODE)" -ForegroundColor Red
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
Write-Host "Upgrade Summary:" -ForegroundColor Cyan
Write-Host "  Successful: $successCount" -ForegroundColor Green
Write-Host "  Failed: $failCount" -ForegroundColor Red
