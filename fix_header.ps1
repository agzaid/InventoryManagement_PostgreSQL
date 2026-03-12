# Read file as bytes to preserve encoding
$filePath = 'InventoryManagement\Views\InventoryMovement\Recordgoodsissuedfromthewarehouse.cshtml'
$bytes = [System.IO.File]::ReadAllBytes($filePath)
$content = [System.Text.Encoding]::UTF8.GetString($bytes)

Write-Host "Original content length:" $content.Length

# Find and replace the header using byte arrays for Arabic text
$oldPattern = [System.Text.Encoding]::UTF8.GetBytes('                                    <th class="p-4 border-b text-center">إجراء</th>')
$newPattern = [System.Text.Encoding]::UTF8.GetBytes('                                    <th class="p-4 border-b">اسم الموظف</th>' + "`r`n" + '                                    <th class="p-4 border-b text-center">إجراء</th>')

$oldText = [System.Text.Encoding]::UTF8.GetString($oldPattern)
$newText = [System.Text.Encoding]::UTF8.GetString($newPattern)

Write-Host "Looking for pattern:" $oldText
Write-Host "Contains pattern:" $content.Contains($oldText)

if ($content.Contains($oldText)) {
    $content = $content.Replace($oldText, $newText)
    [System.IO.File]::WriteAllText($filePath, $content, [System.Text.Encoding]::UTF8)
    Write-Host "Header replaced successfully!"
} else {
    Write-Host "Pattern not found in content"
}
