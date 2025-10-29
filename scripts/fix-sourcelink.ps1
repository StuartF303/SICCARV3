# Fix SourceLink - Replace AzureRepos.Git with GitHub
# Changes Microsoft.SourceLink.AzureRepos.Git to Microsoft.SourceLink.GitHub

Write-Host "Fixing SourceLink references from AzureRepos to GitHub..." -ForegroundColor Green

$csprojFiles = Get-ChildItem -Path "c:\Projects\SICCARV3\src" -Recurse -Filter "*.csproj" -File

$modifiedCount = 0

foreach ($file in $csprojFiles) {
    $content = Get-Content -Path $file.FullName -Raw

    if ($content -match "Microsoft\.SourceLink\.AzureRepos\.Git") {
        Write-Host "Processing: $($file.FullName)" -ForegroundColor Yellow

        # Replace the package reference
        $newContent = $content -replace 'Microsoft\.SourceLink\.AzureRepos\.Git', 'Microsoft.SourceLink.GitHub'

        Set-Content -Path $file.FullName -Value $newContent -NoNewline

        $modifiedCount++
        Write-Host "  UPDATED: AzureRepos.Git -> GitHub" -ForegroundColor Green
    }
}

Write-Host "`n" + ("=" * 60)
Write-Host "Summary:" -ForegroundColor Green
Write-Host "  Modified: $modifiedCount files" -ForegroundColor Cyan
Write-Host ("=" * 60)

if ($modifiedCount -gt 0) {
    Write-Host "`nSuccessfully updated SourceLink in $modifiedCount files!" -ForegroundColor Green
} else {
    Write-Host "`nNo SourceLink references found to update." -ForegroundColor Yellow
}
