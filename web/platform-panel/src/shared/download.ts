/** Tarayıcıda dosya indirir (dekont, denetim kaydı CSV'si). */
export function downloadFile(blob: Blob, fileName: string) {
  const url = URL.createObjectURL(blob);
  const link = document.createElement('a');
  link.href = url;
  link.download = fileName;
  document.body.append(link);
  link.click();
  link.remove();
  URL.revokeObjectURL(url);
}

/** Content-Disposition başlığındaki dosya adı; yoksa null. */
export function dispositionFileName(response: Response): string | null {
  const disposition = response.headers.get('Content-Disposition') ?? '';
  return /filename="?([^";]+)"?/.exec(disposition)?.[1] ?? null;
}
