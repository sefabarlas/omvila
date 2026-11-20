import { useEffect } from 'react';

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

/**
 * Koyu tema data-theme="dark" ile açılır (design-tokens). Tercih yoksa işletim sisteminin teması izlenir.
 * Tema seçici Ayarlar'da gelir; o zamana kadar sistem teması veya kayıtlı tercih.
 */
export function useThemeEffect(preference: ThemePreference = readThemePreference()): void {
  useEffect(() => {
    const root = document.documentElement;
    const media = window.matchMedia('(prefers-color-scheme: dark)');
    const apply = () => {
      const dark = preference === 'dark' || (preference === 'system' && media.matches);
      root.dataset.theme = dark ? 'dark' : 'light';
    };
    apply();
    media.addEventListener('change', apply);
    return () => {
      media.removeEventListener('change', apply);
    };
  }, [preference]);
}
