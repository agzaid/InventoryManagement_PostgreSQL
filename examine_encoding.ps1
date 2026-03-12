$filePath = 'InventoryManagement\Views\InventoryMovement\Recordgoodsissuedfromthewarehouse.cshtml'
$bytes = [System.IO.File]::ReadAllBytes($filePath)

# Try to detect encoding by looking for byte order mark
if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) {
    Write-Host "File has UTF-8 BOM"
    $encoding = [System.Text.Encoding]::UTF8
} elseif ($bytes.Length -ge 2 -and $bytes[0] -eq 0xFF -and $bytes[1] -eq 0xFE) {
    Write-Host "File has UTF-16 LE BOM"
    $encoding = [System.Text.Encoding]::Unicode
} elseif ($bytes.Length -ge 2 -and $bytes[0] -eq 0xFE -and $bytes[1] -eq 0xFF) {
    Write-Host "File has UTF-16 BE BOM"
    $encoding = [System.Text.Encoding]::BigEndianUnicode
} else {
    Write-Host "No BOM detected, trying different encodings..."
    
    # Try different encodings
    $encodings = @(
        [System.Text.Encoding]::UTF8,
        [System.Text.Encoding]::Unicode,
        [System.Text.Encoding]::BigEndianUnicode,
        [System.Text.Encoding]::Default
    )
    
    foreach ($enc in $encodings) {
        try {
            $testContent = $enc.GetString($bytes)
            if ($testContent -match 'الترتيب') {
                Write-Host "Found Arabic text with encoding: $($enc.EncodingName)"
                $encoding = $enc
                break
            }
        } catch {
            continue
        }
    }
}

if ($encoding) {
    $content = $encoding.GetString($bytes)
    Write-Host "Successfully decoded with: $($encoding.EncodingName)"
    
    # Look for the action header
    if ($content -match 'إجراء') {
        Write-Host "Found Arabic 'إجراء' text"
        # Show some context around it
        $lines = $content -split "`r`n"
        for ($i = 0; $i -lt $lines.Count; $i++) {
            if ($lines[$i] -match 'إجراء') {
                Write-Host "Line $($i+1): $($lines[$i].Trim())"
                if ($i -gt 0) { Write-Host "Line $i : $($lines[$i-1].Trim())" }
                if ($i -lt $lines.Count - 1) { Write-Host "Line $($i+2): $($lines[$i+1].Trim())" }
                break
            }
        }
    }
} else {
    Write-Host "Could not determine encoding"
}
