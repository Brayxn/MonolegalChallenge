[![License: Restricted](https://img.shields.io/badge/License-Restricted-red.svg)](PERMISSION.md)

# MonolegalChallenge
Automatización de recordatorios de pago y desactivación de facturas para Monolegal. Incluye backend en ASP.NET Core (C#) con arquitectura en capas, base de datos MongoDB y frontend en React + TypeScript. Permite gestión de clientes, facturas y envíos de correo automáticos.

## Aviso de uso
Este repositorio fue desarrollado exclusivamente como prueba técnica para Monolegal. El código y los recursos incluidos están destinados únicamente a la evaluación técnica de la empresa Monolegal.

No se autoriza su uso comercial ni su despliegue en entornos productivos sin la autorización por escrito del autor. Para permisos y consultas contacte al autor.

Ver `PERMISSION.md` y `LICENSE` para detalles legales.

## Resumen
- Backend: ASP.NET Core (C#) — `backend/`
- Frontend: React + TypeScript — `frontend/`
- Base de datos: MongoDB

## Cómo ejecutar 
Backend:

```powershell
# Desde la raíz del repositorio
cd backend\MonolegalChallenge.Api
dotnet restore
dotnet build
# Ejecutar en el puerto que espera el frontend (por defecto 5045)
dotnet run --urls "http://localhost:5045"
```

Frontend:

```bash
cd frontend
npm install
# Usa el puerto de la API si es necesario: export REACT_APP_API_URL=http://localhost:5045/api
npm start
```
Para más detalles, ver `README_TECHNICAL.md`.
## Licencia
Uso restringido — ver `LICENSE` y `PERMISSION.md`.
