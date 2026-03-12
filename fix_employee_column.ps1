$file = 'InventoryManagement\Views\InventoryMovement\Recordgoodsissuedfromthewarehouse.cshtml'
$content = Get-Content $file -Raw -Encoding UTF8

# Add employee name column to header
$content = $content -replace '                                    <th class="p-4 border-b text-center">إجراء</th>', '                                    <th class="p-4 border-b">اسم الموظف</th>`r`n                                    <th class="p-4 border-b text-center">إجراء</th>'

# Add employee name column to table rows
$content = $content -replace '                                            <td class="p-4 text-center">', '                                            <td class="p-4">@tr.EmpName</td>`r`n                                            <td class="p-4 text-center">'

[System.IO.File]::WriteAllText($file, $content, [System.Text.Encoding]::UTF8)
Write-Host "Employee name column added successfully!"
