import { ANY_STAFF } from '@omvila/ui-web/booking';
import type { Catalog } from './booking-api';

// Akışın seçim durumu ve URL karşılığı (web/rezervasyon/AGENTS.md: `?branch=…&services=a,b&staff=…&date=…&time=…`).
// Kişisel veri (ad, telefon, not) ve rezervasyon jetonu URL'ye yazılmaz; yalnız bellekte tutulur.

export const steps = [
  'branch',
  'services',
  'staff',
  'time',
  'details',
  'code',
  'confirm',
  'done',
] as const;
export type Step = (typeof steps)[number];

/** Online randevuda en çok kalem (sözleşme: items maxItems 5). */
export const maxServices = 5;

export interface Selection {
  step: Step;
  branchId: string | null;
  serviceIds: string[];
  /** "İki hizmet de aynı kişiden" (tek hizmette her zaman true). */
  sameStaff: boolean;
  /** sameStaff: [kimlik | ANY_STAFF]; değilse serviceIds ile aynı sırada. Boş dize = seçilmedi. */
  staff: string[];
  /** Şubenin takvim günü, yyyy-aa-gg. */
  date: string | null;
  /** Şubenin yerel saati, "14:30". */
  time: string | null;
}

export const emptySelection: Selection = {
  step: 'services',
  branchId: null,
  serviceIds: [],
  sameStaff: true,
  staff: [],
  date: null,
  time: null,
};

const datePattern = /^\d{4}-\d{2}-\d{2}$/;
const timePattern = /^([01]\d|2[0-3]):[0-5]\d$/;
const idPattern = /^[0-9a-f-]{36}$/i;

const list = (value: string | null) => (value ? value.split(',').filter(Boolean) : []);

/** Adres çubuğundan seçim; tanınmayan değerler atılır (kataloğa göre ayrıca `reconcile`). */
export function parseSelection(search: string): Selection {
  const params = new URLSearchParams(search);
  const step = params.get('step');
  const date = params.get('date');
  const time = params.get('time');
  const branch = params.get('branch');
  return {
    step: steps.includes(step as Step) ? (step as Step) : 'services',
    branchId: branch && idPattern.test(branch) ? branch : null,
    serviceIds: list(params.get('services')).filter((id) => idPattern.test(id)),
    sameStaff: params.get('same') !== '0',
    staff: list(params.get('staff')).map((id) =>
      id === ANY_STAFF || idPattern.test(id) ? id : '',
    ),
    date: date && datePattern.test(date) ? date : null,
    time: time && timePattern.test(time) ? time : null,
  };
}

/**
 * Seçimin adres karşılığı ("?branch=…&services=…"); boş alanlar yazılmaz. Başarı adımı adres taşımaz.
 * Tek şubeli markada şube yazılmaz (`includeBranch` false).
 */
export function serializeSelection(selection: Selection, includeBranch = true): string {
  if (selection.step === 'done') return '';
  const params = new URLSearchParams();
  if (selection.step !== 'services') params.set('step', selection.step);
  if (includeBranch && selection.branchId) params.set('branch', selection.branchId);
  if (selection.serviceIds.length > 0) params.set('services', selection.serviceIds.join(','));
  if (!selection.sameStaff) params.set('same', '0');
  if (selection.staff.some(Boolean)) params.set('staff', selection.staff.join(','));
  if (selection.date) params.set('date', selection.date);
  if (selection.time) params.set('time', selection.time);
  const query = params.toString();
  return query ? `?${query}` : '';
}

/** Personel adımı gösterilir mi (marka ayarı; kapalıysa adım atlanır). */
export const hasStaffStep = (catalog: Catalog) => catalog.allowStaffSelection;

/** Aynı kişi anahtarı yalnız ≥ 2 hizmette ve personel seçimi açıkken anlamlıdır. */
export function effectiveSameStaff(selection: Selection, catalog: Catalog): boolean {
  return !catalog.allowStaffSelection || selection.serviceIds.length < 2 || selection.sameStaff;
}

/** Seçilen hizmetlerin hepsini veren personel (aynı kişi) veya tek hizmeti verenler. */
export function staffFor(catalog: Catalog, serviceIds: readonly string[]) {
  const services = catalog.services.filter((service) => serviceIds.includes(service.id));
  return catalog.staff.filter((member) =>
    services.every((service) => service.staffIds.includes(member.id)),
  );
}

function validStaff(value: string, catalog: Catalog, serviceIds: readonly string[]): boolean {
  if (value === ANY_STAFF) return catalog.allowAnyStaffOption;
  return staffFor(catalog, serviceIds).some((member) => member.id === value);
}

/** Personel seçimi tamam mı (adım kapalıysa her zaman). */
export function staffComplete(selection: Selection, catalog: Catalog): boolean {
  if (!catalog.allowStaffSelection) return true;
  if (effectiveSameStaff(selection, catalog)) {
    const [value] = selection.staff;
    return value !== undefined && validStaff(value, catalog, selection.serviceIds);
  }
  return selection.serviceIds.every((serviceId, index) => {
    const value = selection.staff[index];
    return value !== undefined && validStaff(value, catalog, [serviceId]);
  });
}

/** Müsaitlik ve oluşturma isteğinin kalemleri; "fark etmez" ve kapalı personel seçimi null. */
export function itemRequests(selection: Selection, catalog: Catalog) {
  const same = effectiveSameStaff(selection, catalog);
  return selection.serviceIds.map((serviceId, index) => {
    const value = catalog.allowStaffSelection
      ? same
        ? selection.staff[0]
        : selection.staff[index]
      : undefined;
    return { serviceId, staffId: value && value !== ANY_STAFF ? value : null };
  });
}

/**
 * Kataloğa göre geçersiz seçimi temizler ve adımı ilk eksik adıma çeker (yenilemede ya da eski bağlantıda).
 * Bellek adımları (kod, onay) burada değil, akışta jeton/kod durumuna göre düzeltilir.
 */
export function reconcile(selection: Selection, catalog: Catalog): Selection {
  const known = new Set(catalog.services.map((service) => service.id));
  const serviceIds = [...new Set(selection.serviceIds.filter((id) => known.has(id)))].slice(
    0,
    maxServices,
  );
  const next: Selection = { ...selection, serviceIds };
  if (serviceIds.length !== selection.serviceIds.length) {
    next.staff = [];
    next.date = null;
    next.time = null;
  }
  const order = steps.indexOf(next.step);
  if (serviceIds.length === 0) return { ...next, step: 'services', date: null, time: null };
  if (!staffComplete(next, catalog)) {
    return {
      ...next,
      step: order > steps.indexOf('staff') ? 'staff' : next.step,
      date: null,
      time: null,
    };
  }
  if (next.step === 'staff' && !catalog.allowStaffSelection) return { ...next, step: 'time' };
  if (order > steps.indexOf('time') && (!next.date || !next.time)) return { ...next, step: 'time' };
  return next;
}

/** Hizmet listesi değişince personeli hizmet kimliğine göre korur; gün ve saat sıfırlanır. */
export function withServices(selection: Selection, serviceIds: string[]): Selection {
  const byService = new Map(selection.serviceIds.map((id, i) => [id, selection.staff[i] ?? '']));
  return {
    ...selection,
    serviceIds,
    staff: selection.sameStaff
      ? selection.staff.slice(0, 1)
      : serviceIds.map((id) => byService.get(id) ?? ''),
    date: null,
    time: null,
  };
}
