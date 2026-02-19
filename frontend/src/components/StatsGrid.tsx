

import React from 'react';
import { FiFileText, FiDollarSign, FiClock, FiSlash, FiUsers } from 'react-icons/fi';


interface StatsGridProps {
  totalFacturas: number;
  montoTotal: number;
  pendientes: number;
  desactivados: number;
  clientes: number;
}


const StatsGrid: React.FC<StatsGridProps> = ({ totalFacturas, montoTotal, pendientes, desactivados, clientes }) => (
  <div className="stats-grid">
    <div className="stat-card">
      <div className="stat-label">
        <div className="stat-icon" style={{background:'#f3f3f1',color:'#555'}}>
          <FiFileText size={22} />
        </div>
        Total Facturas
      </div>
      <div className="stat-value">{totalFacturas}</div>
    </div>
    <div className="stat-card">
      <div className="stat-label">
        <div className="stat-icon" style={{background:'#fff7ed',color:'#c2410c'}}>
          <FiDollarSign size={22} />
        </div>
        Monto Total
      </div>
      <div className="stat-value">${montoTotal.toLocaleString()}</div>
    </div>
    <div className="stat-card">
      <div className="stat-label">
        <div className="stat-icon" style={{background:'#fffbeb',color:'#d97706'}}>
          <FiClock size={22} />
        </div>
        Pendientes
      </div>
      <div className="stat-value">{pendientes}</div>
    </div>
    <div className="stat-card">
      <div className="stat-label">
        <div className="stat-icon" style={{background:'#fef2f2',color:'#dc2626'}}>
          <FiSlash size={22} />
        </div>
        Desactivados
      </div>
      <div className="stat-value">{desactivados}</div>
    </div>
    <div className="stat-card">
      <div className="stat-label">
        <div className="stat-icon" style={{background:'#f0fdf4',color:'#16a34a'}}>
          <FiUsers size={22} />
        </div>
        Clientes
      </div>
      <div className="stat-value">{clientes}</div>
    </div>
  </div>
);

export default StatsGrid;
