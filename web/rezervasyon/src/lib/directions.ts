interface BranchLocation {
  addressLine: string;
  district: string;
  latitude?: number | null;
  longitude?: number | null;
}

/**
 * "Yol tarifi" bağlantısı: koordinat varsa ona, yoksa açık adrese (Basari, Randevum). Uygulama yüklü telefonda
 * harita uygulaması açılır.
 */
export function directionsUrl(branch: BranchLocation): string | null {
  if (branch.latitude != null && branch.longitude != null) {
    return `https://www.google.com/maps/dir/?api=1&destination=${String(branch.latitude)},${String(branch.longitude)}`;
  }
  const address = [branch.addressLine, branch.district].filter(Boolean).join(', ');
  return address
    ? `https://www.google.com/maps/dir/?api=1&destination=${encodeURIComponent(address)}`
    : null;
}
