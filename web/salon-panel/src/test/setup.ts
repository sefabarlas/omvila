// jsdom'da olmayan tarayıcı API'leri (tema ve yönlendirici kaydırması).
Object.defineProperty(window, 'matchMedia', {
  writable: true,
  value: (query: string) => ({
    matches: false,
    media: query,
    addEventListener: () => undefined,
    removeEventListener: () => undefined,
  }),
});
window.scrollTo = () => undefined;
