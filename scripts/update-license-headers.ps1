# Update License Headers Script
# Replaces proprietary copyright notices with MIT License

Write-Host "Updating license headers to MIT License..." -ForegroundColor Green

$newHeader = @"
// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors
"@

$files = Get-ChildItem -Path "c:\Projects\SICCARV3\src" -Recurse -Include "*.cs","*.ts","*.js","*.tsx","*.jsx" |
    Where-Object { (Get-Content $_.FullName -Raw) -match "Copyright \(c\)" }

$totalFiles = $files.Count
$processedCount = 0

Write-Host "Found $totalFiles files with copyright notices`n" -ForegroundColor Cyan

foreach ($file in $files) {
    $processedCount++
    Write-Host "[$processedCount/$totalFiles] Processing: $($file.Name)" -ForegroundColor Yellow

    try {
        $content = Get-Content -Path $file.FullName -Raw

        # Pattern to match the entire copyright block (between /* and */ with newlines)
        $pattern = '(?s)/\*\s*\r?\n\*\s*Copyright.*?\*/'

        if ($content -match $pattern) {
            # Replace the copyright block with new MIT header
            $newContent = $content -replace $pattern, $newHeader

            Set-Content -Path $file.FullName -Value $newContent -NoNewline
            Write-Host "  SUCCESS: Updated license header" -ForegroundColor Green
        }
        else {
            Write-Host "  SKIPPED: No matching copyright block found" -ForegroundColor Gray
        }
    }
    catch {
        Write-Host "  ERROR: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`nCompleted! Processed $processedCount files." -ForegroundColor Green
