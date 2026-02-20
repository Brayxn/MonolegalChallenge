import { useState, useEffect } from 'react';
import axios from 'axios';
import API_BASE_URL from '../config/api';

export function useFetch<T>(url: string, refreshKey?: number) {
  const [data, setData] = useState<T | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setLoading(true);
    // Si la URL es relativa, prepende API_BASE_URL
    const fullUrl = url.startsWith('http') ? url : `${API_BASE_URL}${url}`;
    axios.get(fullUrl)
      .then(res => setData(res.data))
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  }, [url, refreshKey]);

  return { data, loading, error };
}
