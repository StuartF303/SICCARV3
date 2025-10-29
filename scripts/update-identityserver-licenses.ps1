# Update IdentityServer4 License Headers Script
# Replaces IdentityServer4 copyright notices with MIT License

Write-Host "Updating IdentityServer4 license headers to MIT License..." -ForegroundColor Green

$newHeader = @"
// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors
"@

$files = Get-ChildItem -Path "c:\Projects\SICCARV3\src\Services\Tenant\TenantService\Identity" -Recurse -Include "*.cs" |
    Where-Object { (Get-Content $_.FullName -Raw) -match "Copyright.*Brock Allen.*Dominick Baier" }

$totalFiles = $files.Count
$processedCount = 0

Write-Host "Found $totalFiles IdentityServer4 files with copyright notices`n" -ForegroundColor Cyan

foreach ($file in $files) {
    $processedCount++
    Write-Host "[$processedCount/$totalFiles] Processing: $($file.Name)" -ForegroundColor Yellow

    try {
        $content = Get-Content -Path $file.FullName -Raw

        # Pattern to match IdentityServer4 copyright (// comments at start of file)
        $pattern = '(?m)^//\s*Copyright.*?Brock Allen.*?\r?\n//\s*Licensed.*?\r?\n'

        if ($content -match $pattern) {
            # Replace the copyright with new MIT header
            $newContent = $content -replace $pattern, "$newHeader`r`n"

            Set-Content -Path $file.FullName -Value $newContent -NoNewline
            Write-Host "  SUCCESS: Updated license header" -ForegroundColor Green
        }
        else {
            Write-Host "  SKIPPED: No matching copyright found" -ForegroundColor Gray
        }
    }
    catch {
        Write-Host "  ERROR: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`nCompleted! Processed $processedCount files." -ForegroundColor Green
