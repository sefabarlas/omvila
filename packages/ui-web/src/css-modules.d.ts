declare module '*.module.css' {
  const classes: Readonly<Record<string, string>>;
  export default classes;
}

// Vite: CSS dosyasının içeriği metin olarak (önizlemede ölçek tokenlarını seçmek için).
declare module '*.css?inline' {
  const css: string;
  export default css;
}

declare module '*.svg' {
  const url: string;
  export default url;
}
