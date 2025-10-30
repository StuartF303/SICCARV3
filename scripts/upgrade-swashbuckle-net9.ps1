# Upgrade Swashbuckle to .NET 9 Compatible Version

Write-Host "Upgrading Swashbuckle packages to 6.9.0..." -ForegroundColor Green

$packages = @(
    @("Swashbuckle.AspNetCore", "6.9.0"),
    @("Swashbuckle.AspNetCore.Annotations", "6.9.0")
)

$projects = Get-ChildItem -Path "c:\Projects\SICCARV3\src" -Recurse -Filter "*.csproj"

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
