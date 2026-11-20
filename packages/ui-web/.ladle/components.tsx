// Önizleme sarmalayıcısı: panel tokenları, yazı tipi ve tema. Koyu tema data-theme ile açılır (design-tokens).
import '@omvila/design-tokens/fonts.css';
import '@omvila/design-tokens/panel.css';
import { ThemeState } from '@ladle/react';
import type { GlobalProvider } from '@ladle/react';
import { useEffect } from 'react';
import './preview.css';

export const Provider: GlobalProvider = ({ children, globalState }) => {
  const theme = globalState.theme === ThemeState.Dark ? 'dark' : 'light';
  useEffect(() => {
    document.documentElement.dataset.theme = theme;
  }, [theme]);
  return <div className="omvila-preview">{children}</div>;
};
