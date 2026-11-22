import { useEffect, useState } from 'react';

/** Saniye sayacı (yeniden gönderme, deneme sınırı); saatten değil, aralıktan sayar. */
export function useCountdown(initialSeconds: number): [number, (seconds: number) => void] {
  const [seconds, setSeconds] = useState(initialSeconds);
  useEffect(() => {
    if (seconds <= 0) {
      return;
    }

    const timer = setTimeout(() => {
      setSeconds((s) => s - 1);
    }, 1000);
    return () => {
      clearTimeout(timer);
    };
  }, [seconds]);
  return [seconds, setSeconds];
}
