import React from 'react';
import { TableRow, TableCell, Button, Typography, CircularProgress } from '@mui/material';
import PlayArrowRoundedIcon from '@mui/icons-material/PlayArrowRounded';
import { EstadoFactura } from '../../shared/EstadoFactura';

interface FacturaRowProps {
  factura: any;
  onProcesar: (id: string) => void;
  processing: boolean;
}

const FacturaRow: React.FC<FacturaRowProps> = ({ factura, onProcesar, processing }) => (
  <TableRow sx={{ '&:hover': { bgcolor: '#fcfbf9' }, transition: 'background 0.2s' }}>
    <TableCell>
      <Typography variant="body2" sx={{ fontFamily: 'DM Mono', fontWeight: 500, color: '#c8a84b' }}>
        #{factura.id.slice(0, 8).toUpperCase()}
      </Typography>
    </TableCell>
    <TableCell>
      <Typography variant="body2" sx={{ fontWeight: 600, color: '#1a1814' }}>{factura.clienteId}</Typography>
    </TableCell>
    <TableCell>
      <Typography variant="body2" sx={{ fontWeight: 700 }}>${factura.monto.toLocaleString('es-CL')}</Typography>
    </TableCell>
    <TableCell>
      <Typography variant="body2" sx={{ color: '#7a7670' }}>
        {new Date(factura.fechaEmision).toLocaleDateString('es-ES')}
      </Typography>
    </TableCell>
    <TableCell>
      <EstadoFactura estado={factura.estado} />
    </TableCell>
    <TableCell>
      <Typography variant="body2" sx={{ color: '#7a7670' }}>
        {factura.fechaLimiteDesactivacion
          ? (() => {
              const d = new Date(factura.fechaLimiteDesactivacion);
              const fecha = d.toLocaleDateString('es-ES');
              const hora = d.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit', hour12: true });
              return `${fecha}, ${hora}`;
            })()
          : '—'}
      </Typography>
    </TableCell>
    <TableCell align="right">
      <Button
        variant="contained"
        size="small"
        disableElevation
        onClick={() => onProcesar(factura.id)}
        disabled={processing}
        startIcon={processing ? <CircularProgress size={14} color="inherit" /> : <PlayArrowRoundedIcon />}
        sx={{
          borderRadius: '8px',
          textTransform: 'none',
          fontWeight: 600,
          bgcolor: '#1a1814',
          '&:hover': { bgcolor: '#333' },
          '&.Mui-disabled': { bgcolor: '#e5e2dc' },
          px: 2
        }}
      >
        {processing ? 'Enviando' : 'Procesar'}
      </Button>
    </TableCell>
  </TableRow>
);

export default FacturaRow;