// Önizleme sarmalayıcısı: yazı tipi, ölçek tokenları ve tema. Koyu tema data-theme ile açılır (design-tokens).
// İki ölçek de :root üzerinde tanımlıdır; aynı sayfada yalnız biri yüklenir: "Rezervasyon/" hikâyeleri mobil
// ölçeği (rezervasyon sayfası gibi), diğerleri panel ölçeğini kullanır.
import '@omvila/design-tokens/fonts.css';
import mobilTokens from '@omvila/design-tokens/mobil.css?inline';
import panelTokens from '@omvila/design-tokens/panel.css?inline';
import { ThemeState } from '@ladle/react';
import type { GlobalProvider } from '@ladle/react';
import { useEffect } from 'react';
import './preview.css';

export const Provider: GlobalProvider = ({ children, globalState }) => {
  const theme = globalState.theme === ThemeState.Dark ? 'dark' : 'light';
  const booking = globalState.story.startsWith('rezervasyon--');
  useEffect(() => {
    document.documentElement.dataset.theme = theme;
    document.documentElement.lang = 'tr';
  }, [theme]);
  return (
    <>
      <style>{booking ? mobilTokens : panelTokens}</style>
      <div className={booking ? 'omvila-preview omvila-preview-mobil' : 'omvila-preview'}>
        {children}
      </div>
    </>
  );
};
