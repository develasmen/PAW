$ErrorActionPreference = 'Stop'
$api = 'http://localhost:5080'
$web = 'http://localhost:5252'
$createdIds = [Collections.Generic.List[int]]::new()
function Assert($condition, $message) {
    if (!$condition) { throw $message }
    Write-Output "PASS: $message"
}
function Request($method, $path, $body = $null) {
    $parameters = @{ Uri = "$api$path"; Method = $method; SkipHttpErrorCheck = $true; TimeoutSec = 20 }
    if ($null -ne $body) { $parameters.Body = ($body | ConvertTo-Json); $parameters.ContentType = 'application/json' }
    Invoke-WebRequest @parameters
}
function Token($html) {
    $match = [regex]::Match($html, 'name="__RequestVerificationToken"[^>]*value="([^"]+)"')
    if (!$match.Success) { throw 'No se encontró el token del formulario.' }
    [Net.WebUtility]::HtmlDecode($match.Groups[1].Value)
}
try {
    $list = Request GET /tasks
    Assert ($list.StatusCode -eq 200) 'GET lista de tareas existentes'
    $initialTasks = @($list.Content | ConvertFrom-Json)
    Write-Output "Tareas iniciales: $($initialTasks.Count)"
    $label = "PAW API verification $([Guid]::NewGuid().ToString('N'))"
    $result = Request POST /tasks @{name=$label;description='Prueba CRUD';status='Pendiente';dueDate='2026-11-01T12:00:00';modifiedBy='QA'}
    Assert ($result.StatusCode -eq 201) 'POST crea una tarea'
    $task = $result.Content | ConvertFrom-Json
    $createdIds.Add($task.id)
    Assert ($result.Headers.Location -eq "/tasks/$($task.id)") 'POST devuelve Location'
    Assert ($task.createdAt -and $task.lastModified) 'Fechas de auditoría generadas en servidor'
    $result = Request GET "/tasks/$($task.id)"
    Assert (($result.Content | ConvertFrom-Json).name -eq $label) 'GET por ID lee el registro creado'
    $result = Request PUT "/tasks/$($task.id)" @{id=$task.id;name="$label editada";status='Completada';createdAt='2000-01-01';description='Cambio';modifiedBy='QA'}
    $updated = $result.Content | ConvertFrom-Json
    Assert ($result.StatusCode -eq 200 -and $updated.status -eq 'Completada') 'PUT actualiza la tarea'
    Assert ($updated.createdAt -eq $task.createdAt) 'PUT conserva CreatedAt'
    Assert ((Request POST /tasks @{name='   '}).StatusCode -eq 400) 'Nombre vacío devuelve 400'
    Assert ((Request POST /tasks @{name=('x' * 256)}).StatusCode -eq 400) 'Nombre demasiado largo devuelve 400'
    Assert ((Request POST /tasks @{name='Fecha';dueDate='1700-01-01'}).StatusCode -eq 400) 'Fecha fuera del rango SQL devuelve 400'
    Assert ((Request PUT "/tasks/$($task.id)" @{id=-1;name='Invalid'}).StatusCode -eq 400) 'ID inconsistente devuelve 400'
    Assert ((Request GET /tasks/-1).StatusCode -eq 404) 'GET inexistente devuelve 404'
    Assert ((Request PUT /tasks/-1 @{name='Missing'}).StatusCode -eq 404) 'PUT inexistente devuelve 404'
    Assert ((Request DELETE /tasks/-1).StatusCode -eq 404) 'DELETE inexistente devuelve 404'
    $index = Invoke-WebRequest "$web/Task" -SessionVariable session
    Assert ($index.StatusCode -eq 200 -and $index.Content.Contains('exchange-rate-widget')) 'Listado MVC y panel del tipo de cambio'
    if ($initialTasks.Count -gt 0) { Assert ($index.Content.Contains([Net.WebUtility]::HtmlEncode($initialTasks[0].name))) 'MVC carga datos existentes desde Minimal API' }
    $details = Invoke-WebRequest "$web/Task/Details?id=$($task.id)"
    Assert ($details.Content.Contains("$label editada") -and !$details.Content.Contains('v@model')) 'Detalles MVC renderizados'
    $create = Invoke-WebRequest "$web/Task/Create" -WebSession $session
    $mvcLabel = "PAW MVC verification $([Guid]::NewGuid().ToString('N'))"
    $form = @{Name=$mvcLabel;Description='Creada desde MVC';Status='Pendiente';ModifiedBy='QA';__RequestVerificationToken=(Token $create.Content)}
    $saved = Invoke-WebRequest "$web/Task/Create" -Method POST -Body $form -WebSession $session
    $mvcTask = @( (Request GET /tasks).Content | ConvertFrom-Json ) | Where-Object name -eq $mvcLabel
    if ($mvcTask) { $createdIds.Add($mvcTask.id) }
    Assert ($mvcTask -and $saved.Content.Contains($mvcLabel)) 'Formulario MVC crea y muestra tarea'
    $edit = Invoke-WebRequest "$web/Task/Edit?id=$($mvcTask.id)" -WebSession $session
    $form = @{Id=$mvcTask.id;Name="$mvcLabel editada";Description='Cambio desde MVC';Status='Completada';ModifiedBy='QA';__RequestVerificationToken=(Token $edit.Content)}
    $saved = Invoke-WebRequest "$web/Task/Edit" -Method POST -Body $form -WebSession $session
    Assert (((Request GET "/tasks/$($mvcTask.id)").Content | ConvertFrom-Json).status -eq 'Completada') 'Formulario MVC edita usando PUT'
    $index = Invoke-WebRequest "$web/Task" -WebSession $session
    $deleted = Invoke-WebRequest "$web/Task/Delete" -Method POST -Body @{id=$mvcTask.id;__RequestVerificationToken=(Token $index.Content)} -WebSession $session
    Assert ((Request GET "/tasks/$($mvcTask.id)").StatusCode -eq 404) 'Formulario MVC elimina usando DELETE'
    $createdIds.Remove($mvcTask.id) | Out-Null
    $result = Request DELETE "/tasks/$($task.id)"
    Assert ($result.StatusCode -eq 204) 'DELETE devuelve 204'
    $createdIds.Remove($task.id) | Out-Null
    Assert ((Request GET "/tasks/$($task.id)").StatusCode -eq 404) 'Tarea eliminada ya no existe'
    Assert (@((Request GET /tasks).Content | ConvertFrom-Json).Count -eq $initialTasks.Count) 'Registros originales conservados después de las pruebas'
} finally {
    foreach ($id in $createdIds) { Request DELETE "/tasks/$id" | Out-Null }
}
