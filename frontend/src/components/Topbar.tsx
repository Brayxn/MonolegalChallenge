
import React from 'react';
import { useBackendStatus } from '../hooks/useBackendStatus';

const Topbar: React.FC = () => {
  const status = useBackendStatus();
  const isOk = status === 'ok';
  return (
    <header className="topbar">
      <div className="brand">
        <h1>Panel de Cobranza</h1>
        <p>Supervisa facturas y automatiza recordatorios de pago</p>
      </div>
      <div
        className="status-pill"
        style={{
          background: isOk ? 'var(--green-bg)' : '#fef2f2',
          color: isOk ? 'var(--green)' : '#dc2626',
          border: isOk ? '1px solid #bbf7d0' : '1px solid #fecaca',
        }}
      >
        <span
          className="dot"
          style={{ background: isOk ? 'var(--green)' : '#dc2626' }}
        ></span>
        {isOk ? 'Backend Activo' : 'Backend Desactivado'}
      </div>
    </header>
  );
};

export default Topbar;
