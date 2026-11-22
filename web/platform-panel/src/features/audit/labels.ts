import type { components } from '@omvila/api-client-ts';
import { formatCount } from '@omvila/ui-web/format';
import { roleLabels } from '@/app/session';
import { signedCount, subscriptionLabels } from '@/features/brands/labels';
import type { SubscriptionStatus } from '@/features/brands/labels';

// Denetim kaydı satırının metinleri (adminListAuditLog: işlem adı istemcide action + entityType + changes'ten üretilir).
// Marka detayı "Denetim kaydı" sekmesi ve PDenetim ortak kullanır.
export type AuditEntry = components['schemas']['AuditLogEntry'];

const sectionLabels: Record<string, string> = {
  Summary: 'Özet',
  Invoices: 'Abonelik ve fatura',
  Credits: 'Kontör',
  Payments: 'Ödemeler',
  Audit: 'Denetim kaydı',
  Receipt: 'Dekont',
};

const str = (value: unknown): string | undefined =>
  typeof value === 'string' && value !== '' ? value : undefined;
const num = (value: unknown): number | undefined =>
  typeof value === 'number' && Number.isFinite(value) ? value : undefined;

function isStatus(value: unknown): value is SubscriptionStatus {
  return typeof value === 'string' && value in subscriptionLabels;
}

/** "İşlem": "Marka görüntülendi", "Havale eşleştirildi", "Salt okunura geçti"… */
export function auditActionLabel(entry: AuditEntry): string {
  const { action, entityType, changes } = entry;
  const status = str(changes.status);
  switch (action) {
    case 'View':
      if (entityType === 'PaymentNotice') {
        return changes.section === 'Receipt' ? 'Dekont açıldı' : 'Havale bildirimi görüntülendi';
      }

      return entityType === 'Brand' ? 'Marka görüntülendi' : 'Görüntülendi';
    case 'Match':
      return 'Havale eşleştirildi';
    case 'Approve':
      return entityType === 'BrandSmsSender' ? 'SMS başlığı onaylandı' : 'Onaylandı';
    case 'Export':
      return entityType === 'AuditLog' ? 'Denetim kaydı dışa aktarıldı' : 'Dışa aktarıldı';
    case 'Anonymize':
      return 'KVKK silme tamamlandı';
    case 'Delete':
      return 'Kayıt silindi';
    case 'Create':
      switch (entityType) {
        case 'CreditTransaction':
          return 'Kontör eklendi';
        case 'PlatformBankTransaction':
          return 'Banka hareketi eklendi';
        case 'SubscriptionInvoice':
          return 'Fatura kesildi';
        default:
          return 'Kayıt oluşturuldu';
      }
    case 'Update':
      switch (entityType) {
        case 'Subscription':
          if (status === 'ReadOnly') return 'Salt okunura geçti';
          if (status === 'PastDue') return 'Ödeme gecikti';
          if (status === 'Active') return 'Abonelik açıldı';
          return str(changes.planCode) || str(changes.planName)
            ? 'Plan değişti'
            : 'Abonelik değişti';
        case 'PaymentNotice':
          return status === 'Rejected' ? 'Havale bildirimi reddedildi' : 'Havale bulunamadı';
        case 'BrandSmsSender':
          return 'SMS başlığı reddedildi';
        case 'Brand':
          return status === 'Suspended' ? 'Marka askıya alındı' : 'Marka durumu değişti';
        case 'Plan':
          return 'Plan tanımı değişti';
        case 'CreditPackage':
          return 'Kontör paketi değişti';
        default:
          return 'Kayıt güncellendi';
      }
  }
}

/** "Ayrıntı": bilinen alanlardan kısa metin ("Özet sekmesi", "5.000 SMS kontörü · KNT-SM-0412"); değerler maskeli gelir. */
export function auditDetail(entry: AuditEntry): string {
  const c = entry.changes;
  const section = str(c.section);
  if (entry.action === 'View' && section) {
    return `${sectionLabels[section] ?? section} sekmesi`;
  }

  const parts: string[] = [];
  const credits = num(c.credits);
  if (credits !== undefined) {
    parts.push(
      entry.action === 'Match'
        ? `${formatCount(credits)} SMS kontörü`
        : `${signedCount(credits)} SMS`,
    );
  }

  const status = c.status;
  const previous = c.previousStatus;
  if (entry.entityType === 'Subscription' && isStatus(status)) {
    parts.push(
      isStatus(previous)
        ? `${subscriptionLabels[previous]} → ${subscriptionLabels[status]}`
        : subscriptionLabels[status],
    );
  }

  for (const key of ['planName', 'transferCode', 'name', 'reason', 'message', 'fileName']) {
    const value = str(c[key]);
    if (value) parts.push(value);
  }

  const count = num(c.count) ?? num(c.imported);
  if (count !== undefined && credits === undefined) {
    parts.push(`${formatCount(count)} kayıt`);
  }

  return parts.length > 0 ? parts.join(' · ') : '—';
}

/** "Kim" iki satırı: ad ve rol ("Ayşe Demir" / "Destek"; "Sistem" / "Otomatik"). */
export function auditActor(actor: AuditEntry['actor']): { name: string; role: string } {
  switch (actor.type) {
    case 'System':
      return { name: 'Sistem', role: 'Otomatik' };
    case 'PlatformUser':
      return {
        name: actor.name ?? '—',
        role: actor.platformRole ? roleLabels[actor.platformRole] : 'Platform',
      };
    case 'User':
      return { name: actor.name ?? '—', role: 'Salon kullanıcısı' };
    case 'Customer':
      return { name: actor.name ?? '—', role: 'Müşteri' };
  }
}

/** Tek satır "Kim" (marka detayı sekmesi): "Ayşe Demir · Destek", "Sistem · Otomatik". */
export function auditActorLabel(actor: AuditEntry['actor']): string {
  const { name, role } = auditActor(actor);
  return `${name} · ${role}`;
}
