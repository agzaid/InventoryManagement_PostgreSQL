# Read the file with proper encoding detection
$filePath = 'InventoryManagement\Views\InventoryMovement\Recordgoodsissuedfromthewarehouse.cshtml'

# Try different encodings
$encodings = @('UTF8', 'UTF7', 'UTF32', 'Unicode', 'Default')
$content = $null

foreach ($encoding in $encodings) {
    try {
        $content = Get-Content $filePath -Raw -Encoding $encoding
        if ($content -and $content.Contains('الترتيب')) {
            Write-Host "Successfully read file with encoding: $encoding"
            break
        }
    } catch {
        continue
    }
}

if (-not $content) {
    Write-Host "Could not read file properly"
    exit
}

# Make the replacements
Write-Host "Making replacements..."

# Replace header
$oldHeader = '                                    <th class="p-4 border-b text-center">إجراء</th>'
$newHeader = '                                    <th class="p-4 border-b">اسم الموظف</th>' + "`r`n" + '                                    <th class="p-4 border-b text-center">إجراء</th>'
$content = $content -replace [regex]::Escape($oldHeader), $newHeader

# Replace table rows
$oldCell = '                                            <td class="p-4 text-center">'
$newCell = '                                            <td class="p-4">@tr.EmpName</td>' + "`r`n" + '                                            <td class="p-4 text-center">'
$content = $content -replace [regex]::Escape($oldCell), $newCell

# Write back with UTF8 encoding
[System.IO.File]::WriteAllText($filePath, $content, [System.Text.Encoding]::UTF8)
Write-Host "File updated successfully!"
