import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatDate } from '@omvila/ui-web/format';
import {
  Badge,
  Button,
  EmptyState,
  Modal,
  Select,
  Skeleton,
  TextArea,
  useToast,
} from '@omvila/ui-web/panel';
import { toBranchDate } from '@omvila/ui-web/time';
import { useQuery } from '@tanstack/react-query';
import { FileText } from 'lucide-react';
import { useState } from 'react';
import { useForm, useWatch } from 'react-hook-form';
import { useApi } from '@/app/api-context';
import { useClock } from '@/app/clock';
import { defaultTimeZone } from '@/app/config';
import { useSession } from '@/app/session';
import { ConflictBanner } from '@/shared/ConflictBanner';
import { DataExportModal } from './DataExportModal';
import kvkk from './Kvkk.module.css';
import {
  dataExportStatusLabels,
  latestDataExportQuery,
  privacySettingsQuery,
  useUpdatePrivacySettings,
} from './privacy-api';
import type { DataExportResult, PrivacySettings } from './privacy-api';
import { describeSaveError } from './save-error';
import styles from './Settings.module.css';
import { SettingRow, SettingRows, SettingsCard } from './SettingsLayout';

export const privacyFormId = 'kvkk-ve-veri';
const customRequired = 'Aydınlatma metninizi yazın veya standart metni kullanın.';

interface FormValues {
  noticeMode: PrivacySettings['noticeMode'];
  customNoticeText: string;
  transactionRetentionYears: string;
}

const toForm = (s: PrivacySettings): FormValues => ({
  noticeMode: s.noticeMode,
  customNoticeText: s.customNoticeText ?? '',
  transactionRetentionYears: String(s.transactionRetentionYears),
});

/** "1 Kasım 2026" (yerel gün). */
const longDay = (localDate: string) =>
  `${formatDate(localDate, 'dayMonth')} ${localDate.slice(0, 4)}`;

/**
 * KVKK ve veri: "Aydınlatma metni" ve "Veri saklama" kartları (salon-paneli/AyarlarKvkk). Metin ve süre settings.manage
 * (Ayarlar zaten bu izinle açılır), "Tüm verileri dışa aktar" ayrıca customer.export ister; izin yoksa satır görünmez.
 * Kaydetme üst bardaki "Değişiklikleri kaydet" ile (form privacyFormId).
 */
export function PrivacyCards({ onSaving }: { onSaving: (saving: boolean) => void }) {
  const api = useApi();
  const query = useQuery(privacySettingsQuery(api));

  if (query.isPending) {
    return <Skeleton variant="block" height="320px" />;
  }

  if (query.isError) {
    return (
      <section className={styles.card} aria-label="KVKK ayarları">
        <EmptyState
          headingLevel={2}
          title="KVKK ayarları yüklenemedi"
          description={errorMessage(
            query.error instanceof ApiProblem ? query.error.code : undefined,
          )}
          actions={<Button onClick={() => void query.refetch()}>Tekrar dene</Button>}
        />
      </section>
    );
  }

  return (
    <PrivacyForm settings={query.data} onSaving={onSaving} onReload={() => void query.refetch()} />
  );
}

