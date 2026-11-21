import { zodResolver } from '@hookform/resolvers/zod';
import { formatIban, formatMoney, isValidIban, normalizeIban } from '@omvila/ui-web/format';
import { Banner, Select, TextField, useToast } from '@omvila/ui-web/panel';
import { useSuspenseQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { useForm, useWatch } from 'react-hook-form';
import { z } from 'zod';
import { useApi } from '@/app/api-context';
import { ConflictBanner } from '@/shared/ConflictBanner';
import { bankAccountQuery, transferWindowOptions, useUpdateBankAccount } from './bank-account-api';
import type { BankAccount, BankAccountWrite, TransferWindowMinutes } from './bank-account-api';
import { describeSaveError } from './save-error';
import styles from './Settings.module.css';
import { SettingsCard, SettingsLayout } from './SettingsLayout';

const formId = 'kapora-ve-iban';
const ibanMessage = 'IBAN’ı kontrol edin: TR ile başlayan 26 karakter olmalı.';
const ibanRequired = 'IBAN girin: TR ile başlayan 26 karakter.';
// Önizlemedeki örnek tutar ve kod: gerçek tutar hizmetin kaporasından, kod randevuya özel üretilir (acik-sorular.md T-79).
const previewAmount = { amountMinor: 15000, currency: 'TRY' };
const previewCode = '7K2X';
const windowValues = transferWindowOptions.map((o) => String(o.value)) as [string, ...string[]];

function schema(account: BankAccount) {
  return z
    .object({
      accountHolderName: z
        .string()
        .trim()
        .min(2, 'Alıcı adını girin: hesap sahibinin adı.')
        .max(120, 'Alıcı adı en fazla 120 karakter olabilir.'),
      bankName: z
        .string()
        .trim()
        .min(2, 'Banka adını girin.')
        .max(80, 'Banka adı en fazla 80 karakter olabilir.'),
      iban: z.string(),
      transferCodePrefix: z
        .string()
        .trim()
        .regex(/^[A-Z]{2,5}$/, 'Önek 2–5 Latin harfi olmalı: OMV'),
      transferWindowMinutes: z.enum(windowValues),
    })
    .superRefine((values, ctx) => {
      const changed = newIban(account, values.iban);
      if (changed === undefined && !account.configured) {
        ctx.addIssue({ code: 'custom', path: ['iban'], message: ibanRequired });
      } else if (changed !== undefined && !isValidIban(changed)) {
        ctx.addIssue({ code: 'custom', path: ['iban'], message: ibanMessage });
      }
    });
}

type FormValues = z.infer<ReturnType<typeof schema>>;

/** Yeni yazılmış IBAN; alan boşsa veya kayıtlı maskeli değer duruyorsa yok (kayıtlı IBAN korunur). */
function newIban(account: BankAccount, value: string): string | undefined {
  const typed = value.trim();
  return typed === '' || typed === account.ibanMasked ? undefined : typed;
}

function toForm(account: BankAccount): FormValues {
  return {
    accountHolderName: account.accountHolderName ?? '',
    bankName: account.bankName ?? '',
    iban: account.ibanMasked ?? '',
    transferCodePrefix: account.transferCodePrefix,
    transferWindowMinutes: String(account.transferWindowMinutes),
  };
}

function toBody(account: BankAccount, values: FormValues): BankAccountWrite {
  const iban = newIban(account, values.iban);
  return {
    accountHolderName: values.accountHolderName,
    bankName: values.bankName,
    ...(iban === undefined ? {} : { iban: normalizeIban(iban) }),
    transferCodePrefix: values.transferCodePrefix,
    transferWindowMinutes: Number(values.transferWindowMinutes) as TransferWindowMinutes,
  };
}

/** Önizlemede dar alan: "TR29 … 6789 01" (sözleşme: "••••" yerine "…" gösterilebilir). */
function previewIban(account: BankAccount, value: string): string {
  const typed = newIban(account, value);
  if (typed !== undefined && isValidIban(typed)) {
    const groups = formatIban(typed).split(' ');
    return `${groups[0] ?? ''} … ${groups.slice(-2).join(' ')}`;
  }

  return account.ibanMasked?.replace('••••', '…') ?? '—';
}

/** Sunucunun alan hataları (400 validation_failed) → alanın altında Türkçe metin. */
const fieldMessages: Partial<Record<keyof FormValues, (code: string) => string>> = {
  iban: (code) => (code === 'required' ? ibanRequired : ibanMessage),
  accountHolderName: () => 'Alıcı adını kontrol edin.',
  bankName: () => 'Banka adını kontrol edin.',
  transferCodePrefix: () => 'Önek 2–5 Latin harfi olmalı: OMV',
};

/**
 * Ayarlar → Kapora ve IBAN (salon-paneli/AyarlarOdeme). OMVILA ödeme almaz: hesap yalnız müşteriye gösterilir.
 * Tam IBAN sunucudan dönmez; alan kayıtlı maskeli değerle açılır, yalnız yeni IBAN yazılırsa gönderilir.
 */
export function BankAccountPage() {
  const api = useApi();
  const toast = useToast();
  const query = useSuspenseQuery(bankAccountQuery(api));
  const account = query.data;
  const update = useUpdateBankAccount();
  const [conflict, setConflict] = useState(false);
  const form = useForm<FormValues>({
    resolver: zodResolver(schema(account)),
    defaultValues: toForm(account),
  });
  const { errors } = form.formState;
  const [holder, iban, prefix] = useWatch({
    control: form.control,
    name: ['accountHolderName', 'iban', 'transferCodePrefix'],
  });
  const prefixField = form.register('transferCodePrefix');
  const ibanField = form.register('iban');

  const onSubmit = form.handleSubmit(async (values) => {
    try {
      const saved = await update.mutateAsync({
        version: account.version,
        body: toBody(account, values),
      });
      form.reset(toForm(saved));
      setConflict(false);
      toast.show({ tone: 'basari', title: 'Kapora hesabı kaydedildi' });
    } catch (error) {
      const view = describeSaveError(error);
      setConflict(view.conflict);
      for (const [field, codes] of Object.entries(view.fields)) {
        const message = fieldMessages[field as keyof FormValues];
        if (message) {
          form.setError(field as keyof FormValues, { message: message(codes[0] ?? '') });
        }
      }

      if (!view.conflict) {
        toast.show({ tone: 'hata', title: view.message });
      }
    }
  });

  const reload = async () => {
    const fresh = await query.refetch();
    if (fresh.data) {
      form.reset(toForm(fresh.data));
    }

    setConflict(false);
  };

  const previewPrefix = /^[A-Z]{2,5}$/.test(prefix) ? prefix : account.transferCodePrefix;
  return (
    <SettingsLayout
      context="Kapora ve IBAN"
      formId={formId}
      saving={update.isPending}
      saveLabel="Kaydet"
    >
      {conflict ? <ConflictBanner onReload={() => void reload()} /> : null}
      <Banner tone="bilgi" title="OMVILA ödeme almaz.">
        Kaporayı müşteri doğrudan sizin IBAN’ınıza havale eder. Rezervasyon sırasında müşteriye bu
        hesap, tutar ve kişiye özel açıklama kodu gösterilir; havaleyi görünce randevuda
        işaretlersiniz.
      </Banner>
      <div className={styles.paymentLayout}>
        <form id={formId} noValidate onSubmit={(e) => void onSubmit(e)}>
          <SettingsCard title="Kaporanın gönderileceği hesap">
            <div className={styles.grid}>
              <TextField
                label="Alıcı adı"
                autoComplete="name"
                maxLength={120}
                hint="Müşterinin bankasında görünecek ad; hesap sahibiyle aynı olmalı"
                error={errors.accountHolderName?.message}
                {...form.register('accountHolderName')}
              />
              <TextField
                label="Banka"
                placeholder="Ziraat Bankası"
                maxLength={80}
                error={errors.bankName?.message}
                {...form.register('bankName')}
              />
              <TextField
                className={styles.full}
                label="IBAN"
                inputMode="text"
                autoComplete="off"
                spellCheck={false}
                placeholder="TR00 0000 0000 0000 0000 0000 00"
                maxLength={40}
                hint={
                  account.configured
                    ? 'Kayıtlı IBAN maskeli görünür; değiştirmek için yenisini yazın. Müşteriye tam hali gösterilir.'
                    : 'Müşteriye tam hali gösterilir; panelde yalnızca sahip ve yöneticiler görür'
                }
                error={errors.iban?.message}
                {...ibanField}
                onFocus={(event) => {
                  // Maskeli değer duruyorsa yazılan yeni IBAN onun yerine geçsin.
                  if (event.currentTarget.value === account.ibanMasked) {
                    event.currentTarget.select();
                  }
                }}
              />
              <TextField
                label="Açıklama kodu öneki"
                maxLength={5}
                autoComplete="off"
                hint={`Her randevuya ${previewPrefix}-${previewCode} gibi ayrı kod üretilir`}
                error={errors.transferCodePrefix?.message}
                {...prefixField}
                onChange={(event) => {
                  event.target.value = event.target.value.toLocaleUpperCase('en-US');
                  void prefixField.onChange(event);
                }}
              />
              <Select
                label="Havale süresi"
                options={transferWindowOptions.map((o) => ({
                  value: String(o.value),
                  label: o.label,
                }))}
                {...form.register('transferWindowMinutes')}
              />
            </div>
          </SettingsCard>
        </form>
        <SettingsCard title="Müşteri ne görür?">
          <dl className={styles.preview}>
            <div className={styles.previewRow}>
              <dt>Alıcı</dt>
              <dd>{holder.trim() === '' ? '—' : holder}</dd>
            </div>
            <div className={styles.previewRow}>
              <dt>IBAN</dt>
              <dd>{previewIban(account, iban)}</dd>
            </div>
            <div className={styles.previewRow}>
              <dt>Tutar</dt>
              <dd>{formatMoney(previewAmount)}</dd>
            </div>
            <div className={styles.previewRow}>
              <dt>Açıklama</dt>
              <dd>{`${previewPrefix}-${previewCode}`}</dd>
            </div>
          </dl>
          <p className={styles.cardText}>
            Aynı bilgiler SMS ile de gider. Müşteri “Havaleyi yaptım” derse size bildirim düşer;
            asıl onay sizin işaretinizdir.
          </p>
        </SettingsCard>
      </div>
    </SettingsLayout>
  );
}
