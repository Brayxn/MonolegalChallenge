import { useEffect, useState } from 'react';
import axios from 'axios';
import API_BASE_URL from '../config/api';

export interface CorreoHistorialItem {
  id: string;
  destinatario: string;
  asunto: string;
  fecha: string;
  facturaId?: string;
  cuerpo?: string;
}

export function useCorreoHistorial(refreshKey?: number) {
  const [correos, setCorreos] = useState<CorreoHistorialItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setLoading(true);
    setError(null);
    axios.get(`${API_BASE_URL}/correos`)
      .then(res => setCorreos(res.data || []))
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  }, [refreshKey]);

  return { correos, loading, error };
}
