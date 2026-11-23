import { createContext, createElement, useContext, useEffect, useState } from 'react';
import type { ReactNode } from 'react';

const storageKey = 'omvila.tema';
export type ThemePreference = 'system' | 'light' | 'dark';

/** Kullanıcının tema tercihi (yalnız bu tarayıcıda; okunamazsa sistem). */
export function readThemePreference(): ThemePreference {
  try {
    const value = localStorage.getItem(storageKey);
    return value === 'light' || value === 'dark' ? value : 'system';
  } catch {
    return 'system';
  }
}

export function saveThemePreference(preference: ThemePreference): void {
  try {
    if (preference === 'system') {
      localStorage.removeItem(storageKey);
    } else {
      localStorage.setItem(storageKey, preference);
    }
  } catch {
    // Depolama kapalıysa tercih bu oturumla sınırlı kalır.
  }
}

const darkQuery = '(prefers-color-scheme: dark)';

function systemDark(): boolean {
  return typeof window.matchMedia === 'function' && window.matchMedia(darkQuery).matches;
}

/**
 * Koyu tema data-theme="dark" ile açılır (design-tokens). Tercih yoksa işletim sisteminin teması izlenir.
 * Tema seçici kenar menüde; anahtar salon paneliyle aynı (T-129): seçim bu tarayıcıda saklanır (localStorage).
 */
export function useThemeEffect(preference: ThemePreference = readThemePreference()): boolean {
  const [osDark, setOsDark] = useState(systemDark);
  useEffect(() => {
    const media = window.matchMedia(darkQuery);
    const onChange = () => {
      setOsDark(media.matches);
    };
    media.addEventListener('change', onChange);
    return () => {
      media.removeEventListener('change', onChange);
    };
  }, []);
  const dark = preference === 'dark' || (preference === 'system' && osDark);
  useEffect(() => {
    document.documentElement.dataset.theme = dark ? 'dark' : 'light';
  }, [dark]);
  return dark;
}

interface ThemeApi {
  preference: ThemePreference;
  /** Seçimi uygular ve bu tarayıcıda saklar ("Sistem" kaydı siler). */
  setPreference: (preference: ThemePreference) => void;
}

const ThemeContext = createContext<ThemeApi>({
  preference: 'system',
  setPreference: () => undefined,
});

/** Kök düzeyde tema tercihi; kenar menüdeki seçici `useTheme` ile değiştirir. */
export function ThemeProvider({ children }: { children: ReactNode }) {
  const [preference, setPreference] = useState(readThemePreference);
  useThemeEffect(preference);
  const choose = (next: ThemePreference) => {
    saveThemePreference(next);
    setPreference(next);
  };
  return createElement(
    ThemeContext.Provider,
    { value: { preference, setPreference: choose } },
    children,
  );
}

export function useTheme(): ThemeApi {
  return useContext(ThemeContext);
}
