/**
 * SMS'in gideceği yer: "0532 *** ** 67 numarasına"; maskeli numara bilinmiyorsa (bekleme teklifi bağlantısı, kod
 * adımı yok) "telefonunuza".
 */
export function sentTo(phoneMasked: string | null | undefined): string {
  return phoneMasked ? `${phoneMasked} numarasına` : 'telefonunuza';
}
