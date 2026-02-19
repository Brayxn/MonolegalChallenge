import React from 'react';

interface ToastProps {
  message: string;
  type?: 'ok' | 'error';
}


const Toast: React.FC<ToastProps> = ({ message, type = 'ok' }) => (
  <div className="toast" style={{ background: type === 'ok' ? 'var(--green)' : 'var(--text)' }}>
    {message}
  </div>
);

export default Toast;
