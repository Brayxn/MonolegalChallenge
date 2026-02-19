import React, { useState } from 'react';
import API_BASE_URL from '../../config/api';
import { 
  Box, Table, TableBody, TableCell, TableContainer, 
  TableHead, TableRow, CircularProgress, Alert, Snackbar, Paper 
} from '@mui/material';
import FacturaRow from './FacturaRow';


const FacturasResumen: React.FC<any> = ({ facturas = [], onForceRefresh }) => {
  const [processingId, setProcessingId] = useState<string | null>(null);
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' as 'success'|'error' });

  const handleProcesar = async (id: string) => {
    setProcessingId(id);
    try {
      const res = await fetch(`${API_BASE_URL}/facturas/procesar-recordatorio/${id}`, { method: 'POST' });
      if (!res.ok) throw new Error();
      setSnackbar({ open: true, message: 'Factura procesada', severity: 'success' });
      onForceRefresh?.();
    } catch {
      setSnackbar({ open: true, message: 'Error al procesar', severity: 'error' });
    } finally { setProcessingId(null); }
  };

  return (
    <Box sx={{ width: '100%', overflowX: 'auto', bgcolor: '#fff', borderRadius: '12px', border: '1px solid #e5e2dc', boxShadow: '0 1px 3px rgba(0,0,0,0.03)' }}>
      <TableContainer sx={{ maxHeight: 600, borderRadius: '12px' }}>
        <Table stickyHeader>
          <TableHead>
            <TableRow>
              <TableCell sx={{ bgcolor: '#fcfbf9', color: '#7a7670', fontWeight: 700, fontSize: '0.75rem', py: 2, borderBottom: '1px solid #e5e2dc' }}>ID</TableCell>
              <TableCell sx={{ bgcolor: '#fcfbf9', color: '#7a7670', fontWeight: 700, fontSize: '0.75rem', py: 2, borderBottom: '1px solid #e5e2dc' }}>CLIENTE</TableCell>
              <TableCell sx={{ bgcolor: '#fcfbf9', color: '#7a7670', fontWeight: 700, fontSize: '0.75rem', py: 2, borderBottom: '1px solid #e5e2dc' }}>MONTO</TableCell>
              <TableCell sx={{ bgcolor: '#fcfbf9', color: '#7a7670', fontWeight: 700, fontSize: '0.75rem', py: 2, borderBottom: '1px solid #e5e2dc' }}>EMISIÓN</TableCell>
              <TableCell sx={{ bgcolor: '#fcfbf9', color: '#7a7670', fontWeight: 700, fontSize: '0.75rem', py: 2, borderBottom: '1px solid #e5e2dc' }}>ESTADO</TableCell>
              <TableCell sx={{ bgcolor: '#fcfbf9', color: '#7a7670', fontWeight: 700, fontSize: '0.75rem', py: 2, borderBottom: '1px solid #e5e2dc' }}>LÍMITE DESACTIVACIÓN</TableCell>
              <TableCell sx={{ bgcolor: '#fcfbf9', color: '#7a7670', fontWeight: 700, fontSize: '0.75rem', py: 2, borderBottom: '1px solid #e5e2dc' }}></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {facturas.length === 0 ? (
              <TableRow><TableCell colSpan={7} align="center" sx={{ py: 10 }}>No hay facturas para mostrar</TableCell></TableRow>
            ) : facturas.map((f: any) => (
              <FacturaRow key={f.id} factura={f} onProcesar={handleProcesar} processing={processingId === f.id} />
            ))}
          </TableBody>
        </Table>
      </TableContainer>
      <Snackbar open={snackbar.open} autoHideDuration={3000} onClose={() => setSnackbar({...snackbar, open: false})}>
        <Alert severity={snackbar.severity} variant="filled" sx={{ borderRadius: '12px' }}>{snackbar.message}</Alert>
      </Snackbar>
    </Box>
  );
};

export default FacturasResumen;