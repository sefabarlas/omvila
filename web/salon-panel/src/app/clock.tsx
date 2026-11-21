import { systemClock } from '@omvila/ui-web/time';
import type { Clock } from '@omvila/ui-web/time';
import { createContext, useContext, useEffect, useState } from 'react';

/** "Şimdi" yalnız buradan (testte sabit saat verilir). */
export const ClockContext = createContext<Clock>(systemClock);

export function useClock(): Clock {
  return useContext(ClockContext);
}

/**
 * Enjekte edilen saatin "şimdi"si; verilen aralıkla yeniden okunur (kalan süre sayacı). Testte sabit saat döner.
 */
export function useNow(intervalMs = 30_000): Date {
  const clock = useClock();
  const [now, setNow] = useState(() => clock.now());
  useEffect(() => {
    const timer = setInterval(() => {
      setNow(clock.now());
    }, intervalMs);
    return () => {
      clearInterval(timer);
    };
  }, [clock, intervalMs]);
  return now;
}
