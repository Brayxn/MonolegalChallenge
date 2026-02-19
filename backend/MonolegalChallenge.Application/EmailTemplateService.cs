namespace MonolegalChallenge.Application
{
    public static class EmailTemplateService
    {
        public static string BuildBodyPrimerRecordatorio(string nombre, string facturaId, decimal monto)
        {
            return $"Estimado/a {nombre},\n\nEste es el primer recordatorio de pago para su factura {facturaId}.\nMonto pendiente: {monto:C}\n\nPor favor realice el pago a la brevedad o contáctenos si ya lo hizo.\n\nAtentamente,\nEquipo de Cobranzas";
        }

        public static string BuildBodySegundoRecordatorio(string nombre, string facturaId, decimal monto, TimeSpan tiempoEspera, string fechaLimiteDesactivacion = null)
        {
            string tiempoTexto;
            if (tiempoEspera.TotalMinutes < 60)
            {
                tiempoTexto = $"{(int)tiempoEspera.TotalMinutes} minuto{(tiempoEspera.TotalMinutes == 1 ? "" : "s")}";
            }
            else if (tiempoEspera.TotalMinutes % 60 == 0)
            {
                int horas = (int)tiempoEspera.TotalMinutes / 60;
                tiempoTexto = $"{horas} hora{(horas == 1 ? "" : "s")}";
            }
            else
            {
                int horas = (int)tiempoEspera.TotalMinutes / 60;
                int minutos = (int)tiempoEspera.TotalMinutes % 60;
                tiempoTexto = $"{horas} hora{(horas == 1 ? "" : "s")}{(minutos > 0 ? $" y {minutos} minuto{(minutos == 1 ? "" : "s")}" : "")}";
            }
            string fechaTexto = string.IsNullOrEmpty(fechaLimiteDesactivacion) ? "" : $"\nFecha Límite Desactivación: {fechaLimiteDesactivacion}";
            return $"Estimado/a {nombre},\n\nEste es el segundo recordatorio de pago para su factura {facturaId}. Si no realiza el pago de su monto en {tiempoTexto}, se le desactivará su cuenta.{fechaTexto}\nMonto pendiente: {monto:C}\n\nPor favor realice el pago a la brevedad o contáctenos si ya lo hizo.\n\nAtentamente,\nEquipo de Cobranzas";
        }

        public static string BuildBodyDesactivado(string nombre, string facturaId, decimal monto)
        {
            return $"Estimado/a {nombre},\n\nSu factura {facturaId} ha sido desactivada por falta de pago.\nMonto pendiente: {monto:C}\n\nSi ya realizó el pago, por favor contáctenos.\n\nAtentamente,\nEquipo de Cobranzas";
        }
    }
}
