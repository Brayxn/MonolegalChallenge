import React from 'react';
import { Box, Paper, Typography } from '@mui/material';
import { 
  MonetizationOnRounded, 
  ReceiptLongRounded, 
  PersonRounded, 
  BlockRounded 
} from '@mui/icons-material';

interface DashboardStatsProps {
  totalFacturas: number;
  montoTotal: number;
  desactivados: number;
  clientes: number;
}

const StatCard = ({ title, value, icon, color, bg }: any) => (
  <Paper elevation={0} sx={{
    p: 2.5,
    borderRadius: '12px',
    border: '1px solid #e5e2dc',
    bgcolor: '#fff',
    display: 'flex',
    flexDirection: 'column',
    gap: 1.5,
    minWidth: 0,
    boxShadow: '0 1px 3px rgba(0,0,0,0.03)',
    transition: 'box-shadow 0.2s, transform 0.2s',
    '&:hover': {
      boxShadow: '0 4px 16px rgba(200,168,75,0.10)',
      borderColor: '#c8a84b',
      transform: 'translateY(-2px)'
    }
  }}>
    <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 0.5 }}>
      <Typography sx={{ fontSize: 13, fontWeight: 600, color: '#7a7670', textTransform: 'uppercase', letterSpacing: '0.06em' }}>{title}</Typography>
      <Box sx={{
        width: 32, height: 32, borderRadius: '8px', display: 'flex', alignItems: 'center', justifyContent: 'center', bgcolor: bg, color: color
      }}>{icon}</Box>
    </Box>
    <Typography sx={{ fontWeight: 800, fontSize: 24, color: '#1a1814', letterSpacing: '-0.5px' }}>{value}</Typography>
  </Paper>
);

const DashboardStats: React.FC<DashboardStatsProps> = ({ totalFacturas, montoTotal, desactivados, clientes }) => (
  <Box sx={{
    display: 'grid',
    gridTemplateColumns: { xs: '1fr 1fr', sm: 'repeat(4, 1fr)' },
    gap: 2.5,
    mb: 5
  }}>
    <StatCard title="Total Facturas" value={totalFacturas} icon={<ReceiptLongRounded fontSize="medium" />} color="#1a1814" bg="#f2f0ec" />
    <StatCard title="Monto Total" value={`$${montoTotal.toLocaleString('es-CL')}`} icon={<MonetizationOnRounded fontSize="medium" />} color="#c8a84b" bg="#fdf8ed" />
    <StatCard title="Desactivados" value={desactivados} icon={<BlockRounded fontSize="medium" />} color="#b83535" bg="#fdf0f0" />
    <StatCard title="Clientes" value={clientes} icon={<PersonRounded fontSize="medium" />} color="#2d5fa6" bg="#edf3fd" />
  </Box>
);

export default DashboardStats;