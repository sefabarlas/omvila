/** Gün içi selamlama (GenelBakisBos "Günaydın, Elif"), şubenin saatine göre. */
export function greeting(hour: number): string {
  if (hour >= 5 && hour < 12) {
    return 'Günaydın';
  }

  return hour >= 12 && hour < 18 ? 'İyi günler' : 'İyi akşamlar';
}

/** Ad soyaddan ilk ad; ad yoksa boş (kayıtta ad sorulmaz, Dilim 1 varsayımı 2). */
export function firstName(fullName: string): string {
  return fullName.trim().split(/\s+/)[0] ?? '';
}
