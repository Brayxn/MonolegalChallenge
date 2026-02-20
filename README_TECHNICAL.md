# Documentación Técnica — MonolegalChallenge

Este documento está dirigido a desarrolladores/operaciones que necesitan entender la configuración, despliegue, parámetros y estructura técnica del proyecto.

Índice
- Resumen del sistema
- Estructura del repositorio y capas
- Requisitos y dependencias
- Configuración: SMTP, MongoDB, tiempos de automatización
- Uso de `dotnet user-secrets` y generación de contraseña de aplicación (Gmail)
- Endpoints importantes y ejemplos
- Esquema de la base de datos (colecciones y documentos)
- Servicios background y lógica de automatización
- Logs, monitoreo y recomendaciones de seguridad
- Pruebas y CI sugerido
- Herramientas útiles para testing de correo
- Mejoras y notas de despliegue

Resumen del sistema
-------------------
MonolegalChallenge automatiza recordatorios de pago para facturas y su posible desactivación tras un tiempo configurado. Está compuesto por:

- Backend: ASP.NET Core (C#) con arquitectura en capas (Api, Application, Domain, Infrastructure).
- Base de datos: MongoDB (colecciones `facturas`, `clientes`, `correo_historial`).
- Frontend: React + TypeScript (CRA) con componentes de UI y llamadas a la API.

Estructura del repositorio y capas
---------------------------------
- `backend/MonolegalChallenge.Api` — controllers y arranque (`Program.cs`).
- `backend/MonolegalChallenge.Application` — lógica de negocio (`FacturaService`, `CorreoHistorialService`, `EmailTemplateService`, `FacturaAutoDesactivacionService`).
- `backend/MonolegalChallenge.Domain` — modelos y DTOs.
- `backend/MonolegalChallenge.Infrastructure` — repositorios Mongo y `EmailService` (SMTP wrapper), `SmtpSettings`.
- `frontend/` — app React (páginas, componentes, hooks).

Requisitos y dependencias
-------------------------
- .NET 7+ SDK para compilar/ejecutar el backend.
- Node.js 18+ y npm para el frontend.
- MongoDB (puede ser local o remoto).
- Cuenta SMTP válida (Gmail con App Password, o proveedor SMTP con credenciales).

Configuración: SMTP, MongoDB, tiempos de automatización
-------------------------------------------------------
Parámetros más importantes (en `appsettings.json` o variables de entorno):

- `MongoDb:ConnectionString` — URI de MongoDB (e.g. `mongodb://localhost:27017`).
- `MongoDb:Database` — nombre de la base de datos (e.g. `monolegal_db`).
- `TiempoEsperaDesactivacionMin` — minutos que el sistema considera antes de pasar a `desactivado`.
- `IntervaloAutomatizacionMin` — intervalo en minutos con el que corre el servicio background `FacturaAutoDesactivacionService`.
- `SmtpSettings`:
  - `Host` (p.ej. `smtp.gmail.com`).
  - `Port` (p.ej. `587`).
  - `User` (usuario SMTP o correo).
  - `Pass` (contraseña o app password).
  - `From` (dirección remitente que aparece en los correos).

Uso de `dotnet user-secrets` y generación de contraseña de aplicación (Gmail)
----------------------------------------------------------------------------
Recomendado: nunca poner credenciales en `appsettings.json` del repo. Usa `dotnet user-secrets` en desarrollo o variables de entorno en producción.

Inicializar user-secrets (en la carpeta del proyecto API):

```powershell
cd backend/MonolegalChallenge.Api
dotnet user-secrets init
```

Ejemplo para guardar cada setting:

```powershell
# Cambia los valores por tus credenciales
dotnet user-secrets set "SmtpSettings:Host" "smtp.gmail.com"
dotnet user-secrets set "SmtpSettings:Port" "587"
dotnet user-secrets set "SmtpSettings:User" "tu-email@gmail.com"
dotnet user-secrets set "SmtpSettings:Pass" "MI_APP_PASSWORD"
dotnet user-secrets set "SmtpSettings:From" "tu-email@gmail.com"
```

Cómo generar contraseña de aplicación en Gmail (resumen):
1. Activa la verificación en dos pasos en tu cuenta Google.
2. En Seguridad → Contraseñas de aplicaciones, crea una contraseña para "Correo" y "Otro (nombre personalizado)".
3. Copia la contraseña generada y úsalas en `SmtpSettings:Pass`.

> Nota: las contraseñas de aplicación funcionan con cuentas Google que tengan 2FA; si usas cuentas corporativas o proveedores diferentes sigue las recomendaciones del proveedor.

Endpoints importantes y ejemplos
-------------------------------
- `GET /api/facturas` — devuelve la lista de facturas.
- `POST /api/facturas/procesar-recordatorio/{id}` — procesa una factura concreta.
- `POST /api/facturas/procesar-recordatorios` — procesa masivamente.
- `GET /api/correos` — historial de correos.
- `DELETE /api/correos/{id}` — elimina un registro del historial.
- `GET /api/clientes` — lista de clientes.
- `POST /api/seed` — carga datos de ejemplo (SeedController).

Ejemplo: procesar una factura (curl):

```bash
curl -X POST http://localhost:5045/api/facturas/procesar-recordatorio/F1
```

Esquema de la base de datos (colecciones y documentos)
------------------------------------------------------
Colecciones principales:

- `facturas`:
  - `_id` (string): id de factura.
  - `clienteId` (string): id del cliente.
  - `monto` (decimal/number): monto de la factura.
  - `fechaEmision` (Date): fecha de emisión.
  - `estado` (string): `primerrecordatorio` | `segundorecordatorio` | `desactivado`.
  - `fechaLimiteDesactivacion` (Date|null): calculada según `TiempoEsperaDesactivacionMin`.

- `clientes`:
  - `_id` (string)
  - `nombre` (string)
  - `email` (string)

- `correo_historial`:
  - `_id` (guid)
  - `destinatario` (string)
  - `asunto` (string)
  - `cuerpo` (string)
  - `fecha` (Date)
  - `facturaId` (string|null)

Servicios background y lógica de automatización
----------------------------------------------
`FacturaAutoDesactivacionService` es un `BackgroundService` que se ejecuta periódicamente (configurable por `IntervaloAutomatizacionMin`). Su función:

1. Consultar facturas en estados de recordatorio.
2. Calcular si `FechaLimiteDesactivacion` ha pasado: si la fecha límite es anterior a ahora, marcar factura `desactivado`.
3. Guardar evento en `correo_historial` si corresponde y actualizar documento de la factura.

El cálculo usa `TiempoEsperaDesactivacionMin` para establecer el umbral desde el último recordatorio.

Logs, monitoreo y recomendaciones de seguridad
---------------------------------------------
- Usa un logger estructurado (por ejemplo Serilog) para enviar logs a archivos rotativos o a un servicio central (ELK, Azure Monitor).
- Asegura las credenciales: en producción usa un proveedor de secretos (Azure Key Vault, AWS Secrets Manager).
- Habilita HTTPS y políticas CORS restringidas para el API en producción.
- Limita el acceso a MongoDB con credenciales y redes privadas.

Pruebas y CI sugerido
---------------------
- Tests unitarios: `backend/MonolegalChallenge.Tests` (xUnit + Moq). Ejecutar con:

```powershell
cd backend/MonolegalChallenge.Tests
dotnet test
```

- Tests de integración: idealmente usar una instancia controlada de MongoDB (docker-compose o servicio en CI). Si no quieres Docker en local, usar un MongoDB remoto o `Mongo2Go` para pruebas en memoria.

- CI/Workflow mínimo (GitHub Actions):
  - `dotnet restore` + `dotnet build` + `dotnet test`.
  - `npm ci` + `npm run build` para el frontend.

Herramientas útiles para testing de correo
-----------------------------------------
- `smtp4dev` (local): servidor SMTP de desarrollo para ver correos sin enviarlos a Internet.
- Mailtrap: entorno remoto para pruebas de SMTP.
- MailHog: alternativa a smtp4dev.

Mejoras y notas de despliegue
----------------------------
- Para entornos de producción, separar infraestructura: una base de datos gestionada, servidor web frontal y servicio de correo seguro.
- Escalado de envío de correos: si la carga aumenta, usa una cola (RabbitMQ, Azure Service Bus) y workers dedicados.
- Añadir pruebas e2e (Playwright/Cypress) para cubrir flujos completos.

Fin de la documentación técnica. Si quieres, puedo generar ejemplos concretos de GitHub Actions y show-cases para integrar con servicios de monitorización.

- SOLID:
  - `ICorreoHistorialService` y `ICorreoHistorialService` implementados; `CorreoHistorialService` depende ahora de la interfaz.
  - `IEmailTemplateService` + `EmailTemplateService` como implementación inyectable 
  - `IRecordatorioProcessor` extraído y `RecordatorioProcessor` creado; `FacturaService` delega la lógica de recordatorios en este processor (mejora SRP).
  - `FacturaBackgroundService` actualizado para crear scopes con `IServiceScopeFactory` y resolver servicios con alcance dentro del bucle.

## Cómo correr localmente 

- Asegúrate de que MongoDB esté disponible en la conexión configurada (`appsettings.json` por defecto `mongodb://localhost:27017`).
- Arrancar API (desde la raíz):

```powershell
dotnet run --project backend\MonolegalChallenge.Api\MonolegalChallenge.Api.csproj --urls "http://localhost:5045"
```

- Arrancar frontend:

```bash
cd frontend
npm install
npm start
```

## Tests

- Ejecutar unidad tests:

```powershell
dotnet test backend\MonolegalChallenge.Tests\MonolegalChallenge.Tests.csproj
```


