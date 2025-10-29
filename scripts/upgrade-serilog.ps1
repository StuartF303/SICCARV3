# Upgrade Serilog packages
$ErrorActionPreference = "Continue"

$packages = @(
    @("Serilog.AspNetCore", "8.0.3"),
    @("Serilog.Enrichers.Environment", "3.0.1"),
    @("Serilog.Expressions", "5.0.0"),
    @("Serilog.Sinks.Console", "6.0.0"),
    @("Serilog.Sinks.Seq", "9.0.0")
)

$csprojFiles = Get-ChildItem -Path "c:\Projects\SICCARV3" -Filter "*.csproj" -Recurse

Write-Host "=" * 50
Write-Host "Serilog Package Upgrades"
Write-Host "=" * 50
Write-Host ""
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
            Write-Host "  [OK] $($csproj.Name)" -ForegroundColor Green
        }
    }

    Write-Host "  Updated in $upgraded projects" -ForegroundColor Green
    Write-Host ""
}

Write-Host "=" * 50
Write-Host "Serilog upgrades complete!"
Write-Host "=" * 50
