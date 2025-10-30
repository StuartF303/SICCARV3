# Upgrade Pomelo.EntityFrameworkCore.MySql to .NET 9 compatible version
Write-Host "Upgrading Pomelo.EntityFrameworkCore.MySql to v9.0.0..." -ForegroundColor Cyan

$projectsWithPomelo = @(
    "src/UI/AdminUI/Server/AdminUI.Server.csproj",
    "src/Services/Register/RegisterCoreMongoDBStorage/RegisterCoreMongoDBStorage.csproj",
    "src/Services/Validator/ValidatorCore/ValidatorCore.csproj",
    "src/Common/SiccarPlatformCryptographyTests/SiccarPlatformCryptographyTests.csproj",
    "src/Services/Register/RegisterCore/RegisterCore.csproj",
    "src/Services/Validator/ValidationEngine/ValidationEngine.csproj",
    "src/Common/SiccarPlatformCryptography/SiccarPlatformCryptography.csproj",
    "src/UI/AdminUI/Client/AdminUI.Client.csproj",
    "src/Services/Wallet/WalletServiceCore/WalletServiceCore.csproj",
    "src/Common/SiccarPlatform/SiccarPlatform.csproj",
    "src/UI/siccarcmd/siccarcmd.csproj",
    "src/Common/SiccarCommon/SiccarCommon.csproj"
)

$successCount = 0
$failCount = 0

foreach ($project in $projectsWithPomelo) {
    $fullPath = Join-Path (Join-Path $PSScriptRoot "..") $project
    if (Test-Path $fullPath) {
        Write-Host "Upgrading Pomelo in: $project" -ForegroundColor Yellow

        try {
            dotnet add $fullPath package Pomelo.EntityFrameworkCore.MySql --version 9.0.0 2>&1 | Out-Null
            if ($LASTEXITCODE -eq 0) {
                Write-Host "  Success" -ForegroundColor Green
                $successCount++
            } else {
                Write-Host "  Failed" -ForegroundColor Red
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
