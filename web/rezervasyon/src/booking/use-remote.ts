import { useCallback, useEffect, useState } from 'react';

// Küçük sunucu verisi kancası: TanStack Query yerine (salon sayfasının JS bütçesi, T-55/T-56). İstek anahtarı
// JSON olarak verilir; aynı anahtar akış boyunca bellekten döner (geri tuşunda yeniden istek yok).

export interface Remote<T> {
  data: T | undefined;
  error: unknown;
  loading: boolean;
  /** Önbelleği atıp yeniden ister (saat doldu, tekrar dene). */
  reload: () => void;
}

interface Settled<T> {
  key: string;
  data?: T;
  error?: unknown;
}

/**
 * @param request İstek gövdesi (null: istek yok). Anahtar JSON karşılığıdır.
 * @param load Modül düzeyinde sabit fonksiyon; isteği JSON'dan geri okur.
 */
export function useRemote<R, T>(request: R | null, load: (request: R) => Promise<T>): Remote<T> {
  const key = request === null ? null : JSON.stringify(request);
  const [cache] = useState(() => new Map<string, Promise<T>>());
  const [settled, setSettled] = useState<Settled<T> | null>(null);
  const [version, setVersion] = useState(0);

  useEffect(() => {
    if (key === null) return;
    let live = true;
    let pending = cache.get(key);
    if (!pending) {
      pending = load(JSON.parse(key) as R);
      cache.set(key, pending);
      // Hata önbellekte kalmaz; sonraki deneme yeniden ister.
      pending.catch(() => cache.delete(key));
    }
    pending.then(
      (data) => {
        if (live) setSettled({ key, data });
      },
      (error: unknown) => {
        if (live) setSettled({ key, error });
      },
    );
    return () => {
      live = false;
    };
  }, [key, load, cache, version]);

  const reload = useCallback(() => {
    if (key !== null) cache.delete(key);
    setSettled(null);
    setVersion((value) => value + 1);
  }, [cache, key]);

  const current = settled !== null && settled.key === key ? settled : null;
  return {
    data: current?.data,
    error: current?.error,
    loading: key !== null && current === null,
    reload,
  };
}
