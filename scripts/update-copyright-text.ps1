# Update Copyright Text Script
# Changes "Copyright (c) 2024 SICCAR Project Contributors" to "Copyleft 2026 Sorcha Project Contributors"

Write-Host "Updating copyright text to Copyleft 2026 Sorcha Project Contributors..." -ForegroundColor Green

$files = Get-ChildItem -Path "c:\Projects\SICCARV3\src" -Recurse -Include "*.cs","*.ts","*.js","*.tsx","*.jsx" |
    Where-Object { (Get-Content $_.FullName -Raw) -match "Copyright \(c\) 2024 SICCAR Project Contributors" }

$totalFiles = $files.Count
$processedCount = 0

Write-Host "Found $totalFiles files with copyright text to update`n" -ForegroundColor Cyan

foreach ($file in $files) {
    $processedCount++
    Write-Host "[$processedCount/$totalFiles] Processing: $($file.Name)" -ForegroundColor Yellow

    try {
        $content = Get-Content -Path $file.FullName -Raw

        # Replace the copyright text
        $newContent = $content -replace "Copyright \(c\) 2024 SICCAR Project Contributors", "Copyleft 2026 Sorcha Project Contributors"

        Set-Content -Path $file.FullName -Value $newContent -NoNewline
        Write-Host "  SUCCESS: Updated copyright text" -ForegroundColor Green
    }
    catch {
        Write-Host "  ERROR: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`nCompleted! Processed $processedCount files." -ForegroundColor Green
