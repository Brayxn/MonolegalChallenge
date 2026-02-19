// Componente reutilizable para mostrar el estado de una factura con color
import React from 'react';
import { Chip } from '@mui/material';

interface EstadoFacturaProps {
  estado: string;
}


const colorMap: Record<string, 'success' | 'warning' | 'error' | 'default'> = {
  primerrecordatorio: 'warning',
  segundorecordatorio: 'error',
  desactivado: 'default',
  pagada: 'success',
};


function formatEstado(estado: string) {
  switch (estado) {
    case 'primerrecordatorio': return 'Primer recordatorio';
    case 'segundorecordatorio': return 'Segundo recordatorio';
    case 'desactivado': return 'Desactivado';
    case 'pagada': return 'Pagada';
    default: return estado;
  }
}

export const EstadoFactura: React.FC<EstadoFacturaProps> = ({ estado }) => (
  <Chip label={formatEstado(estado)} color={colorMap[estado] || 'default'} size="small" />
);
