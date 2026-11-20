// "Sonra yaparım" (KayitKurulum): kurulum listesi bu tarayıcıda ertelenir, genel bakış açılır. Tercih yalnız bu
// tarayıcıda; okunamazsa liste yeniden görünür (zararsız).
const key = (brandId: string) => `omvila.kurulum-ertelendi.${brandId}`;

export function isOnboardingDismissed(brandId: string): boolean {
  try {
    return localStorage.getItem(key(brandId)) === '1';
  } catch {
    return false;
  }
}

export function dismissOnboarding(brandId: string): void {
  try {
    localStorage.setItem(key(brandId), '1');
  } catch {
    // Depolama kapalıysa erteleme bu oturumla sınırlı (bileşen durumu).
  }
}
