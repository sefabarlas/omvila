import type { CustomerImportPreview, CustomerImportRowError } from './api';

// CSV içe aktarma görünümü (salon-paneli/MusteriIceAktar): sütun adları, hata metinleri, "Hatalı satırları indir"
// için istemcide üretilen CSV (sözleşme CustomerImportPreview: hatalı satırlar önizlemede liste olarak gelir).

const columnLabels: Record<CustomerImportPreview['mappedColumns'][number], string> = {
  FullName: 'Ad',
  Phone: 'Telefon',
  Notes: 'Not',
  BirthYear: 'Doğum yılı',
  Email: 'E-posta',
};

const errorLabels: Record<CustomerImportRowError['code'], string> = {
  invalid_phone: 'telefon okunamadı',
  missing_name: 'ad yok',
  duplicate_in_file: 'dosyada tekrar ediyor',
  too_long: 'alan çok uzun',
};

/** "Ad, Telefon, Not sütunları eşlendi". */
export function mappedColumnsText(columns: CustomerImportPreview['mappedColumns']): string {
  return columns.length === 0
    ? 'Eşlenen sütun yok'
    : `${columns.map((c) => columnLabels[c]).join(', ')} sütunları eşlendi`;
}

export function importErrorLabel(code: CustomerImportRowError['code']): string {
  return errorLabels[code];
}

/** Hatalı satır kartının alt satırı: tek tür hata varsa onun metni ("telefon okunamadı"), yoksa "atlanacak". */
export function invalidRowsSummary(errors: readonly CustomerImportRowError[]): string {
  const codes = new Set(errors.map((e) => e.code));
  const [only] = codes;
  return codes.size === 1 && only ? errorLabels[only] : 'atlanacak';
}

function csvCell(value: string): string {
  return /[;"\n]/.test(value) ? `"${value.replace(/"/g, '""')}"` : value;
}

/** Hatalı satırlar CSV'si (UTF-8 BOM'lu, noktalı virgül ayraçlı; Excel Türkçe yerelinde doğrudan açılır). */
export function errorRowsCsv(errors: readonly CustomerImportRowError[]): string {
  const lines = errors.map((e) =>
    [String(e.rowNumber), e.column ?? '', errorLabels[e.code]].map(csvCell).join(';'),
  );
  return `\uFEFF${['Satır;Sütun;Hata', ...lines].join('\r\n')}\r\n`;
}

/** "musteriler-eski-program.csv" → "musteriler-eski-program-hatali-satirlar.csv". */
export function errorRowsFileName(fileName: string): string {
  return `${fileName.replace(/\.csv$/i, '')}-hatali-satirlar.csv`;
}

/** Dosya hatası (400 validation_failed, errors.file): çok büyük veya CSV değil. */
export function fileErrorText(codes: readonly string[] | undefined): string | null {
  if (codes?.includes('too_large')) {
    return 'Dosya çok büyük. Listeyi bölüp parça parça içe aktarın.';
  }

  if (codes?.includes('invalid_format')) {
    return 'Dosya okunamadı. UTF-8 CSV olarak kaydedip yeniden seçin.';
  }

  return null;
}
