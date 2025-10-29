# Remove ProjectBob NuGet Feeds Script
# Removes all references to projectbob.pkgs.visualstudio.com from nuget.config files

Write-Host "Removing ProjectBob NuGet feeds from all nuget.config files..." -ForegroundColor Green

$configFiles = Get-ChildItem -Path "c:\Projects\SICCARV3" -Recurse -Filter "nuget.config" -File

$modifiedCount = 0
$totalFiles = $configFiles.Count

Write-Host "Found $totalFiles nuget.config files`n" -ForegroundColor Cyan

foreach ($file in $configFiles) {
    Write-Host "Processing: $($file.FullName)" -ForegroundColor Yellow

    $content = Get-Content -Path $file.FullName -Raw

    # Check if file contains projectbob reference
    if ($content -match "projectbob\.pkgs\.visualstudio\.com") {
        # Remove the entire line containing projectbob
        $newContent = $content -replace '\s*<add key="[^"]*" value="https://projectbob\.pkgs\.visualstudio\.com[^"]*"[^>]*/>[\r\n]*', ''

        # Write back to file
        Set-Content -Path $file.FullName -Value $newContent -NoNewline

        $modifiedCount++
        Write-Host "  REMOVED: projectbob feed" -ForegroundColor Green
    } else {
        Write-Host "  SKIP: No projectbob feed found" -ForegroundColor Gray
    }
}

Write-Host "`n" + ("=" * 60)
Write-Host "Summary:" -ForegroundColor Green
Write-Host "  Total files: $totalFiles" -ForegroundColor Cyan
Write-Host "  Modified: $modifiedCount" -ForegroundColor Cyan
Write-Host ("=" * 60)

if ($modifiedCount -gt 0) {
    Write-Host "`nSuccessfully removed ProjectBob feeds from $modifiedCount files!" -ForegroundColor Green
} else {
    Write-Host "`nNo ProjectBob feeds found to remove." -ForegroundColor Yellow
}
