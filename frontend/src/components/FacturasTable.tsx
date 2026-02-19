

import React, { useEffect, useState } from 'react';
import { FiPlay } from 'react-icons/fi';


interface Factura {
  id: string;
  cliente: string;
  clienteNombre: string;
  empresa: string;
  monto: number;
  emision: string;
  estado: string;
  limite: string | number;
  descripcion: string;
  fechaLimiteDesactivacion?: string | null;
}

interface FacturasTableProps {
  facturas: Factura[];
  onProcesar: (id: string) => void;
  processingId?: string | null;
}

const FacturasTable: React.FC<FacturasTableProps> = ({ facturas, onProcesar, processingId }) => {
  // Dynamic "Límite" countdown
  const [limites, setLimites] = useState<{[id:string]:string}>({});

  useEffect(() => {
    const updateLimites = () => {
      const now = Date.now();
      const newLimites: {[id:string]:string} = {};
      facturas.forEach(f => {
        // Solo para segundo recordatorio y si hay fechaLimiteDesactivacion
        if (
          (f.estado === 'Segundo recordatorio' || f.estado === 'segundo recordatorio' || f.estado === 'segundorecordatorio') &&
          f.fechaLimiteDesactivacion
        ) {
          let target = new Date(f.fechaLimiteDesactivacion).getTime();
          let diff = target - now;
          if (diff <= 0) newLimites[f.id] = 'Desactivando...';
          else {
            let min = Math.floor(diff/60000);
            let sec = Math.floor((diff%60000)/1000);
            newLimites[f.id] = `${min}m ${sec}s`;
          }
        } else {
          newLimites[f.id] = '-';
        }
      });
      setLimites(newLimites);
    };
    updateLimites();
    const timer = setInterval(updateLimites, 1000);
    return () => clearInterval(timer);
  }, [facturas]);

  return (
    <div className="table-wrap">
      <table>
        <thead>
          <tr>
            <th>Factura</th>
            <th>Cliente</th>
            <th>Monto</th>
            <th>Emisión</th>
            <th>Estado</th>
            <th>Límite</th>
            <th>Descripción</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {facturas.length === 0 ? (
            <tr><td colSpan={8} style={{textAlign:'center',padding:'40px',color:'var(--muted)'}}>No hay facturas para mostrar</td></tr>
          ) : facturas.map(f => {
            let bCls = '';
            let estadoLabel = '';
            if(f.estado === 'Primer recordatorio' || f.estado === 'primer recordatorio' || f.estado === 'primerrecordatorio') {
              bCls = 'badge-first';
              estadoLabel = 'Primer recordatorio';
            } else if(f.estado === 'Segundo recordatorio' || f.estado === 'segundo recordatorio' || f.estado === 'segundorecordatorio') {
              bCls = 'badge-second';
              estadoLabel = 'Segundo recordatorio';
            } else if(f.estado === 'Desactivado') {
              bCls = 'badge-disabled';
              estadoLabel = 'Desactivado';
            } else {
              bCls = 'badge-first';
              estadoLabel = f.estado;
            }
            // Formatear fecha de emisión
            let fechaEmision = f.emision;
            if (f.emision && !isNaN(Date.parse(f.emision))) {
              const d = new Date(f.emision);
              fechaEmision = d.toLocaleDateString('es-CL', { year: 'numeric', month: '2-digit', day: '2-digit' });
            }
            return (
              <tr key={f.id}>
                <td><span style={{fontFamily:'monospace',fontWeight:600,color:'var(--accent)'}}>#{f.id}</span></td>
                <td><div style={{fontWeight:600}}>{f.clienteNombre}</div></td>
                <td><span style={{fontFamily:'monospace',fontWeight:600}}>${f.monto.toLocaleString()}</span></td>
                <td>{fechaEmision}</td>
                <td><span className={`badge ${bCls}`}>{estadoLabel}</span></td>
                <td style={{color:'var(--muted)'}}>
                  {estadoLabel === 'Segundo recordatorio' && f.fechaLimiteDesactivacion ? limites[f.id] : '-'}
                </td>
                <td style={{fontSize:13}}>{f.descripcion}</td>
                <td style={{textAlign:'right'}}>
                  <button className="btn btn-dark" style={{padding:'8px 16px',fontSize:12,position:'relative',display:'flex',alignItems:'center',gap:6}} onClick={()=>onProcesar(f.id)} disabled={estadoLabel==='Desactivado'||processingId===f.id}>
                    <FiPlay size={14} style={{marginRight:4}} />
                    {processingId===f.id ? <span className="spinner" style={{marginLeft:4}}></span> : null}
                    Procesar
                  </button>
                </td>
              </tr>
            );
          })}
        </tbody>
      </table>
    </div>
  );
};

export default FacturasTable;
