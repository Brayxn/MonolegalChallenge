
// Usar base de datos "monolegal"
use monolegal

// (Opcional) borrar colecciones existentes para evitar duplicados
try {
  db.clientes.drop()
  db.facturas.drop()
  db.correo_historial.drop()
} catch (e) {}

// --- Colección: clientes ---
db.createCollection('clientes')
db.clientes.insertOne({
  _id: "1",
  Nombre: "brayan osorio",
  Email: "cliente1@demo.com"
})

// --- Colección: facturas ---
db.createCollection('facturas')
db.facturas.insertOne({
  _id: "F3",
  ClienteId: "3",
  Monto: { $numberDecimal: "5000" },
  FechaEmision: { $date: "2026-02-19T02:36:50.994Z" },
  Estado: "primerrecordatorio",
  FechaLimiteDesactivacion: null,
  FechaEnvioPrimerRecordatorio: null,
  FechaEnvioSegundoRecordatorio: null,
  FechaPago: null
})

// --- Colección: correo_historial ---
db.createCollection('correo_historial')
db.correo_historial.insertOne({
  _id: "c302758b-aa0a-4477-9784-19c0282d4fa2",
  Destinatario: "cliente1@demo.com",
  Asunto: "Primer recordatorio de pago",
  FacturaId: "F1",
  Cuerpo: "Estimado/a Brayan osorio,\n\nEste es el primer recordatorio de pago para su factura F1.\nMonto pendiente: $ 5.000,00\n\nPor favor realice el pago a la brevedad o contáctenos si ya lo hizo.\n\nAtentamente,\nEquipo de Cobranzas",
  Fecha: ISODate("2026-02-19T21:39:53.400Z")
})

print('mongo.js: seed aplicado (clientes, facturas, correo_historial)')
