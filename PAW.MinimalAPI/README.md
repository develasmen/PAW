# PAW.MinimalAPI — Tasks

Minimal API de ASP.NET Core (.NET 8, compatible con Visual Studio 2022) basada en el tutorial:
https://learn.microsoft.com/es-mx/aspnet/core/tutorials/min-web-api?view=aspnetcore-10.0&tabs=visual-studio

Se utiliza el modelo `PAW.Models.Task` y su DTO. Los endpoints reciben
`ITasksRepository` por DI/IoC (`AddScoped`). `SqlTasksRepository` accede a la
tabla existente `ProductDb.dbo.Tasks` mediante ADO.NET y SQL parametrizado.
Este proyecto no utiliza Entity Framework ni DbContext. Los registros persisten
en SQL Server y se leen de la misma base utilizada por PAW.API.

## Ejecutar

Desde la carpeta de la solución, en terminales separadas:

```powershell
dotnet run --project PAW.MinimalAPI --launch-profile http
dotnet run --project PAW.Web --launch-profile http
```

Abrir http://localhost:5252/Task. La API escucha en http://localhost:5080.
Si se necesitan las demás pantallas, ejecutar también PAW.API con su perfil HTTPS.
Visual Studio: abrir PAW.sln y seleccionar el perfil de inicio múltiple.

Configurar `ConnectionStrings:Tasks` en appsettings.json de PAW.MinimalAPI
si cambia el servidor o la base de datos. Configurar `TasksApi:BaseUrl` en
PAW.Web si cambia la dirección de la API. No se crean tablas ni se reemplazan datos.

| Método | Ruta | Resultado |
| --- | --- | --- |
| GET | /tasks | 200 con todas las tareas |
| GET | /tasks/{id} | 200 con la tarea o 404 |
| POST | /tasks | 201, tarea creada y cabecera Location |
| PUT | /tasks/{id} | 200, tarea actualizada, o 404 |
| DELETE | /tasks/{id} | 204 o 404 |

POST y PUT reciben un objeto JSON, no una lista. `name` es obligatorio.
Los límites de longitud corresponden al esquema existente. El ID y las fechas
de auditoría los administra el servidor; PUT conserva CreatedAt.
La pantalla utiliza POST para crear, PUT para editar y DELETE para eliminar.
El panel del tipo de cambio conserva el servicio de Hacienda y aparece a la derecha
en escritorio; en pantallas pequeñas se coloca después del contenido.
## Comprobar el CRUD

Con la API y PAW.Web ejecutándose, usar PowerShell 7:

```powershell
pwsh -File PAW.MinimalAPI/Tests/Smoke-Test.ps1
```

La prueba verifica el CRUD por HTTP y desde los formularios MVC, los detalles,
la validación, las respuestas 404, las fechas de auditoría y el panel del tipo
de cambio. Crea tareas temporales y elimina únicamente esas tareas al finalizar.
El archivo PAW.MinimalAPI.http también permite probar las rutas manualmente;
usar en taskId el ID devuelto por POST.