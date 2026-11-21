import { addDays } from '@omvila/ui-web/time';
import { useEffect, useRef, useState } from 'react';
import { searchDays } from './booking-api';
import type { DaySearchResult, ItemRequest } from './booking-api';

// Gün şeridi: sözleşme tek istekte en çok 31 gün verir (DaySearchRequest.to). Şerit bugünden başlar; seçili gün
// (veya "en yakın uygun saat"e geçiş) yüklenen aralığın sonuna yaklaşınca sonraki 31 gün eklenir, maxDate'e kadar.

export type Day = DaySearchResult['days'][number];

const windowDays = 31;
/** Seçili günün ardından en az bu kadar gün şeritte görünür. */
const lookAheadDays = 7;

export interface DaysQuery {
  branchId: string;
  items: ItemRequest[];
  sameStaffForAll: boolean;
}

export interface DaysState {
  days: Day[];
  maxDate: string | undefined;
  /** Yüklenen son aralıkta hiç boş gün yoksa ilk uygun saat (T-51 (6)). */
  nextAvailableUtc: string | null;
  loading: boolean;
  error: unknown;
  reload: () => void;
}

interface Range {
  key: string;
  from: string;
  to: string;
}

const minDate = (a: string, b: string) => (a < b ? a : b);

/**
 * @param query null: istek yok (saat adımı açık değil).
 * @param through Bu güne kadar yüklensin (seçili gün veya geçiş hedefi).
 */
export function useDays(query: DaysQuery | null, today: string, through: string | null): DaysState {
  const base = query ? JSON.stringify(query) : null;
  const [results, setResults] = useState<
    Record<string, { data?: DaySearchResult; error?: unknown }>
  >({});
  const inFlight = useRef(new Set<string>());
  const [version, setVersion] = useState(0);

  // Gereken aralıklar: ilki her zaman; sonrakiler öncekiler yüklendikçe ve maxDate izin verdikçe.
  const windows: Range[] = [];
  if (base !== null) {
    let from = today;
    let maxDate: string | undefined;
    for (;;) {
      const last = addDays(from, windowDays - 1);
      const to = maxDate ? minDate(last, maxDate) : last;
      const key = `${base}|${from}|${to}|${String(version)}`;
      windows.push({ key, from, to });
      const loaded = results[key]?.data;
      maxDate ??= loaded?.maxDate;
      const target = through ? addDays(through, lookAheadDays) : today;
      if (!loaded || !maxDate || to >= maxDate || to >= target) break;
      from = addDays(to, 1);
    }
  }

  const wanted = windows.map((range) => range.key).join(';');
  useEffect(() => {
    if (base === null) return;
    const parsed = JSON.parse(base) as DaysQuery;
    for (const range of wanted.split(';')) {
      if (inFlight.current.has(range)) continue;
      const [, from, to] = range.split('|');
      if (!from || !to) continue;
      inFlight.current.add(range);
      searchDays(parsed.branchId, {
        items: parsed.items,
        sameStaffForAll: parsed.sameStaffForAll,
        from,
        to,
      }).then(
        (data) => {
          setResults((previous) => ({ ...previous, [range]: { data } }));
        },
        (error: unknown) => {
          inFlight.current.delete(range);
          setResults((previous) => ({ ...previous, [range]: { error } }));
        },
      );
    }
  }, [base, wanted]);

  const loaded = windows.map((range) => results[range.key]);
  const firstData = loaded[0]?.data;
  const maxDate = firstData?.maxDate;
  const days: Day[] = [];
  let nextAvailableUtc: string | null = null;
  for (const result of loaded) {
    if (!result?.data) break;
    days.push(...result.data.days);
    nextAvailableUtc = result.data.nextAvailableUtc ?? null;
  }

  return {
    days: maxDate ? days.filter((day) => day.date <= maxDate) : days,
    maxDate,
    nextAvailableUtc,
    loading: base !== null && !firstData && !loaded[0]?.error,
    error: loaded.find((result) => result?.error !== undefined)?.error,
    reload: () => {
      inFlight.current.clear();
      setVersion((value) => value + 1);
    },
  };
}
