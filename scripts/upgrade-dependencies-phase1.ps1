# SICCARV3 Dependency Upgrade Script - Phase 1 (Low-Risk)
# This script upgrades packages with minor version changes

param(
    [switch]$DryRun = $false,
    [switch]$Verbose = $false
)

$ErrorActionPreference = "Continue"
$solutionRoot = "C:\Projects\SICCARV3"

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "SICCARV3 Dependency Upgrade - Phase 1" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

if ($DryRun) {
    Write-Host "[DRY RUN MODE] - No changes will be made" -ForegroundColor Yellow
    Write-Host ""
}

# Define packages to upgrade
$packages = @(
    # Microsoft Core Packages
    @{ Name = "Microsoft.AspNetCore.Authentication.JwtBearer"; Version = "8.0.11"; Category = "Microsoft Core" },
    @{ Name = "Microsoft.AspNetCore.Authentication.OpenIdConnect"; Version = "8.0.11"; Category = "Microsoft Core" },
    @{ Name = "Microsoft.EntityFrameworkCore"; Version = "8.0.11"; Category = "Microsoft Core" },
    @{ Name = "Microsoft.EntityFrameworkCore.SqlServer"; Version = "8.0.11"; Category = "Microsoft Core" },
    @{ Name = "Microsoft.EntityFrameworkCore.Design"; Version = "8.0.11"; Category = "Microsoft Core" },
    @{ Name = "Microsoft.Extensions.Http.Polly"; Version = "8.0.11"; Category = "Microsoft Core" },
    @{ Name = "Microsoft.AspNetCore.OData"; Version = "8.2.5"; Category = "Microsoft Core" },

    # Dapr
    @{ Name = "Dapr.AspNetCore"; Version = "1.14.0"; Category = "Dapr" },
    @{ Name = "Dapr.Extensions.Configuration"; Version = "1.14.0"; Category = "Dapr" },

    # Validation
    @{ Name = "FluentValidation"; Version = "11.10.0"; Category = "Validation" },

    # Azure
    @{ Name = "Azure.Identity"; Version = "1.13.1"; Category = "Azure" },
    @{ Name = "Azure.Extensions.AspNetCore.DataProtection.Blobs"; Version = "1.3.4"; Category = "Azure" },
    @{ Name = "Azure.Extensions.AspNetCore.DataProtection.Keys"; Version = "1.2.4"; Category = "Azure" },

    # API/Documentation
    @{ Name = "Swashbuckle.AspNetCore"; Version = "6.9.0"; Category = "API" },
    @{ Name = "Swashbuckle.AspNetCore.Annotations"; Version = "6.9.0"; Category = "API" },
    @{ Name = "Asp.Versioning.Mvc"; Version = "8.1.0"; Category = "API" },
    @{ Name = "Asp.Versioning.Mvc.ApiExplorer"; Version = "8.1.0"; Category = "API" },

    # Health Checks
    @{ Name = "AspNetCore.HealthChecks.AzureKeyVault"; Version = "8.0.1"; Category = "Health Checks" },
    @{ Name = "AspNetCore.HealthChecks.MongoDb"; Version = "8.1.0"; Category = "Health Checks" },
    @{ Name = "AspNetCore.HealthChecks.MySql"; Version = "8.0.1"; Category = "Health Checks" },

    # Other
    @{ Name = "Microsoft.VisualStudio.Azure.Containers.Tools.Targets"; Version = "1.21.0"; Category = "Tools" },
    @{ Name = "JsonLogic"; Version = "4.1.0"; Category = "JSON" },
    @{ Name = "IdentityModel.AspNetCore"; Version = "4.4.0"; Category = "Identity" }
)

# Find all .csproj files
Write-Host "Searching for .csproj files..." -ForegroundColor Cyan
$projectFiles = Get-ChildItem -Path "$solutionRoot\src" -Filter "*.csproj" -Recurse

Write-Host "Found $($projectFiles.Count) project files" -ForegroundColor Green
Write-Host ""

# Track results
$results = @{
    Updated = 0
    Skipped = 0
    Errors = 0
    Projects = @{}
}

# Group packages by category
$categories = $packages | Group-Object -Property Category

foreach ($category in $categories) {
    Write-Host "=== $($category.Name) ===" -ForegroundColor Magenta

    foreach ($pkg in $category.Group) {
        Write-Host "Upgrading $($pkg.Name) to $($pkg.Version)..." -ForegroundColor Yellow

        $updatedProjects = 0

        foreach ($projectFile in $projectFiles) {
            $content = Get-Content $projectFile.FullName -Raw

            # Check if project references this package
            if ($content -match "<PackageReference\s+Include=`"$($pkg.Name)`"") {

                # Extract current version
                if ($content -match "<PackageReference\s+Include=`"$($pkg.Name)`"\s+Version=`"([^`"]+)`"") {
                    $currentVersion = $matches[1]

                    if ($currentVersion -ne $pkg.Version) {
                        Write-Host "  - $($projectFile.Name): $currentVersion → $($pkg.Version)" -ForegroundColor Gray

                        if (-not $DryRun) {
                            try {
                                Push-Location $projectFile.DirectoryName
                                $output = dotnet add package $pkg.Name --version $pkg.Version 2>&1

                                if ($LASTEXITCODE -eq 0) {
                                    $updatedProjects++
                                    $results.Updated++
                                } else {
                                    Write-Host "    ERROR: $output" -ForegroundColor Red
                                    $results.Errors++
                                }

                                Pop-Location
                            } catch {
                                Write-Host "    ERROR: $_" -ForegroundColor Red
                                $results.Errors++
                                Pop-Location
                            }
                        } else {
                            $updatedProjects++
                        }
                    } else {
                        if ($Verbose) {
                            Write-Host "  - $($projectFile.Name): Already at $($pkg.Version)" -ForegroundColor DarkGray
                        }
                        $results.Skipped++
                    }
                }
            }
        }

        if ($updatedProjects -gt 0) {
            Write-Host "  ✓ Updated in $updatedProjects project(s)" -ForegroundColor Green
        } else {
            Write-Host "  ℹ No projects needed updating" -ForegroundColor DarkGray
        }

        Write-Host ""
    }
}

# Summary
Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "SUMMARY" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Packages Updated: $($results.Updated)" -ForegroundColor Green
Write-Host "Packages Skipped (already current): $($results.Skipped)" -ForegroundColor Yellow
Write-Host "Errors: $($results.Errors)" -ForegroundColor $(if ($results.Errors -gt 0) { "Red" } else { "Green" })
Write-Host ""

if ($DryRun) {
    Write-Host "This was a DRY RUN - no actual changes were made" -ForegroundColor Yellow
    Write-Host "Run without -DryRun to apply changes" -ForegroundColor Yellow
} else {
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "1. Run: dotnet restore" -ForegroundColor White
    Write-Host "2. Run: dotnet build" -ForegroundColor White
    Write-Host "3. Run: dotnet test" -ForegroundColor White
    Write-Host "4. Review and commit changes if all tests pass" -ForegroundColor White
}

Write-Host ""
Write-Host "For detailed upgrade plan, see: DEPENDENCY_UPGRADE_PLAN.md" -ForegroundColor Cyan
