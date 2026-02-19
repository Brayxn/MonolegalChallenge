import { useEffect, useState } from 'react';
import axios from 'axios';
import API_BASE_URL from '../config/api';

export function useBackendStatus() {
  // Cambiamos el estado inicial a 'loading' para evitar falsos positivos al arrancar
  const [status, setStatus] = useState<'ok' | 'error' | 'loading'>('loading');

  useEffect(() => {
    let cancelled = false;

    async function check() {
      try {
        // 1. Usa una ruta de API real o la raíz, no el .html de Swagger
        // 2. Agregamos un timeout corto para que no se quede colgado
        await axios.get(`${API_BASE_URL}/correos`, { timeout: 2000 });
        
        if (!cancelled) setStatus('ok');
      } catch (err) {
        if (!cancelled) {
          // Si el backend responde (aunque sea un 404), el servidor está VIVO.
          // Si no hay respuesta (network error), el servidor está CAÍDO.
          if (axios.isAxiosError(err) && err.response) {
            setStatus('ok'); // El server respondió, así que está corriendo
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