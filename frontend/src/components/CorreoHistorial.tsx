import React, { useState, useMemo } from 'react';
import Modal from './Modal';
import { Box, Typography, Avatar, Paper, IconButton, TextField, Stack, Collapse } from '@mui/material';
import { FiMail, FiChevronLeft, FiChevronRight, FiTrash2, FiChevronDown, FiChevronUp } from 'react-icons/fi';
import axios from 'axios';
import API_BASE_URL from '../config/api';

function formatFecha(fecha: string) {
  const d = new Date(fecha);
  return d.toLocaleString('es-CL', { year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit' });
}

const CorreoHistorial: React.FC<{ correos: any[] }> = ({ correos }) => {
  const [expandido, setExpandido] = useState<string | null>(null);
  const [filtro, setFiltro] = useState({ destinatario: '', asunto: '', factura: '', dia: new Date() });
  const [borrando, setBorrando] = useState<string | null>(null);
  
  // Estado del modal unificado
  const [modal, setModal] = useState<{ open: boolean; title: string; desc: string; onConfirm?: () => void } | null>(null);

  const correosFiltrados = useMemo(() => {
    const diaStr = filtro.dia.toISOString().slice(0, 10);
    return correos.filter(correo => {
      const correoFecha = new Date(correo.fecha);
      const correoDia = correoFecha.toISOString().slice(0, 10);
      return (
        (!filtro.destinatario || correo.destinatario.toLowerCase().includes(filtro.destinatario.toLowerCase())) &&
        (!filtro.asunto || correo.asunto.toLowerCase().includes(filtro.asunto.toLowerCase())) &&
        (!filtro.factura || (correo.facturaId || correo.FacturaId || '').toLowerCase().includes(filtro.factura.toLowerCase())) &&
        correoDia === diaStr
      );
    });
  }, [correos, filtro]);

  const handleExpand = (id: string) => {
    setExpandido(expandido === id ? null : id);
  };

  const cambiarDia = (delta: number) => {
    setFiltro(f => {
      const d = new Date(f.dia);
      d.setDate(d.getDate() + delta);
      return { ...f, dia: d };
    });
  };

  // Función para cerrar el modal (Esto es lo que pedías para el botón cancelar)
  const cerrarModal = () => setModal(null);

  const handleBorrar = (id: string) => {
    setModal({
      open: true,
      title: `Borrar Correo`,
      desc: `¿Confirmas el borrado del correo? Esta acción no se puede deshacer.`,
      onConfirm: async () => {
        setBorrando(id);
        cerrarModal(); // Cerramos al confirmar
        try {
          await axios.delete(`${API_BASE_URL}/correos/${id}`);
          if (typeof (window as any).onCorreoBorrado === 'function') {
            (window as any).onCorreoBorrado();
          }
        } catch (error) {
          console.error("Error al borrar:", error);
        } finally {
          setBorrando(null);
        }
      }
    });
  };

  return (
    <Box sx={{ p: { xs: 2, md: 4 }, bgcolor: 'transparent' }}>
      <Typography variant="h6" sx={{ fontWeight: 800, mb: 3, color: '#1a1814', letterSpacing: '-0.02em' }}>
        Registro de envíos
      </Typography>

      {/* --- MODAL (Fuera del loop para que funcione correctamente) --- */}
      {modal?.open && (
        <Modal
          open={modal.open}
          title={modal.title}
          description={modal.desc}
          onCancel={cerrarModal} // El botón cancelar ahora limpia el estado y quita la alerta
          onConfirm={modal.onConfirm!}
        />
      )}

      <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ mb: 3, flexWrap: 'wrap' }}>
        <TextField label="Correo destinatario" size="small" value={filtro.destinatario} onChange={e => setFiltro(f => ({ ...f, destinatario: e.target.value }))} sx={{ minWidth: 180 }} />
        <TextField label="Asunto" size="small" value={filtro.asunto} onChange={e => setFiltro(f => ({ ...f, asunto: e.target.value }))} sx={{ minWidth: 140 }} />
        <TextField label="Factura" size="small" value={filtro.factura} onChange={e => setFiltro(f => ({ ...f, factura: e.target.value }))} sx={{ minWidth: 120 }} />
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <IconButton onClick={() => cambiarDia(-1)}><FiChevronLeft size={18} /></IconButton>
          <TextField
            type="date"
            size="small"
            value={filtro.dia.toISOString().slice(0, 10)}
            onChange={e => setFiltro(f => ({ ...f, dia: new Date(e.target.value) }))}
            sx={{ minWidth: 120 }}
          />
          <IconButton onClick={() => cambiarDia(1)}><FiChevronRight size={18} /></IconButton>
        </Box>
      </Stack>

      <Box sx={{ mt: 1 }}>
        {correosFiltrados.length === 0 ? (
          <Paper elevation={0} sx={{ p: 4, borderRadius: '12px', border: '1px solid #e5e2dc', textAlign: 'center', color: '#7a7670', bgcolor: '#fcfbf9' }}>
            Sin actividad para los filtros seleccionados.
          </Paper>
        ) : (
          <Box>
            {correosFiltrados.map((correo) => (
              <Paper key={correo.id} elevation={0} sx={{ mb: 2, borderRadius: '12px', border: '1px solid #e5e2dc', p: 0, overflow: 'hidden', boxShadow: '0 1px 3px rgba(0,0,0,0.03)' }}>
                <Box
                  sx={{
                    display: 'flex', gap: 2, alignItems: 'center', px: 3, py: 2,
                    bgcolor: expandido === correo.id ? '#f2f0ec' : '#fff',
                    cursor: 'pointer',
                    borderBottom: expandido === correo.id ? '1px solid #e5e2dc' : 'none',
                    transition: 'background 0.2s',
                    '&:hover': { bgcolor: '#f2f0ec' }
                  }}
                  onClick={() => handleExpand(correo.id)}
                >
                  <Avatar sx={{ bgcolor: '#f2f0ec', color: '#1a1814', width: 36, height: 36 }}><FiMail size={18} /></Avatar>
                  <Box sx={{ flex: 1 }}>
                    <Typography variant="body2" sx={{ fontWeight: 700 }}>{correo.asunto}</Typography>
                    <Typography variant="caption" sx={{ color: '#7a7670' }}>Para: {correo.destinatario}</Typography>
                  </Box>
                  <Typography variant="caption" sx={{ color: '#7a7670', minWidth: 120, textAlign: 'right' }}>{formatFecha(correo.fecha)}</Typography>
                  
                  <IconButton 
                    size="small" 
                    onClick={e => { e.stopPropagation(); handleBorrar(correo.id); }} 
                    disabled={borrando === correo.id}
                  >
                    <FiTrash2 size={18} />
                  </IconButton>

                  <IconButton size="small" onClick={e => { e.stopPropagation(); handleExpand(correo.id); }}>
                    {expandido === correo.id ? <FiChevronUp size={18} /> : <FiChevronDown size={18} />}
                  </IconButton>
                </Box>
                
                <Collapse in={expandido === correo.id} timeout="auto" unmountOnExit>
                  <Box sx={{ bgcolor: '#f9f9f7', p: 3, borderTop: '1px solid #e5e2dc' }}>
                    <Typography variant="subtitle2" sx={{ mb: 1, fontWeight: 700 }}>Asunto: {correo.asunto}</Typography>
                    <Typography variant="body2" sx={{ mb: 1 }}><b>Para:</b> {correo.destinatario}</Typography>
                    <Typography variant="body2" sx={{ mb: 1 }}><b>Factura:</b> {correo.facturaId || correo.FacturaId || '-'}</Typography>
                    <Typography variant="body2" sx={{ mb: 1 }}><b>Fecha:</b> {formatFecha(correo.fecha)}</Typography>
                    {correo.cuerpo && (
                      <Box sx={{ mt: 2 }}>
                        <Typography variant="body2" sx={{ fontWeight: 700, mb: 0.5 }}>Contenido:</Typography>
                        <Paper variant="outlined" sx={{ p: 2, background: '#fff', whiteSpace: 'pre-line', fontFamily: 'monospace', fontSize: 14 }}>
                          {correo.cuerpo}
                        </Paper>
                      </Box>
                    )}
                  </Box>
                </Collapse>
              </Paper>
            ))}
          </Box>
        )}
      </Box>
    </Box>
  );
};

export default CorreoHistorial;