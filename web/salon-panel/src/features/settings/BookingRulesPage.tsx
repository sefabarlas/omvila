import { Banner, Select, Switch, useToast } from '@omvila/ui-web/panel';
import { useSuspenseQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { useForm, useWatch } from 'react-hook-form';
import { useApi } from '@/app/api-context';
import { brandSettingsBody, brandSettingsQuery, useUpdateBrandSettings } from './api';
import type { BrandSettings } from './api';
import { ConflictBanner } from '@/shared/ConflictBanner';
import { describeSaveError } from './save-error';
import styles from './Settings.module.css';
import { SettingRow, SettingRows, SettingsCard, SettingsLayout } from './SettingsLayout';

const formId = 'randevu-kurallari';

interface Option {
  value: string;
  label: string;
}

const hours = (h: number) => (h === 0 ? 'Sınır yok' : `${String(h)} saat önce`);

// Seçenekler sözleşmedeki aralıklardan (sureler-ve-ayarlar.md); kayıtlı değer listede yoksa o da eklenir.
const approvalTimeouts: Option[] = [
  { value: '30', label: '30 dakika' },
  { value: '60', label: '1 saat' },
  { value: '120', label: '2 saat (önerilen)' },
  { value: '240', label: '4 saat' },
];
const cutoffHours = [0, 2, 4, 6, 12, 24, 48, 72];
const noShowMinutes = [5, 10, 15, 20, 30, 45, 60];
const riskyThresholds = [1, 2, 3, 4, 5];
const depositPolicies: Option[] = [
  { value: 'None', label: 'Hiçbir zaman' },
  { value: 'RiskyCustomersOnly', label: 'Yalnızca riskli müşterilerde' },
  { value: 'Always', label: 'Her zaman' },
];

function withCurrent(options: Option[], current: number, label: (n: number) => string): Option[] {
  return options.some((o) => o.value === String(current))
    ? options
    : [...options, { value: String(current), label: label(current) }].sort(
        (a, b) => Number(a.value) - Number(b.value),
      );
}

function numberOptions(values: number[], current: number, label: (n: number) => string) {
  return withCurrent(
    values.map((v) => ({ value: String(v), label: label(v) })),
    current,
    label,
  );
}

const asNumber = { setValueAs: (value: string) => Number(value) };

/**
 * Ayarlar → Randevu kuralları (salon-paneli/Ayarlar). Dilim 1'de yalnız ayar kaydedilir; davranışı Dilim 2–3'te.
 * Ekranda olmayan alanlar (ileri tarih sınırı, hatırlatmalar…) olduğu gibi geri gönderilir.
 */
export function BookingRulesPage() {
  const api = useApi();
  const toast = useToast();
  const query = useSuspenseQuery(brandSettingsQuery(api));
  const settings = query.data;
  const update = useUpdateBrandSettings();
  const [conflict, setConflict] = useState(false);
  const form = useForm<BrandSettings>({ defaultValues: settings });
  const requiresApproval = useWatch({ control: form.control, name: 'requiresApproval' });
  const allowStaffSelection = useWatch({ control: form.control, name: 'allowStaffSelection' });
  const depositAvailable = settings.depositAvailable === true;

  const onSubmit = form.handleSubmit(async (values) => {
    const body = brandSettingsBody(settings, values);
    try {
      const saved = await update.mutateAsync({ version: settings.version ?? '', body });
      form.reset(saved);
      setConflict(false);
      toast.show({ tone: 'basari', title: 'Değişiklikler kaydedildi' });
    } catch (error) {
      const view = describeSaveError(error);
      setConflict(view.conflict);
      if (!view.conflict) {
        toast.show({ tone: 'hata', title: view.message });
      }
    }
  });

  const reload = async () => {
    const fresh = await query.refetch();
    if (fresh.data) {
      form.reset(fresh.data);
    }

    setConflict(false);
  };

  const describedBy = (id: string) => ({
    'aria-describedby': `${id}-aciklama`,
  });

  return (
    <SettingsLayout context="Bütün şubeler için ortak" formId={formId} saving={update.isPending}>
      {conflict ? <ConflictBanner onReload={() => void reload()} /> : null}
      <form id={formId} className={styles.sections} noValidate onSubmit={(e) => void onSubmit(e)}>
        <SettingsCard title="Online randevu">
          <SettingRows>
            <SettingRow
              id="onay"
              narrow
              title="Randevular onayımla kesinleşsin"
              description="Açıksa online talepler takvime “Onay bekliyor” olarak düşer. Kapalıysa uygun saat hemen onaylanır."
            >
              <Switch
                label="Randevular onayımla kesinleşsin"
                hideLabel
                {...describedBy('onay')}
                {...form.register('requiresApproval')}
              />
            </SettingRow>
            <SettingRow
              id="yanit"
              title="Yanıt süresi"
              description="Süre dolunca talep otomatik iptal edilir, tutulan saat boşa çıkar ve müşteriye bildirim gider."
            >
              <Select
                label="Yanıt süresi"
                hideLabel
                disabled={!requiresApproval}
                options={approvalTimeouts}
                {...describedBy('yanit')}
                {...form.register('approvalTimeoutMinutes', asNumber)}
              />
            </SettingRow>
            <SettingRow
              id="personel"
              narrow
              title="Müşteri personel seçebilsin"
              description="Rezervasyon sayfasında personel adımı gösterilir."
            >
              <Switch
                label="Müşteri personel seçebilsin"
                hideLabel
                {...describedBy('personel')}
                {...form.register('allowStaffSelection')}
              />
            </SettingRow>
            <SettingRow
              id="fark-etmez"
              narrow
              title="“Fark etmez” seçeneği"
              description="En erken uygun personeli otomatik atar."
            >
              <Switch
                label="“Fark etmez” seçeneği"
                hideLabel
                disabled={!allowStaffSelection}
                {...describedBy('fark-etmez')}
                {...form.register('allowAnyStaffOption')}
              />
            </SettingRow>
          </SettingRows>
        </SettingsCard>

        <SettingsCard title="Kapora">
          {depositAvailable ? null : (
            <Banner tone="bilgi" title="Kapora için IBAN gerekir">
              Müşteri kaporayı salonun hesabına havale eder. IBAN tanımlı değilse kapora istenemez.
            </Banner>
          )}
          <SettingRows>
            <SettingRow
              id="kapora"
              title="Kapora istenecek durumlar"
              description="Tutar her hizmette ayrıca belirlenir (sabit tutar veya yüzde)."
            >
              <Select
                label="Kapora istenecek durumlar"
                hideLabel
                disabled={!depositAvailable}
                options={depositPolicies}
                {...describedBy('kapora')}
                {...form.register('depositPolicy')}
              />
            </SettingRow>
            <SettingRow
              id="riskli"
              title="Riskli müşteri tanımı"
              description="Bu kadar kez gelmeyen müşteri riskli sayılır."
            >
              <Select
                label="Riskli müşteri tanımı"
                hideLabel
                disabled={!depositAvailable}
                options={numberOptions(
                  riskyThresholds,
                  settings.riskyCustomerNoShowThreshold,
                  (n) => `${String(n)} kez gelmeyen`,
                )}
                {...describedBy('riskli')}
                {...form.register('riskyCustomerNoShowThreshold', asNumber)}
              />
            </SettingRow>
          </SettingRows>
        </SettingsCard>

        <SettingsCard title="İptal ve gelmeme">
          <SettingRows>
            <SettingRow
              id="iptal"
              title="Ücretsiz iptal süresi"
              description="Bu süreden sonra müşteri iptal edemez, salonu aramalıdır."
            >
              <Select
                label="Ücretsiz iptal süresi"
                hideLabel
                options={numberOptions(cutoffHours, settings.cancellationCutoffHours, hours)}
                {...describedBy('iptal')}
                {...form.register('cancellationCutoffHours', asNumber)}
              />
            </SettingRow>
            <SettingRow
              id="erteleme"
              title="Erteleme süresi"
              description="Randevuya bu süreden az kala müşteri saat değiştiremez."
            >
              <Select
                label="Erteleme süresi"
                hideLabel
                options={numberOptions(cutoffHours, settings.rescheduleCutoffHours, hours)}
                {...describedBy('erteleme')}
                {...form.register('rescheduleCutoffHours', asNumber)}
              />
            </SettingRow>
            <SettingRow
              id="gelmedi"
              title="“Gelmedi” sayılma süresi"
              description="Randevu saatinden bu kadar sonra check-in yoksa randevu otomatik “Gelmedi” olur. Yanlışsa geri alabilirsiniz."
            >
              <Select
                label="“Gelmedi” sayılma süresi"
                hideLabel
                options={numberOptions(
                  noShowMinutes,
                  settings.noShowAfterMinutes,
                  (n) => `${String(n)} dakika`,
                )}
                {...describedBy('gelmedi')}
                {...form.register('noShowAfterMinutes', asNumber)}
              />
            </SettingRow>
          </SettingRows>
        </SettingsCard>

        <SettingsCard title="Gizlilik">
          <SettingRows>
            <SettingRow
              id="gizlilik"
              narrow
              title="Telefon numarasını personelden gizle"
              description="Personel rolünde numaralar maskelenir, arama ve SMS butonları gizlenir."
            >
              <Switch
                label="Telefon numarasını personelden gizle"
                hideLabel
                {...describedBy('gizlilik')}
                {...form.register('maskCustomerPhoneFromStaff')}
              />
            </SettingRow>
          </SettingRows>
        </SettingsCard>
      </form>
    </SettingsLayout>
  );
}
