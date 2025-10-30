# Update Dockerfiles to .NET 9

Write-Host "Updating Dockerfiles to .NET 9..." -ForegroundColor Green

$dockerfiles = Get-ChildItem -Path "c:\Projects\SICCARV3\src" -Recurse -Filter "Dockerfile" |
    Where-Object { $_.FullName -notmatch "TenantService" }

$updatedCount = 0

foreach ($dockerfile in $dockerfiles) {
    Write-Host "Processing: $($dockerfile.FullName)" -ForegroundColor Yellow

    try {
        $content = Get-Content -Path $dockerfile.FullName -Raw

        $modified = $false

        # Update aspnet base images
        if ($content -match 'FROM mcr\.microsoft\.com/dotnet/aspnet:8\.0') {
            $content = $content -replace 'FROM mcr\.microsoft\.com/dotnet/aspnet:8\.0[^\s]*', 'FROM mcr.microsoft.com/dotnet/aspnet:9.0'
            $modified = $true
        }

        # Update SDK images
        if ($content -match 'FROM mcr\.microsoft\.com/dotnet/sdk:8\.0') {
            $content = $content -replace 'FROM mcr\.microsoft\.com/dotnet/sdk:8\.0[^\s]*', 'FROM mcr.microsoft.com/dotnet/sdk:9.0'
            $modified = $true
        }

        if ($modified) {
            Set-Content -Path $dockerfile.FullName -Value $content -NoNewline
            $updatedCount++
            Write-Host "  SUCCESS: Updated to .NET 9" -ForegroundColor Green
        }
        else {
            Write-Host "  SKIP: No .NET 8 images found" -ForegroundColor Gray
        }
    }
    catch {
        Write-Host "  ERROR: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Summary:" -ForegroundColor Cyan
Write-Host "  Dockerfiles processed: $($dockerfiles.Count)" -ForegroundColor White
Write-Host "  Updated to .NET 9: $updatedCount" -ForegroundColor Green
Write-Host "========================================`n" -ForegroundColor Cyan
