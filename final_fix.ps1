$filePath = 'InventoryManagement\Views\InventoryMovement\Recordgoodsissuedfromthewarehouse.cshtml'
$encoding = New-Object System.Text.UTF8Encoding($true)
$content = [System.IO.File]::ReadAllText($filePath, $encoding)

$oldHeader = '                                    <th class="p-4 border-b text-center">إجراء</th>'
$newHeader = '                                    <th class="p-4 border-b">اسم الموظف</th>' + "`r`n" + '                                    <th class="p-4 border-b text-center">إجراء</th>'

Write-Host "Pattern found:" $content.Contains($oldHeader)

if ($content.Contains($oldHeader)) {
    $content = $content.Replace($oldHeader, $newHeader)
    [System.IO.File]::WriteAllText($filePath, $content, $encoding)
    Write-Host "Header updated successfully!"
} else {
    Write-Host "Header pattern not found"
}
