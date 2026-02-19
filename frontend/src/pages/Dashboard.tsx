
import React, { useState, useMemo } from 'react';
import { useFetch } from '../hooks/useFetch';
import API_BASE_URL from '../config/api';
import axios from 'axios';

import Topbar from '../components/Topbar';
import StatsGrid from '../components/StatsGrid';
import TabsPanel from '../components/TabsPanel';
import FacturasTable from '../components/FacturasTable';
import Modal from '../components/Modal';
import Toast from '../components/Toast';
import CorreoHistorial from '../components/CorreoHistorial';
import { FiRefreshCw, FiSend, FiMail, FiFileText } from 'react-icons/fi';

import { useFacturasStats } from '../hooks/useFacturasStats';
import { useCorreoHistorial } from '../hooks/useCorreoHistorial';


const Dashboard: React.FC = () => {
  const [refreshKey, setRefreshKey] = useState(0);
  const [processing, setProcessing] = useState(false);
  const [toast, setToast] = useState<{ open: boolean; text: string; type?: 'ok'|'error' }>({ open: false, text: '' });

  // Ocultar toast automáticamente después de 3 segundos
  React.useEffect(() => {
    if (toast.open) {
      const timer = setTimeout(() => setToast(t => ({ ...t, open: false })), 3000);
      return () => clearTimeout(timer);
    }
  }, [toast.open]);
  const [tab, setTab] = useState(0);
  const [cliente, setCliente] = useState('');
  const [modal, setModal] = useState<{ open: boolean; title: string; desc: string; onConfirm?: () => void }>({ open: false, title: '', desc: '' });
  const [processingId, setProcessingId] = useState<string|null>(null);

  const stats = useFacturasStats(refreshKey);
  const { correos } = useCorreoHistorial(refreshKey);
  const { data: facturas } = require('../hooks/useFetch').useFetch(`${API_BASE_URL}/facturas`, refreshKey);
  const facturasArray = facturas || [];

  // Refrescar correos al borrar sin recargar
  React.useEffect(() => {
    (window as any).onCorreoBorrado = () => setRefreshKey(k => k + 1);
    return () => { (window as any).onCorreoBorrado = undefined; };
  }, []);

  // Obtener lista de clientes desde el backend
  const { data: clientesData } = useFetch(`${API_BASE_URL}/clientes`, refreshKey);
  const clientes = Array.isArray(clientesData) ? clientesData : [];

  // Filtrar facturas y correos por cliente
  const facturasFiltradas = useMemo(() => cliente ? facturasArray.filter((f: any) => f.clienteId === cliente) : facturasArray, [facturasArray, cliente]);
  const correosFiltrados = useMemo(() => cliente ? (correos || []).filter((c: any) => c.destinatario && c.destinatario.includes(cliente)) : (correos || []), [correos, cliente]);

  const handleRefresh = () => setRefreshKey(k => k + 1);

  const handleProcesar = (id: string) => {
    setModal({
      open: true,
      title: `Procesar Factura #${id}`,
      desc: `¿Confirmas el procesamiento de la factura #${id}? Se enviará el recordatorio y cambiará de estado.`,
      onConfirm: async () => {
        setProcessingId(id);
        try {
          await axios.post(`${API_BASE_URL}/facturas/procesar-recordatorio/${id}`);
          setToast({ open: true, text: `Factura #${id} procesada`, type: 'ok' });
          handleRefresh();
        } catch {
          setToast({ open: true, text: 'Error al procesar', type: 'error' });
        } finally {
          setProcessingId(null);
          setModal(m => ({ ...m, open: false }));
        }
      }
    });
  };

  const procesarTodo = () => {
    setModal({
      open: true,
      title: 'Procesar Masivo',
      desc: `¿Deseas procesar todas las facturas pendientes? Se enviarán los recordatorios correspondientes.`,
      onConfirm: async () => {
        setProcessing(true);
        try {
          await axios.post(`${API_BASE_URL}/facturas/procesar-recordatorios`);
          setToast({ open: true, text: 'Proceso masivo completado con éxito', type: 'ok' });
          handleRefresh();
        } catch {
          setToast({ open: true, text: 'Error en el proceso masivo', type: 'error' });
        } finally {
          setProcessing(false);
          setModal(m => ({ ...m, open: false }));
        }
      }
    });
  };

  return (
    <>
      <Topbar />
      <div className="main">
        <div className="section-label">Resumen General</div>
        <StatsGrid
          totalFacturas={stats?.totalFacturas || 0}
          montoTotal={stats?.montoTotal || 0}
          pendientes={facturasArray.filter((f: any) => f.estado && f.estado.includes('recordatorio')).length}
          desactivados={facturasArray.filter((f: any) => f.estado === 'desactivado' || f.estado === 'Desactivado').length}
          clientes={new Set(facturasArray.map((f: any) => f.clienteId)).size}
        />
        <TabsPanel
          tabs={[
            {
              label: `Facturas (${facturasFiltradas.length})`,
              icon: (
                <FiFileText size={20} strokeWidth={2} />
              ),
              content: (
                <div className="card">
                  <div className="card-header">
                    <div>
                      <h3>Resumen de Facturas <span className="sub">({facturasFiltradas.length} facturas)</span></h3>
                    </div>
                    <div className="btn-group">
                      <select className="fi" value={cliente} onChange={e => setCliente(e.target.value)}>
                        <option value="">Todos los clientes</option>
                        {clientes.map((c: any) => (
                          <option key={c.id} value={c.id}>{c.nombre}</option>
                        ))}
                      </select>
                      <button className="btn btn-ghost" onClick={handleRefresh}>
                        <FiRefreshCw size={18} style={{marginRight:6,verticalAlign:'middle'}} className="spin-on-hover" />
                        Refrescar
                      </button>
                      <button className="btn btn-dark" onClick={procesarTodo} disabled={processing}>
                        <FiSend size={18} style={{marginRight:8,verticalAlign:'middle'}} />
                        Procesar Masivo
                      </button>
                    </div>
                  </div>
                  <FacturasTable facturas={facturasFiltradas} onProcesar={handleProcesar} processingId={processingId} />
                </div>
              )
            },
            {
              label: `Correos Enviados (${correosFiltrados.length})`,
              icon: (
                <FiMail size={20} />
              ),
              content: (
                <div className="card">
                  <div className="card-header">
                    <h3>Todos los Correos Enviados</h3>
                  </div>
                  <div style={{padding:0}}>
                    <div style={{padding: '0 0 20px 0'}}>
                      <CorreoHistorial correos={correosFiltrados} />
                    </div>
                  </div>
                </div>
              )
            }
          ]}
          value={tab}
          onChange={setTab}
        />
      </div>
      <Modal
        open={modal.open}
        title={modal.title}
        description={modal.desc}
        onCancel={() => setModal(m => ({ ...m, open: false }))}
        onConfirm={modal.onConfirm || (() => setModal(m => ({ ...m, open: false })))}
      />
      {toast.open && <Toast message={toast.text} type={toast.type} />}
    </>
  );
};

export default Dashboard;