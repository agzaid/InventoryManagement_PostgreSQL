$content = Get-Content 'InventoryManagement\Views\InventoryMovement\Recordgoodsissuedfromthewarehouse.cshtml' -Encoding UTF8 -Raw
Write-Host "File contains 'الترتيب':" $content.Contains('الترتيب')
Write-Host "File contains 'إجراء':" $content.Contains('إجراء')
Write-Host "Looking for table header..."
$content -match "الترتيب.*إجراء" | Out-Null
if($matches) {
    Write-Host "Found table header section"
    Write-Host $matches[0]
} else {
    Write-Host "Table header not found"
}
