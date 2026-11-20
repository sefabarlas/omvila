import { systemClock } from '@omvila/ui-web/time';
import type { Clock } from '@omvila/ui-web/time';
import { createContext, useContext } from 'react';

/** "Şimdi" yalnız buradan (testte sabit saat verilir). */
export const ClockContext = createContext<Clock>(systemClock);

export function useClock(): Clock {
  return useContext(ClockContext);
}
