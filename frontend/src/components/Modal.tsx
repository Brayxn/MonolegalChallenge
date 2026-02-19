import React, { useState } from 'react';

interface ModalProps {
  open: boolean;
  title: string;
  description: string;
  onCancel: () => void;
  onConfirm: () => void;
}

const Modal: React.FC<ModalProps> = ({ open, title, description, onCancel, onConfirm }) => {
  const [loading, setLoading] = useState(false);
  if (!open) return null;
  const handleConfirm = async () => {
    setLoading(true);
    await onConfirm();
    setLoading(false);
  };
  return (
    <div className="modal-bg open">
      <div className="modal-box">
        <h3>{title}</h3>
        <p>{description}</p>
        <div className="modal-actions">
          <button className="btn btn-ghost" onClick={onCancel}>Cancelar</button>
          <button className="btn btn-dark" onClick={handleConfirm} style={{position:'relative',display:'flex',alignItems:'center',gap:8}} disabled={loading}>
            {loading && <span className="spinner" style={{marginRight:8}}></span>}
            Confirmar
          </button>
        </div>
      </div>
    </div>
  );
};

export default Modal;
