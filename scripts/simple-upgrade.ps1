# Simple upgrade script for Phase 1 packages
$ErrorActionPreference = "Continue"

$packages = @(
    @("Microsoft.AspNetCore.Authentication.JwtBearer", "8.0.11"),
    @("Microsoft.AspNetCore.Authentication.OpenIdConnect", "8.0.11"),
    @("Microsoft.EntityFrameworkCore", "8.0.11"),
    @("Microsoft.EntityFrameworkCore.Design", "8.0.11"),
    @("Microsoft.EntityFrameworkCore.Tools", "8.0.11"),
    @("Pomelo.EntityFrameworkCore.MySql", "8.0.2"),
    @("Microsoft.Extensions.Http.Polly", "8.0.11"),
    @("Dapr.AspNetCore", "1.14.0"),
    @("Dapr.Client", "1.14.0"),
    @("Dapr.Extensions.Configuration", "1.14.0"),
    @("Azure.Identity", "1.11.4"),
    @("Swashbuckle.AspNetCore", "6.8.1"),
    @("AspNetCore.HealthChecks.Dapr", "8.0.2"),
    @("AspNetCore.HealthChecks.MySql", "8.0.1"),
    @("AspNetCore.HealthChecks.MongoDb", "8.1.0"),
    @("AspNetCore.HealthChecks.Redis", "8.0.1"),
    @("AspNetCore.HealthChecks.UI", "8.0.2"),
    @("AspNetCore.HealthChecks.UI.Client", "8.0.1"),
    @("AspNetCore.HealthChecks.UI.InMemory.Storage", "8.0.1"),
    @("FluentValidation", "11.10.0"),
    @("FluentValidation.AspNetCore", "11.3.0"),
    @("FluentValidation.DependencyInjectionExtensions", "11.10.0"),
    @("xunit", "2.9.2"),
    @("xunit.runner.visualstudio", "2.8.2"),
    @("coverlet.collector", "6.0.2"),
    @("Microsoft.NET.Test.Sdk", "17.11.1")
)

$csprojFiles = Get-ChildItem -Path "c:\Projects\SICCARV3" -Filter "*.csproj" -Recurse

Write-Host "Found $($csprojFiles.Count) project files"
Write-Host ""

foreach ($package in $packages) {
    $name = $package[0]
    $version = $package[1]

    Write-Host "Upgrading $name to $version..." -ForegroundColor Cyan

    $upgraded = 0
    foreach ($csproj in $csprojFiles) {
        $result = dotnet add $csproj.FullName package $name --version $version 2>&1 | Out-String

        if ($result -match "PackageReference.*updated") {
            $upgraded++
        }
    }

    Write-Host "  Updated in $upgraded projects" -ForegroundColor Green
}

Write-Host ""
Write-Host "Phase 1 upgrades complete!" -ForegroundColor Green
