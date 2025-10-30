# Upgrade Microsoft Packages to .NET 9 - Phase 1
# Excludes TenantService projects

Write-Host "Upgrading Microsoft packages to .NET 9..." -ForegroundColor Green

$excludeProjects = @(
    "TenantService.csproj",
    "TenantCore.csproj",
    "TenantRepository.csproj",
    "TenantService.IntegrationTests.csproj"
)

$packages = @(
    @("Microsoft.AspNetCore.Authentication.JwtBearer", "9.0.0"),
    @("Microsoft.AspNetCore.Authentication.OpenIdConnect", "9.0.0"),
    @("Microsoft.AspNetCore.Components.Web", "9.0.0"),
    @("Microsoft.AspNetCore.Components.WebAssembly.Server", "9.0.0"),
    @("Microsoft.AspNetCore.Components.WebAssembly.Authentication", "9.0.0"),
    @("Microsoft.AspNetCore.SignalR.Client", "9.0.0"),
    @("Microsoft.AspNetCore.Http.Connections.Common", "9.0.0"),
    @("Microsoft.AspNetCore.SignalR.Common", "9.0.0"),
    @("Microsoft.AspNetCore.OData", "9.0.0"),
    @("Microsoft.EntityFrameworkCore", "9.0.0"),
    @("Microsoft.EntityFrameworkCore.Design", "9.0.0"),
    @("Microsoft.EntityFrameworkCore.Tools", "9.0.0"),
    @("Microsoft.EntityFrameworkCore.SqlServer", "9.0.0"),
    @("Microsoft.EntityFrameworkCore.InMemory", "9.0.0"),
    @("Microsoft.Extensions.Configuration", "9.0.0"),
    @("Microsoft.Extensions.Configuration.Binder", "9.0.0"),
    @("Microsoft.Extensions.Configuration.Json", "9.0.0"),
    @("Microsoft.Extensions.DependencyInjection", "9.0.0"),
    @("Microsoft.Extensions.Hosting", "9.0.0"),
    @("Microsoft.Extensions.Http.Polly", "9.0.0"),
    @("Microsoft.Extensions.Logging", "9.0.0"),
    @("Microsoft.Extensions.Options", "9.0.0"),
    @("Microsoft.NET.Test.Sdk", "18.0.0")
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

$successCount = 0
$totalOperations = 0

foreach ($package in $packages) {
    $name = $package[0]
    $version = $package[1]

    Write-Host "`nUpgrading $name to $version..." -ForegroundColor Cyan
    Write-Host "============================================================"

    $projectsWithPackage = $projects | Where-Object {
        $content = Get-Content $_.FullName -Raw
        $content -match "<PackageReference Include=`"$name`""
    }

    if ($projectsWithPackage.Count -eq 0) {
        Write-Host "  No projects found using $name" -ForegroundColor Gray
        continue
    }

    Write-Host "  Found $($projectsWithPackage.Count) projects using $name" -ForegroundColor Yellow

    foreach ($project in $projectsWithPackage) {
        $totalOperations++
        Write-Host "    Processing: $($project.Name)" -ForegroundColor Yellow

        try {
            $output = dotnet add $project.FullName package $name --version $version 2>&1

            if ($LASTEXITCODE -eq 0) {
                $successCount++
                Write-Host "      SUCCESS" -ForegroundColor Green
            }
            else {
                Write-Host "      FAILED: $output" -ForegroundColor Red
            }
        }
        catch {
            Write-Host "      EXCEPTION: $($_.Exception.Message)" -ForegroundColor Red
        }
    }
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Summary:" -ForegroundColor Cyan
Write-Host "  Total operations: $totalOperations" -ForegroundColor White
Write-Host "  Successful upgrades: $successCount" -ForegroundColor Green
Write-Host "  Failed: $($totalOperations - $successCount)" -ForegroundColor Red
Write-Host "========================================`n" -ForegroundColor Cyan
