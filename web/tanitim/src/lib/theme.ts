'use client';

import { useSyncExternalStore } from 'react';

// Tema tercihi (Açık · Koyu · Sistem; tasarım TemaMenu). Sistem varsayılan: işletim sisteminin teması izlenir.
// Seçim bu tarayıcıda saklanır; ilk boyamayı layout.tsx'teki satır içi betik aynı anahtarla karşılar.
export type ThemePreference = 'light' | 'dark' | 'system';

export const themeStorageKey = 'omvila.tema';
const darkQuery = '(prefers-color-scheme: dark)';
const listeners = new Set<() => void>();

function read(): ThemePreference {
  try {
    const value = localStorage.getItem(themeStorageKey);
    return value === 'light' || value === 'dark' ? value : 'system';
  } catch {
    return 'system';
  }
}

let current: ThemePreference | null = null;

function snapshot(): ThemePreference {
  current ??= read();
  return current;
}

function subscribe(listener: () => void): () => void {
  listeners.add(listener);
  return () => {
    listeners.delete(listener);
  };
}

export function isDark(preference: ThemePreference): boolean {
  return (
    preference === 'dark' ||
    (preference === 'system' &&
      typeof window !== 'undefined' &&
      typeof window.matchMedia === 'function' &&
      window.matchMedia(darkQuery).matches)
  );
}

/** Kökteki `data-theme`'i tercihe göre yazar. */
export function applyTheme(preference: ThemePreference = snapshot()): void {
  document.documentElement.dataset.theme = isDark(preference) ? 'dark' : 'light';
}

export function setThemePreference(preference: ThemePreference): void {
  try {
    if (preference === 'system') {
      localStorage.removeItem(themeStorageKey);
    } else {
      localStorage.setItem(themeStorageKey, preference);
    }
  } catch {
    // Depolama kapalıysa seçim bu sayfa yüklemesiyle sınırlı kalır.
  }

  current = preference;
  applyTheme(preference);
  listeners.forEach((listener) => {
    listener();
  });
}

export function useThemePreference(): ThemePreference {
  return useSyncExternalStore(subscribe, snapshot, () => 'system');
}

function subscribeResolved(listener: () => void): () => void {
  const unsubscribe = subscribe(listener);
  if (typeof window.matchMedia !== 'function') {
    return unsubscribe;
  }

  const media = window.matchMedia(darkQuery);
  media.addEventListener('change', listener);
  return () => {
    unsubscribe();
    media.removeEventListener('change', listener);
  };
}

/** Etkin tema koyu mu; sunucu çiziminde ve hidrasyonda açık kabul edilir (çıktı uyuşsun diye). */
export function useResolvedDark(): boolean {
  return useSyncExternalStore(
    subscribeResolved,
    () => isDark(snapshot()),
    () => false,
  );
}

export const darkMediaQuery = darkQuery;
