import { useEffect, useState } from 'react';
import axios from 'axios';
import API_BASE_URL from '../config/api';

export function useBackendStatus() {
  const [status, setStatus] = useState<'ok' | 'error' | 'loading'>('loading');

  useEffect(() => {
    let cancelled = false;

    async function check() {
      try {
        await axios.get(`${API_BASE_URL}/correos`, { timeout: 2000 });
        
        if (!cancelled) setStatus('ok');
      } catch (err) {
        if (!cancelled) {
          if (axios.isAxiosError(err) && err.response) {
            setStatus('ok');
          } else {
            setStatus('error');
          }
        }
      }
    }

    check();
    const interval = setInterval(check, 10000); // 10 segundos es más que suficiente
    return () => { 
      cancelled = true; 
      clearInterval(interval); 
    };
  }, []);

  return status;
}