function PrivacyForm({
  settings,
  onSaving,
  onReload,
}: {
  settings: PrivacySettings;
  onSaving: (saving: boolean) => void;
  onReload: () => void;
}) {
  const toast = useToast();
  const { can } = useSession();
  const update = useUpdatePrivacySettings();
  const [conflict, setConflict] = useState(false);
  const [preview, setPreview] = useState(false);
  const form = useForm<FormValues>({ defaultValues: toForm(settings) });
  const { errors } = form.formState;
  const [mode, customText] = useWatch({
    control: form.control,
    name: ['noticeMode', 'customNoticeText'],
  });
  const custom = mode === 'Custom';

  const onSubmit = form.handleSubmit(async (values) => {
    if (values.noticeMode === 'Custom' && values.customNoticeText.trim() === '') {
      form.setError('customNoticeText', { message: customRequired });
      return;
    }

    onSaving(true);
    try {
      const saved = await update.mutateAsync({
        version: settings.version,
        body: {
          noticeMode: values.noticeMode,
          // Standart metne dönünce salonun metni saklanır (alan gönderilmez: değişmez).
          ...(values.noticeMode === 'Custom'
            ? { customNoticeText: values.customNoticeText.trim() }
            : {}),
          transactionRetentionYears: Number(values.transactionRetentionYears),
        },
      });
      form.reset(toForm(saved));
      setConflict(false);
      toast.show({ tone: 'basari', title: 'KVKK ayarları kaydedildi' });
    } catch (error) {
      const view = describeSaveError(error);
      setConflict(view.conflict);
      if (view.fields.customNoticeText) {
        form.setError('customNoticeText', { message: customRequired });
      } else if (!view.conflict) {
        toast.show({ tone: 'hata', title: view.message });
      }
    } finally {
      onSaving(false);
    }
  });

  const noticeTitle = custom ? 'Kendi aydınlatma metniniz' : 'OMVILA standart aydınlatma metni';
  const updated = custom
    ? settings.customNoticeUpdatedUtc
      ? longDay(toBranchDate(new Date(settings.customNoticeUpdatedUtc), defaultTimeZone))
      : null
    : longDay(settings.standardNotice.updatedOn);

  return (
    <form
      id={privacyFormId}
      className={styles.sections}
      noValidate
      onSubmit={(e) => void onSubmit(e)}
    >
      {conflict ? <ConflictBanner onReload={onReload} /> : null}
      <SettingsCard
        title="Aydınlatma metni"
        description="Veri sorumlusu olarak işletme adınız ve iletişim bilginiz metne otomatik eklenir."
      >
        <div className={kvkk.notice}>
          <FileText size={20} aria-hidden="true" className={kvkk.noticeIcon} />
          <div className={kvkk.noticeText}>
            <strong>{noticeTitle}</strong>
            <span className={styles.cardText}>
              {updated ? `Son güncelleme ${updated} · ` : ''}rezervasyon sayfasında ve uygulamada
              gösterilir
            </span>
          </div>
          <div className={kvkk.noticeActions}>
            <Button
              size="sm"
              onClick={() => {
                setPreview(true);
              }}
            >
              Önizle
            </Button>
            <Button
              size="sm"
              variant="ghost"
              onClick={() => {
                form.setValue('noticeMode', custom ? 'Standard' : 'Custom', { shouldDirty: true });
                form.clearErrors('customNoticeText');
              }}
            >
              {custom ? 'Standart metni kullan' : 'Kendi metnimi kullan'}
            </Button>
          </div>
        </div>
        {custom ? (
          <TextArea
            label="Aydınlatma metniniz"
            rows={8}
            maxLength={20000}
            hint="Rezervasyon sayfasında ve uygulamada standart metnin yerine gösterilir."
            error={errors.customNoticeText?.message}
            {...form.register('customNoticeText')}
          />
        ) : null}
      </SettingsCard>
      <SettingsCard title="Veri saklama">
        <SettingRows>
          <SettingRow
            id="saklama"
            title="İşlem kayıtlarını saklama süresi"
            description="Randevu, ödeme ve prim kayıtları; silme talebinde anonim tutulur."
          >
            <Select
              label="İşlem kayıtlarını saklama süresi"
              hideLabel
              options={settings.allowedRetentionYears.map((years) => ({
                value: String(years),
                label: `${String(years)} yıl`,
              }))}
              {...form.register('transactionRetentionYears')}
            />
          </SettingRow>
          {can('customer.export') ? <ExportRow /> : null}
        </SettingRows>
      </SettingsCard>
      {preview ? (
        <Modal
          open
          onClose={() => {
            setPreview(false);
          }}
          title={noticeTitle}
        >
          <p className={kvkk.noticeBody}>
            {custom ? customText.trim() || 'Henüz metin yazılmadı.' : settings.standardNotice.text}
          </p>
        </Modal>
      ) : null}
    </form>
  );
}

const exportDefaultText = 'Müşteriler, randevular ve ödemeler CSV olarak e-postanıza gönderilir.';

/**
 * Son isteğin satırdaki görünümü: rozet ve açıklama. Bağlantısının süresi dolmuş istek gösterilmez (indirilecek bir şey
 * kalmadı); durum okunamazsa da satır varsayılan hâliyle kalır.
 */
export function exportRowView(
  result: DataExportResult | null | undefined,
  now: Date,
): { badge: { label: string; tone: 'bilgi' | 'basari' | 'hata' } | null; text: string } {
  if (!result) {
    return { badge: null, text: exportDefaultText };
  }

  const label = dataExportStatusLabels[result.status];
  switch (result.status) {
    case 'Failed':
      return {
        badge: { label, tone: 'hata' },
        text: 'Dışa aktarma hazırlanamadı. Yeniden isteyin.',
      };
    case 'Sent': {
      const expires = result.linkExpiresUtc ? new Date(result.linkExpiresUtc) : null;
      if (expires && expires.getTime() <= now.getTime()) {
        return { badge: null, text: exportDefaultText };
      }

      const until = expires
        ? ` Bağlantı ${longDay(toBranchDate(expires, defaultTimeZone))} tarihine kadar geçerli.`
        : '';
      return {
        badge: { label, tone: 'basari' },
        text: `İndirme bağlantısı ${result.emailMasked} adresine gönderildi.${until}`,
      };
    }
    default:
      return {
        badge: { label, tone: 'bilgi' },
        text: `İndirme bağlantısı ${result.emailMasked} adresine gönderilecek. Bağlantı belirli bir süre geçerlidir.`,
      };
  }
}

/**
 * "Tüm verileri dışa aktar": markanın son isteğinin durumu ve bağlantının gideceği maskeli adres satırda görünür; sayfa
 * açılışında sunucudan okunur (manageGetLatestDataExport).
 */
function ExportRow() {
  const api = useApi();
  const clock = useClock();
  const [open, setOpen] = useState(false);
  const latest = useQuery(latestDataExportQuery(api));
  const view = exportRowView(latest.data, clock.now());
  return (
    <SettingRow id="disa-aktar" title="Tüm verileri dışa aktar" description={view.text}>
      <div className={kvkk.exportControl}>
        {view.badge ? <Badge tone={view.badge.tone}>{view.badge.label}</Badge> : null}
        <Button
          icon={<FileText size={16} aria-hidden="true" />}
          onClick={() => {
            setOpen(true);
          }}
        >
          Dışa aktarma iste
        </Button>
      </div>
      {open ? (
        <DataExportModal
          onClose={() => {
            setOpen(false);
          }}
        />
      ) : null}
    </SettingRow>
  );
}
