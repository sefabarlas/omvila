import { zodResolver } from '@hookform/resolvers/zod';
import { formatPhone } from '@omvila/ui-web/format';
import { Select, TextField, useToast } from '@omvila/ui-web/panel';
import { useSuspenseQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { useApi } from '@/app/api-context';
import { industries } from '@/shared/industries';
import type { IndustryCode } from '@/shared/industries';
import { brandQuery, useUpdateBrand } from './api';
import type { Brand, BrandUpdate } from './api';
import { ConflictBanner } from '@/shared/ConflictBanner';
import { describeSaveError } from './save-error';
import styles from './Settings.module.css';
import { SettingsCard, SettingsLayout } from './SettingsLayout';

const formId = 'isletme-bilgileri';
const industryCodes = industries.map((i) => i.value) as [IndustryCode, ...IndustryCode[]];

// Uzunluk sınırları sözleşmeden (BrandUpdate).
const schema = z.object({
  name: z.string().trim().min(2, 'İşletme adı en az 2 karakter olmalı.').max(160),
  industryCode: z.enum(industryCodes),
  shortDescription: z.string().trim().max(160, 'Kısa tanıtım en fazla 160 karakter olabilir.'),
  phone: z.string().trim(),
  email: z.union([z.literal(''), z.email('E-posta adresini kontrol edin: ad@alan.com')]),
  instagram: z.string().trim().max(60),
  website: z.string().trim().max(200),
  legalName: z.string().trim().max(200),
  taxOffice: z.string().trim().max(80),
  taxNumber: z.string().trim().max(20),
});

type FormValues = z.infer<typeof schema>;

const fieldMessages: Partial<Record<keyof FormValues, string>> = {
  phone: 'Telefon numarasını kontrol edin: 0216 345 12 12',
  email: 'E-posta adresini kontrol edin: ad@alan.com',
};

function toForm(brand: Brand): FormValues {
  return {
    name: brand.name,
    industryCode: brand.industryCode,
    shortDescription: brand.shortDescription ?? '',
    phone: brand.phone ? formatPhone(brand.phone) : '',
    email: brand.email ?? '',
    instagram: brand.instagram ?? '',
    website: brand.website ?? '',
    legalName: brand.legalName ?? '',
    taxOffice: brand.taxOffice ?? '',
    taxNumber: brand.taxNumber ?? '',
  };
}

// Boş alan "yok" demektir (null); telefonu sunucu E.164'e çevirir ve doğrular.
function toBody(values: FormValues): BrandUpdate {
  const orNull = (value: string) => (value === '' ? null : value);
  return {
    name: values.name,
    industryCode: values.industryCode,
    shortDescription: orNull(values.shortDescription),
    phone: orNull(values.phone),
    email: orNull(values.email),
    instagram: orNull(values.instagram),
    website: orNull(values.website),
    legalName: orNull(values.legalName),
    taxOffice: orNull(values.taxOffice),
    taxNumber: orNull(values.taxNumber),
  };
}

/** Ayarlar → İşletme bilgileri (salon-paneli/AyarlarIsletme). Logo yükleme sonra (acik-sorular.md T-41). */
export function BusinessSettingsPage() {
  const api = useApi();
  const toast = useToast();
  const query = useSuspenseQuery(brandQuery(api));
  const brand = query.data;
  const update = useUpdateBrand();
  const [conflict, setConflict] = useState(false);
  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: toForm(brand),
  });
  const { errors } = form.formState;

  const onSubmit = form.handleSubmit(async (values) => {
    try {
      const saved = await update.mutateAsync({ version: brand.version, body: toBody(values) });
      form.reset(toForm(saved));
      setConflict(false);
      toast.show({ tone: 'basari', title: 'Değişiklikler kaydedildi' });
    } catch (error) {
      const view = describeSaveError(error);
      setConflict(view.conflict);
      for (const field of Object.keys(view.fields)) {
        if (field in fieldMessages) {
          const key = field as keyof FormValues;
          form.setError(key, { message: fieldMessages[key] ?? view.message });
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

  return (
    <SettingsLayout context="Bütün şubeler için ortak" formId={formId} saving={update.isPending}>
      {conflict ? <ConflictBanner onReload={() => void reload()} /> : null}
      <form id={formId} className={styles.sections} noValidate onSubmit={(e) => void onSubmit(e)}>
        <SettingsCard title="Marka">
          <div className={styles.grid}>
            <TextField
              label="İşletme adı"
              autoComplete="organization"
              error={errors.name?.message}
              {...form.register('name')}
            />
            <Select label="Kategori" options={industries} {...form.register('industryCode')} />
            <TextField
              className={styles.full}
              label="Kısa tanıtım"
              placeholder="Saç, bakım ve makyajda 12 yıllık deneyim."
              hint="Rezervasyon sayfasının üstünde görünür · en fazla 160 karakter"
              maxLength={160}
              error={errors.shortDescription?.message}
              {...form.register('shortDescription')}
            />
          </div>
        </SettingsCard>
        <SettingsCard title="İletişim" description="Şube adres ve telefonları Şubeler sayfasında.">
          <div className={styles.grid}>
            <TextField
              label="İşletme telefonu"
              type="tel"
              inputMode="tel"
              autoComplete="tel"
              placeholder="0216 345 12 12"
              error={errors.phone?.message}
              {...form.register('phone')}
            />
            <TextField
              label="E-posta"
              type="email"
              autoComplete="email"
              placeholder="merhaba@kuaforelif.com"
              error={errors.email?.message}
              {...form.register('email')}
            />
            <TextField
              label="Instagram"
              placeholder="@kuaforelif"
              maxLength={60}
              {...form.register('instagram')}
            />
            <TextField
              label="Web sitesi"
              type="url"
              placeholder="https://"
              maxLength={200}
              {...form.register('website')}
            />
          </div>
        </SettingsCard>
        <SettingsCard
          title="Fatura bilgileri"
          description="OMVILA aboneliğinizin faturası bu bilgilerle kesilir."
        >
          <div className={styles.grid}>
            <TextField
              className={styles.full}
              label="Unvan"
              maxLength={200}
              {...form.register('legalName')}
            />
            <TextField label="Vergi dairesi" maxLength={80} {...form.register('taxOffice')} />
            <TextField
              label="Vergi numarası"
              inputMode="numeric"
              maxLength={20}
              {...form.register('taxNumber')}
            />
          </div>
        </SettingsCard>
      </form>
    </SettingsLayout>
  );
}
