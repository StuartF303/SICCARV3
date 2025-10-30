# Fix Pomelo.EntityFrameworkCore.MySql version in test projects
Write-Host "Upgrading Pomelo.EntityFrameworkCore.MySql to 9.0.0 in test projects..." -ForegroundColor Cyan

$testProjects = @(
    "src/Services/Wallet/WalletTests/WalletUnitTests.csproj",
    "src/Services/Wallet/WalletService.IntegrationTests/WalletService.IntegrationTests.csproj",
    "src/Services/Validator/ValidatorTests/ValidatorTests.csproj",
    "src/Services/Register/RegisterTests/RegisterTests.csproj",
    "src/Services/Register/RegisterService.IntegrationTests/RegisterService.IntegrationTests.csproj",
    "src/Services/Peer/PeerTests/PeerUnitTests.csproj",
    "src/Services/Peer/PeerService.IntegrationTests/PeerService.IntegrationTests.csproj",
    "src/Services/Blueprint/BlueprintTests/BlueprintUnitTests/BlueprintUnitTests.csproj",
    "src/Services/Blueprint/BlueprintService.IntegrationTests/BlueprintService.IntegrationTests.csproj",
    "src/Services/Action/ActionTests/ActionUnitTests.csproj",
    "src/Services/Action/ActionService.IntegrationTests/ActionService.IntegrationTests.csproj",
    "src/SDK/SiccarApplicationClientTests/SiccarApplicationClientTests.csproj",
    "src/Common/SiccarCommonTests/SiccarCommonTests.csproj",
    "src/Common/SiccarApplicationTests/SiccarApplicationTests.csproj"
)

$successCount = 0
$failCount = 0

foreach ($project in $testProjects) {
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
