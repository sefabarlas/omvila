import type { NotificationTemplateSetting } from './notification-api';

// Ayarlar → Bildirimler ve SMS metin yardımcıları (salon-paneli/AyarlarBildirim, bildirimler.md § Müşteri olayları).

/** Sözleşmedeki hatırlatma aralığı (NotificationSettings.reminderOffsetsMinutes): 30 dk – 3 gün, en fazla 3. */
export const reminderBounds = { min: 30, max: 4320, count: 3 } as const;

/** "Ekle" seçenekleri; kayıtlı değer listede yoksa çipte yine adıyla görünür. */
export const reminderChoices = [4320, 2880, 1440, 240, 180, 120, 60, 30] as const;

/** 1440 → "1 gün önce", 120 → "2 saat önce", 90 → "90 dk önce". */
export function reminderLabel(minutes: number): string {
  if (minutes % 1440 === 0) {
    return `${String(minutes / 1440)} gün önce`;
  }

  if (minutes % 60 === 0) {
    return `${String(minutes / 60)} saat önce`;
  }

  return `${String(minutes)} dk önce`;
}

type EventCode = NotificationTemplateSetting['eventCode'];

// Satır adları istemcide eventCode + variant'tan üretilir (manageGetNotificationSettings açıklaması).
const eventNames: Record<EventCode, string> = {
  'appointment.confirmed': 'Randevu onaylandı',
  'appointment.requested': 'Talep alındı',
  'appointment.approved': 'Talep onaylandı',
  'appointment.approval_expired': 'Talep zamanında yanıtlanmadı',
  'appointment.declined': 'Talep onaylanmadı',
  'appointment.reminder': 'Hatırlatma',
  'appointment.rescheduled': 'Saat değişti',
  'appointment.cancelled_by_salon': 'Salon iptal etti',
  'waitlist.offer': 'Bekleme listesi teklifi',
  'review.invite': 'Değerlendirme daveti',
};

const variantNames: Record<string, string> = { same_day: 'aynı gün' };

export function templateName(template: Pick<NotificationTemplateSetting, 'eventCode' | 'variant'>) {
  const base = eventNames[template.eventCode];
  if (template.variant === null) {
    return base;
  }

  return `${base} (${variantNames[template.variant] ?? template.variant})`;
}

export function templateKey(template: Pick<NotificationTemplateSetting, 'eventCode' | 'variant'>) {
  return `${template.eventCode}:${template.variant ?? ''}`;
}

/** Metindeki değişkenler: "{salon}: {tarih}" → ["salon", "tarih"]. */
export function templateVariables(body: string): string[] {
  return [...body.matchAll(/\{([^{}]+)\}/g)].map((match) => match[1] ?? '');
}

/** Salon metninin istemci denetimi; 155 birim sınırı en uzun değerlerle sunucuda ölçülür (too_long). */
export function templateProblem(
  body: string,
  template: Pick<NotificationTemplateSetting, 'variables' | 'requiredVariables'>,
): string | null {
  if (body.trim() === '') {
    return 'Metni yazın veya sistem şablonuna dönün.';
  }

  if (body.length > 300) {
    return 'Metin en fazla 300 karakter olabilir.';
  }

  const used = templateVariables(body);
  const unknown = used.find((name) => !template.variables.includes(name));
  if (unknown !== undefined) {
    return `{${unknown}} kullanılamaz. Listedeki değişkenleri kullanın.`;
  }

  const missing = template.requiredVariables.find((name) => !used.includes(name));
  if (missing !== undefined) {
    return `{${missing}} değişkeni metinde olmalı.`;
  }

  return null;
}

/** Sunucunun şablon hatası (400 validation_failed, templates[i].customBody). */
export function templateServerProblem(code: string | undefined, maxLength: number): string {
  switch (code) {
    case 'too_long':
      return `Metin en uzun değerlerle 1 SMS'i (${String(maxLength)} karakter) aşıyor. Kısaltın.`;
    case 'required':
      return 'Zorunlu değişken metinde yok.';
    default:
      return 'Metinde kullanılamayan bir değişken var.';
  }
}

const latin: Record<string, string> = { Ç: 'C', Ğ: 'G', İ: 'I', I: 'I', Ö: 'O', Ş: 'S', Ü: 'U' };

/**
 * Gönderen adı girişini operatör biçimine çevirir: büyük harf, Türkçe harfler Latin karşılığına (Ç→C, Ğ→G, İ→I, Ö→O,
 * Ş→S, Ü→U), boşluk ve noktalama atılır (manageRequestSmsSender açıklaması).
 */
export function normalizeSenderName(input: string): string {
  return input
    .toLocaleUpperCase('tr-TR')
    .replace(/[ÇĞİÖŞÜ]/g, (char) => latin[char] ?? char)
    .replace(/[^A-Z0-9]/g, '');
}

/** SmsSenderRequest.name deseni: 3–11 karakter, büyük Latin harf ve rakam, en az bir harf. */
export const senderNamePattern = /^(?=.*[A-Z])[A-Z0-9]{3,11}$/;
