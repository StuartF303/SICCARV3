# Update TargetFramework to net9.0 - Phase 1
# Excludes TenantService, TenantCore, TenantRepository, and TenantService.IntegrationTests

Write-Host "Updating TargetFramework to net9.0 (Phase 1 - Excluding TenantService projects)..." -ForegroundColor Green

# Projects to exclude (staying on .NET 8)
$excludeProjects = @(
    "TenantService.csproj",
    "TenantCore.csproj",
    "TenantRepository.csproj",
    "TenantService.IntegrationTests.csproj"
)

$projects = Get-ChildItem -Path "c:\Projects\SICCARV3\src" -Recurse -Filter "*.csproj" |
    Where-Object {
        $excluded = $false
        foreach ($exclude in $excludeProjects) {
            if ($_.Name -eq $exclude) {
                $excluded = $true
                break
            }
        }
        -not $excluded
    }

$totalProjects = $projects.Count
$updatedCount = 0
$alreadyNet9Count = 0
$excludedDisplayed = @()

Write-Host "Found $totalProjects projects to update`n" -ForegroundColor Cyan

foreach ($project in $projects) {
    Write-Host "Processing: $($project.Name)" -ForegroundColor Yellow

    try {
        $content = Get-Content -Path $project.FullName -Raw

        # Check if already net9.0
        if ($content -match '<TargetFramework>net9\.0</TargetFramework>') {
            Write-Host "  SKIP: Already targeting net9.0" -ForegroundColor Gray
            $alreadyNet9Count++
            continue
        }

        $modified = $false

        # Update net8.0 to net9.0
        if ($content -match '<TargetFramework>net8\.0</TargetFramework>') {
            $content = $content -replace '<TargetFramework>net8\.0</TargetFramework>', '<TargetFramework>net9.0</TargetFramework>'
            $modified = $true
            Write-Host "  SUCCESS: Updated net8.0 -> net9.0" -ForegroundColor Green
        }

        # Update net7.0 to net9.0 (for IntegrationTests project)
        if ($content -match '<TargetFramework>net7\.0</TargetFramework>') {
            $content = $content -replace '<TargetFramework>net7\.0</TargetFramework>', '<TargetFramework>net9.0</TargetFramework>'
            $modified = $true
            Write-Host "  SUCCESS: Updated net7.0 -> net9.0" -ForegroundColor Green
        }

        if ($modified) {
            Set-Content -Path $project.FullName -Value $content -NoNewline
            $updatedCount++
        }
        else {
            Write-Host "  SKIP: No TargetFramework found or already updated" -ForegroundColor Gray
        }
    }
    catch {
        Write-Host "  ERROR: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Summary:" -ForegroundColor Cyan
Write-Host "  Total projects processed: $totalProjects" -ForegroundColor White
Write-Host "  Updated to net9.0: $updatedCount" -ForegroundColor Green
Write-Host "  Already net9.0: $alreadyNet9Count" -ForegroundColor Gray
Write-Host "  Excluded (staying on .NET 8): $($excludeProjects.Count)" -ForegroundColor Yellow
foreach ($exclude in $excludeProjects) {
    Write-Host "    - $exclude" -ForegroundColor Yellow
}
Write-Host "========================================`n" -ForegroundColor Cyan
