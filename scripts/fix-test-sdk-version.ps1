# Fix Microsoft.NET.Test.Sdk version in projects
Write-Host "Upgrading Microsoft.NET.Test.Sdk to 18.0.0..." -ForegroundColor Cyan

$projects = @(
    "src/Services/Register/RegisterTests/RegisterTests.csproj",
    "src/Services/Validator/ValidatorTests/ValidatorTests.csproj",
    "src/Services/Wallet/WalletService.IntegrationTests/WalletService.IntegrationTests.csproj",
    "src/Services/Wallet/WalletTests/WalletUnitTests.csproj",
    "src/UI/AdminUI/Server/AdminUI.Server.csproj",
    "src/UI/siccarcmd/siccarcmd.csproj",
    "src/Services/Wallet/WalletServiceCore/WalletServiceCore.csproj",
    "src/UI/AdminUI/Client/AdminUI.Client.csproj",
    "src/Services/Validator/ValidatorCore/ValidatorCore.csproj",
    "src/Services/Wallet/WalletService/WalletService.csproj",
    "src/UI/AdminUI/AdminUiTest/AdminUiTest.csproj",
    "src/Services/Wallet/WalletSQLRepository/WalletSQLRepository.csproj"
)

$successCount = 0
$failCount = 0

foreach ($project in $projects) {
    $fullPath = Join-Path (Join-Path $PSScriptRoot "..") $project
    if (Test-Path $fullPath) {
        Write-Host "Upgrading: $project" -ForegroundColor Yellow

        try {
            dotnet add $fullPath package Microsoft.NET.Test.Sdk --version 18.0.0 2>&1 | Out-Null
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
