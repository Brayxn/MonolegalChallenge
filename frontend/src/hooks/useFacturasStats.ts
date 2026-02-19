import { useEffect, useState } from 'react';
import axios from 'axios';
import API_BASE_URL from '../config/api';

export function useFacturasStats(refreshKey?: number) {
  const [stats, setStats] = useState({
    totalFacturas: 0,
    montoTotal: 0,
    desactivados: 0,
    clientes: 0,
    loading: true,
    error: null as string | null,
  });

  useEffect(() => {
    setStats(s => ({ ...s, loading: true, error: null }));
    axios.get(`${API_BASE_URL}/facturas`)
      .then(res => {
        const facturas = res.data || [];
        const totalFacturas = facturas.length;
        const montoTotal = facturas.reduce((acc: number, f: any) => acc + (f.monto || 0), 0);
        const desactivados = facturas.filter((f: any) => f.estado === 'desactivado').length;
        const clientesSet = new Set(facturas.map((f: any) => f.clienteId));
        setStats({
          totalFacturas,
          montoTotal,
          desactivados,
          clientes: clientesSet.size,
          loading: false,
          error: null,
        });
      })
      .catch(err => setStats(s => ({ ...s, loading: false, error: err.message })));
  }, [refreshKey]);

  return stats;
}
