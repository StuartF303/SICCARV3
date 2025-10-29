# JsonSchema.Net Upgrade Script
# Upgrades JsonSchema.Net from 5.5.1 to 7.2.2

Write-Host "Starting JsonSchema.Net upgrade to v7.2.2..." -ForegroundColor Green

$packages = @(
    @("JsonSchema.Net", "7.2.2"),
    @("JsonSchema.Net.Generation", "7.2.2")
)

$successCount = 0
$totalOperations = 0

$projectsFound = Get-ChildItem -Path "c:\Projects\SICCARV3\src" -Recurse -Filter "*.csproj" | Where-Object {
    $_.FullName -match "SiccarApplication\.csproj" -or $_.FullName -match "RegisterService\.csproj"
}

foreach ($package in $packages) {
    $name = $package[0]
    $version = $package[1]

    Write-Host "`nUpgrading $name to $version..." -ForegroundColor Cyan
    Write-Host "============================================================"

    foreach ($project in $projectsFound) {
        $totalOperations++
        Write-Host "  Processing: $($project.FullName)" -ForegroundColor Yellow

        try {
            dotnet add $project.FullName package $name --version $version

            if ($LASTEXITCODE -eq 0) {
                $successCount++
                Write-Host "  SUCCESS: $name upgraded" -ForegroundColor Green
            } else {
                Write-Host "  ERROR: Failed to upgrade $name" -ForegroundColor Red
            }
        }
        catch {
            Write-Host "  EXCEPTION: $($_.Exception.Message)" -ForegroundColor Red
        }
    }
}

Write-Host "`n" + ("=" * 60)
Write-Host "Upgrade Summary:" -ForegroundColor Green
Write-Host "  Successful: $successCount / $totalOperations" -ForegroundColor Cyan
Write-Host ("=" * 60)

if ($successCount -eq $totalOperations) {
    Write-Host "`nAll packages upgraded successfully!" -ForegroundColor Green
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "  1. Run: dotnet restore" -ForegroundColor White
    Write-Host "  2. Run: dotnet build" -ForegroundColor White
    Write-Host "  3. Run: dotnet test --filter SchemaDataValidatorTests" -ForegroundColor White
} else {
    Write-Host "`nSome packages failed to upgrade. Please review errors above." -ForegroundColor Red
}
