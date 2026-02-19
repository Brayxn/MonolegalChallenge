# Guía del Usuario — MonolegalChallenge

Este documento explica, desde la perspectiva de un usuario final, cómo usar la aplicación para gestionar facturas y enviar recordatorios. Incluye guía de la interfaz, descripción de botones y flujos comunes.

Contenido
- Introducción breve
- Requisitos y arranque local
- Vista principal y secciones
- Explicación de botones y acciones
- Flujo: procesar recordatorio y ver historial
- Mensajes, errores comunes y soluciones

Introducción breve
------------------
MonolegalChallenge permite gestionar facturas, enviar recordatorios por correo y llevar un historial de comunicaciones. La aplicación está diseñada para ser simple: desde un panel puedes ver métricas, procesar recordatorios (individual o masivo) y revisar o borrar el historial de correos enviados.

Requisitos y arranque local
---------------------------
Estos pasos los realiza un desarrollador o administrador que tenga el código:

- Arrancar backend (.NET):
```powershell
cd backend/MonolegalChallenge.Api
dotnet run
```
- Arrancar frontend (React):
```bash
cd frontend
npm install
npm start
```

Si la aplicación está desplegada, simplemente abre la URL que te proporcionen.

Vista principal y secciones
--------------------------
La pantalla principal (Dashboard) contiene:

- Resumen General: tarjetas con métricas (total facturas, monto total, pendientes, desactivados, número de clientes).
- Pestañas: "Facturas" y "Correos Enviados".
  - Facturas: tabla con facturas y filtros por cliente.
  - Correos Enviados: listado cronológico del historial.
- Barra superior (Topbar): navegación y estado de backend (indica si el API responde).

Explicación de botones y acciones
--------------------------------

- Refrescar: fuerza la recarga de datos desde el backend.
- Selector de cliente: filtra listas por cliente.
- Procesar (por fila): abre un modal de confirmación para procesar el recordatorio de esa factura.
- Procesar Masivo: abre un modal y, al confirmar, procesa todas las facturas pendientes.
- Eliminar (papelera) en historial: borra la entrada del `correo_historial` en la base de datos (no reenvía ni borra correos del proveedor SMTP).

Modal de confirmación
---------------------
Antes de ejecutar acciones que alteran datos (procesar/desactivar/eliminar) aparece un modal con botones **Cancelar** y **Confirmar**. Usa Cancelar para abortar; Confirmar para proceder. También verás toasts informando éxito o error.

Flujo: procesar recordatorio y ver historial
-------------------------------------------
1. Al procesar una factura, el backend genera el contenido del correo (plantilla) y lo envía por SMTP.
2. Si el envío es exitoso, se crea un registro en `correo_historial` con destinatario, asunto, cuerpo y fecha.
3. Puedes ver el registro en la pestaña "Correos Enviados" y eliminar entradas si lo deseas.

Mensajes, errores comunes y soluciones
------------------------------------
- "Backend Desactivado": comprueba que el backend esté en ejecución con `dotnet run`.
- No llegan correos: pide al administrador que verifique las credenciales SMTP y que el backend tenga acceso de salida a Internet. En desarrollo puedes usar herramientas como `smtp4dev` o Mailtrap.

Soporte y contacto
-------------------
Si tienes problemas de uso o encuentras errores, captura el mensaje que aparece (toast o consola del navegador) y pásalo al equipo de soporte/desarrollo.

Fin de la guía de usuario.
