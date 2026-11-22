/** Tarayıcıyı uygulama dışındaki bir adrese götürür (Google Takvim izin sayfası). Testte sahtesiyle değiştirilir. */
export function goToExternal(url: string): void {
  window.location.assign(url);
}
