$content = Get-Content 'InventoryManagement\Views\InventoryMovement\Recordgoodsissuedfromthewarehouse.cshtml' -Raw
$oldRow = '                        <td class="p-4">@tr.ItemDesc</td>
                        <td class="p-4">@tr.ItemQnt</td>
                        <td class="p-4 text-slate-400">@tr.TrDate2</td>'
$newRow = '                        <td class="p-4">@tr.ItemDesc</td>
                        <td class="p-4">@tr.ItemQnt</td>
                        <td class="p-4">@tr.EmpName</td>
                        <td class="p-4 text-slate-400">@tr.TrDate2</td>'

$content = $content.Replace($oldRow, $newRow)
Set-Content 'InventoryManagement\Views\InventoryMovement\Recordgoodsissuedfromthewarehouse.cshtml' -Value $content
Write-Host "Employee name column added to table rows!"
