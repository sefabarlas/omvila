import { moneyInputValue, parseMoneyInput } from '@omvila/ui-web/format';
import { z } from 'zod';
import type { DepositRule, Service, ServiceStage, ServiceWrite } from './api';

// Sınırlar sözleşmeden (ServiceWrite, ServiceStage, DepositRule).
const minutes = (min: number, max: number, message: string) =>
  z
    .string()
    .trim()
    .refine((v) => {
      const n = Number(v);
      return v !== '' && Number.isInteger(n) && n >= min && n <= max;
    }, message);

const optionalWhole = (min: number, max: number, message: string) =>
  z
    .string()
    .trim()
    .refine((v) => {
      if (v === '') {
        return true;
      }

      const n = Number(v);
      return Number.isInteger(n) && n >= min && n <= max;
    }, message);

const money = (message: string, optional = false) =>
  z
    .string()
    .trim()
    .refine((v) => (optional && v === '') || parseMoneyInput(v) !== null, message);

export const serviceSchema = z
  .object({
    name: z.string().trim().min(1, 'Hizmet adını girin.').max(120),
    categoryId: z.string().min(1, 'Önce bir kategori ekleyin.'),
    duration: z.string().trim(),
    price: money('Fiyatı girin: 450 veya 85,50 gibi.'),
    branchPrice: money('Fiyatı kontrol edin: 550 gibi.', true),
    priceDisplay: z.enum(['Fixed', 'StartingFrom']),
    description: z.string().trim().max(1000),
    staffIds: z.array(z.string()),
    depositMode: z.enum(['Inherit', 'Always', 'Never']),
    depositKind: z.enum(['amount', 'percent']),
    depositValue: z.string().trim(),
    isOnlineBookable: z.boolean(),
    minNoticeHours: optionalWhole(0, 168, 'En fazla 168 saat.'),
    maxAdvanceDays: optionalWhole(1, 365, '1 ile 365 gün arasında olmalı.'),
    bufferAfter: optionalWhole(0, 120, 'En fazla 120 dakika.'),
    approval: z.enum(['inherit', 'yes', 'no']),
    stages: z.array(
      z.object({ name: z.string(), durationMinutes: z.number(), staffBusy: z.boolean() }),
    ),
  })
  .superRefine((v, ctx) => {
    if (v.stages.length === 0) {
      const check = minutes(5, 720, 'Süre 5 ile 720 dakika arasında olmalı.').safeParse(v.duration);
      if (!check.success) {
        ctx.addIssue({
          code: 'custom',
          path: ['duration'],
          message: 'Süre 5 ile 720 dakika arasında olmalı.',
        });
      }
    }

    if (v.depositMode === 'Always') {
      const ok =
        v.depositKind === 'amount'
          ? parseMoneyInput(v.depositValue) !== null && parseMoneyInput(v.depositValue) !== 0
          : Number.isInteger(Number(v.depositValue)) &&
            Number(v.depositValue) >= 1 &&
            Number(v.depositValue) <= 100;
      if (!ok) {
        ctx.addIssue({
          code: 'custom',
          path: ['depositValue'],
          message:
            v.depositKind === 'amount'
              ? 'Kapora tutarını girin.'
              : 'Yüzde 1 ile 100 arasında olmalı.',
        });
      }
    }
  });

export type ServiceFormValues = z.infer<typeof serviceSchema>;

/** Aşama varsa süre aşamaların toplamıdır (kabul kriteri 7: 45 + 40 + 35 = 120). */
export function totalMinutes(values: Pick<ServiceFormValues, 'duration' | 'stages'>): number {
  return values.stages.length > 0
    ? values.stages.reduce((sum, stage) => sum + stage.durationMinutes, 0)
    : Number(values.duration);
}

export interface FormDefaults {
  categoryId: string;
  staffIds: string[];
}

export function emptyServiceForm({ categoryId, staffIds }: FormDefaults): ServiceFormValues {
  return {
    name: '',
    categoryId,
    duration: '',
    price: '',
    branchPrice: '',
    priceDisplay: 'Fixed',
    description: '',
    staffIds,
    depositMode: 'Inherit',
    depositKind: 'amount',
    depositValue: '',
    isOnlineBookable: true,
    minNoticeHours: '',
    maxAdvanceDays: '',
    bufferAfter: '',
    approval: 'inherit',
    stages: [],
  };
}

/** Düzenlenen hizmet (genel kayıt) → form; şube fiyatı ayrıca verilir. */
export function serviceToForm(service: Service, branchPrice: string): ServiceFormValues {
  const { deposit } = service;
  return {
    name: service.name,
    categoryId: service.categoryId,
    duration: String(service.durationMinutes),
    price: moneyInputValue(service.price.amountMinor),
    branchPrice,
    priceDisplay: service.priceDisplay,
    description: service.description ?? '',
    staffIds: service.staffIds,
    depositMode: deposit.mode,
    depositKind: deposit.percent !== undefined ? 'percent' : 'amount',
    depositValue: deposit.amount
      ? moneyInputValue(deposit.amount.amountMinor)
      : deposit.percent !== undefined
        ? String(deposit.percent)
        : '',
    isOnlineBookable: service.isOnlineBookable,
    minNoticeHours:
      service.minNoticeMinutes != null ? String(Math.round(service.minNoticeMinutes / 60)) : '',
    maxAdvanceDays: service.maxAdvanceDays != null ? String(service.maxAdvanceDays) : '',
    bufferAfter: service.bufferAfterMinutes > 0 ? String(service.bufferAfterMinutes) : '',
    approval:
      service.requiresApproval == null ? 'inherit' : service.requiresApproval ? 'yes' : 'no',
    stages: service.stages,
  };
}

function depositRule(v: ServiceFormValues): DepositRule {
  if (v.depositMode !== 'Always') {
    return { mode: v.depositMode };
  }

  return v.depositKind === 'amount'
    ? {
        mode: 'Always',
        amount: { amountMinor: parseMoneyInput(v.depositValue) ?? 0, currency: 'TRY' },
      }
    : { mode: 'Always', percent: Number(v.depositValue) };
}

/** Form → yazma gövdesi. Boş bırakılan gelişmiş alanlar genel ayarı kullanır (null). */
export function formToWrite(v: ServiceFormValues, base?: Service): ServiceWrite {
  const stages: ServiceStage[] = v.stages;
  return {
    categoryId: v.categoryId,
    name: v.name,
    description: v.description === '' ? null : v.description,
    durationMinutes: totalMinutes(v),
    price: { amountMinor: parseMoneyInput(v.price) ?? 0, currency: 'TRY' },
    priceDisplay: v.priceDisplay,
    bufferBeforeMinutes: base?.bufferBeforeMinutes ?? 0,
    bufferAfterMinutes: v.bufferAfter === '' ? 0 : Number(v.bufferAfter),
    deposit: depositRule(v),
    requiresApproval: v.approval === 'inherit' ? null : v.approval === 'yes',
    minNoticeMinutes: v.minNoticeHours === '' ? null : Number(v.minNoticeHours) * 60,
    maxAdvanceDays: v.maxAdvanceDays === '' ? null : Number(v.maxAdvanceDays),
    resourceTypeId: base?.resourceTypeId ?? null,
    isOnlineBookable: v.isOnlineBookable,
    isActive: base?.isActive ?? true,
    ...(base?.sortOrder !== undefined ? { sortOrder: base.sortOrder } : {}),
    stages,
    staffIds: v.staffIds,
  };
}
