$content = Get-Content 'InventoryManagement\Views\InventoryMovement\Recordgoodsissuedfromthewarehouse.cshtml' -Raw -Encoding UTF8
Write-Host "Contains employee column:" $content.Contains('اسم الموظف')
Write-Host "Contains EmpName:" $content.Contains('@tr.EmpName')
