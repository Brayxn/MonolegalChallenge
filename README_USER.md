# Guía del Usuario — MonolegalChallenge

MonolegalChallenge es una aplicación web para gestionar facturas y enviar recordatorios de pago por correo electrónico.

## ¿Cómo funciona?

Desde el panel principal puedes:
- Ver el resumen de facturas (total, pendientes, desactivadas, monto, clientes).
- Filtrar facturas por cliente.
- Procesar recordatorios de pago (individual o masivo).
- Marcar facturas como pagadas (activar).
- Ver el historial de correos enviados.

### Acciones principales
- **Procesar**: envía un recordatorio de pago al cliente de la factura seleccionada y avanza el estado de la factura.
- **Procesar Masivo**: envía recordatorios a todas las facturas pendientes.
- **Activar**: marca una factura como pagada y la pasa al estado "activo" (útil si el cliente pagó fuera del sistema).
- **Refrescar**: actualiza la información en pantalla.

### Estados de la factura
Cada factura puede estar en uno de los siguientes estados:
- **activo**: la factura está pagada y no requiere acción.
- **primerrecordatorio**: se envió el primer recordatorio de pago.
- **segundorecordatorio**: se envió el segundo recordatorio de pago.
- **desactivado**: la factura fue desactivada automáticamente por falta de pago tras los recordatorios.

### Cuenta regresiva y automatización
Cuando una factura está en "primerrecordatorio" o "segundorecordatorio", verás una cuenta regresiva junto a ella:
- Tras el primer recordatorio, la cuenta regresiva indica cuándo se enviará automáticamente el segundo recordatorio.
- Tras el segundo recordatorio, la cuenta regresiva indica cuándo la factura será desactivada automáticamente si no se paga.
No necesitas hacer nada: el sistema envía los recordatorios y desactiva la factura automáticamente cuando corresponde.

### Confirmaciones y mensajes
Antes de realizar acciones importantes, la app te pedirá confirmación. Recibirás mensajes de éxito o error según el resultado.

## ¿Qué necesitas saber?
No es necesario tener conocimientos técnicos. Si la aplicación está desplegada, solo accede a la URL que te proporcionen y usa el panel para gestionar tus facturas y recordatorios.

Si tienes dudas o problemas, consulta con el administrador o soporte. Si ves mensajes de error en la pantalla, compártelos con el equipo de soporte para que puedan ayudarte.

Fin de la guía de usuario.

