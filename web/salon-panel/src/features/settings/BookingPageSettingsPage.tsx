import { zodResolver } from '@hookform/resolvers/zod';
import { Button, Switch, TextField, useToast } from '@omvila/ui-web/panel';
import { useSuspenseQuery } from '@tanstack/react-query';
import { ExternalLink, Link2 } from 'lucide-react';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { useApi } from '@/app/api-context';
import { bookingPageQuery, displayUrl, useCopyBookingLink } from '@/shared/booking-link';
import { useUpdateBookingPage } from './api';
import type { BookingPageSettings, CoverColor } from './api';
import { ConflictBanner } from '@/shared/ConflictBanner';
import { describeSaveError } from './save-error';
import styles from './Settings.module.css';
import { SettingRow, SettingRows, SettingsCard, SettingsLayout } from './SettingsLayout';

const formId = 'rezervasyon-sayfasi';

const coverColors: readonly { value: CoverColor; label: string }[] = [
  { value: 'petrol', label: 'Petrol' },
  { value: 'mercan', label: 'Mercan' },
  { value: 'kum', label: 'Kum' },
];

const slugMessage = 'Küçük harf, rakam ve tire kullanın; 3–60 karakter.';

// Bağlantı adı kuralı sözleşmeden (BookingPageSettingsUpdate.slug).
const schema = z.object({
  slug: z
    .string()
    .trim()
    .regex(/^[a-z0-9-]{3,60}$/, slugMessage),
  coverColor: z.enum(['petrol', 'mercan', 'kum']),
  showReviews: z.boolean(),
  showPrices: z.boolean(),
  preAppointmentNote: z.string().trim().max(200, 'Not en fazla 200 karakter olabilir.'),
});

type FormValues = z.infer<typeof schema>;

function toForm(settings: BookingPageSettings): FormValues {
  return {
    slug: settings.slug,
    coverColor: settings.coverColor,
    showReviews: settings.showReviews,
    showPrices: settings.showPrices,
    preAppointmentNote: settings.preAppointmentNote ?? '',
  };
}

/**
 * Ayarlar → Rezervasyon sayfası (salon-paneli/AyarlarRezervasyon). Bağlantı adı değişirse eski bağlantı 30 gün
 * yönlendirilir. QR kod indirme ayrı bir PR'da (acik-sorular.md T-40).
 */
export function BookingPageSettingsPage() {
  const api = useApi();
  const toast = useToast();
  const copy = useCopyBookingLink();
  const query = useSuspenseQuery(bookingPageQuery(api));
  const settings = query.data;
  const update = useUpdateBookingPage();
  const [conflict, setConflict] = useState(false);
  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: toForm(settings),
  });
  const { errors } = form.formState;

  const onSubmit = form.handleSubmit(async (values) => {
    try {
      const saved = await update.mutateAsync({
        version: settings.version,
        body: {
          ...values,
          preAppointmentNote: values.preAppointmentNote === '' ? null : values.preAppointmentNote,
        },
      });
      form.reset(toForm(saved));
      setConflict(false);
      toast.show({ tone: 'basari', title: 'Değişiklikler kaydedildi' });
    } catch (error) {
      const view = describeSaveError(error);
      setConflict(view.conflict);
      if (view.conflict) {
        return;
      }

      if (view.code === 'slug_unavailable' || view.fields.slug) {
        form.setError('slug', {
          message: view.code === 'slug_unavailable' ? view.message : slugMessage,
        });
        return;
      }

      toast.show({ tone: 'hata', title: view.message });
    }
  });

  const reload = async () => {
    const fresh = await query.refetch();
    if (fresh.data) {
      form.reset(toForm(fresh.data));
    }

    setConflict(false);
  };

  const url = displayUrl(settings.url);

  return (
    <SettingsLayout context={url} formId={formId} saving={update.isPending}>
      {conflict ? <ConflictBanner onReload={() => void reload()} /> : null}
      <section className={styles.linkCard} aria-label="Rezervasyon bağlantısı">
        <span className={styles.linkIcon} aria-hidden="true">
          <Link2 size={20} />
        </span>
        <div className={styles.linkText}>
          <span className={styles.linkUrl}>{url}</span>
          <span className={styles.linkHint}>
            Instagram profilinize ve Google İşletme kaydınıza ekleyin.
          </span>
        </div>
        <div className={styles.linkActions}>
          <Button onClick={() => void copy(settings.url)}>Kopyala</Button>
          {/* Ekranda tek birincil eylem üst bardaki "Değişiklikleri kaydet" (salon-panel/AGENTS.md). */}
          <Button
            icon={<ExternalLink size={16} aria-hidden="true" />}
            onClick={() => window.open(settings.url, '_blank', 'noopener')}
          >
            Sayfayı aç
          </Button>
        </div>
      </section>
      <form id={formId} className={styles.sections} noValidate onSubmit={(e) => void onSubmit(e)}>
        <SettingsCard title="Görünüm">
          <SettingRows>
            <SettingRow
              id="baglanti"
              title="Bağlantı adı"
              description="Değiştirirseniz eski bağlantı 30 gün yönlendirilir."
            >
              <TextField
                label="Bağlantı adı"
                hideLabel
                autoCapitalize="none"
                spellCheck={false}
                aria-describedby={errors.slug ? undefined : 'baglanti-aciklama'}
                error={errors.slug?.message}
                {...form.register('slug')}
              />
            </SettingRow>
            <SettingRow
              id="kapak"
              narrow
              title="Kapak rengi"
              description="Salon başlığının zemini."
            >
              <fieldset className={styles.swatches} aria-describedby="kapak-aciklama">
                <legend className={styles.visuallyHidden}>Kapak rengi</legend>
                {coverColors.map((color) => (
                  <label
                    key={color.value}
                    className={[styles.swatch, styles[color.value]].filter(Boolean).join(' ')}
                  >
                    <input
                      type="radio"
                      className={styles.swatchInput}
                      value={color.value}
                      aria-label={color.label}
                      {...form.register('coverColor')}
                    />
                  </label>
                ))}
              </fieldset>
            </SettingRow>
            <SettingRow
              id="degerlendirme"
              narrow
              title="Değerlendirmeleri göster"
              description="Ortalama puan ve son yorumlar sayfada görünür."
            >
              <Switch
                label="Değerlendirmeleri göster"
                hideLabel
                aria-describedby="degerlendirme-aciklama"
                {...form.register('showReviews')}
              />
            </SettingRow>
            <SettingRow
              id="fiyat"
              narrow
              title="Fiyatları göster"
              description="Kapalıysa “fiyat salonda” yazar."
            >
              <Switch
                label="Fiyatları göster"
                hideLabel
                aria-describedby="fiyat-aciklama"
                {...form.register('showPrices')}
              />
            </SettingRow>
          </SettingRows>
        </SettingsCard>
        <SettingsCard
          title="Randevu öncesi bilgi"
          description="Onay ekranında ve onay SMS'inde gösterilir."
        >
          <TextField
            label="Müşteriye not"
            placeholder="Randevunuzdan 10 dakika önce gelmenizi rica ederiz."
            maxLength={200}
            error={errors.preAppointmentNote?.message}
            {...form.register('preAppointmentNote')}
          />
        </SettingsCard>
      </form>
    </SettingsLayout>
  );
}
