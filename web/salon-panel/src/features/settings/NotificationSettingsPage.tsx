import { zodResolver } from '@hookform/resolvers/zod';
import { Badge, Banner, Button, IconButton, Select, Switch, useToast } from '@omvila/ui-web/panel';
import { formatCount } from '@omvila/ui-web/format';
import { useSuspenseQuery } from '@tanstack/react-query';
import { Plus, X } from 'lucide-react';
import { useState } from 'react';
import { useForm, useWatch } from 'react-hook-form';
import { z } from 'zod';
import { useApi } from '@/app/api-context';
import { ConflictBanner } from '@/shared/ConflictBanner';
import {
  notificationSettingsQuery,
  smsOverviewQuery,
  useUpdateNotificationSettings,
} from './notification-api';
import type { NotificationSettings, NotificationSettingsUpdate } from './notification-api';
import {
  reminderBounds,
  reminderChoices,
  reminderLabel,
  templateKey,
  templateName,
  templateServerProblem,
} from './notification-text';
import { describeSaveError } from './save-error';
import { SettingRow, SettingRows, SettingsCard, SettingsLayout } from './SettingsLayout';
import { SmsSenderModal } from './SmsSenderModal';
import { SmsSummary } from './SmsSummary';
import { TemplateEditModal } from './TemplateEditModal';
import styles from './NotificationSettings.module.css';
import settingsStyles from './Settings.module.css';

const formId = 'bildirim-ayarlari';

// NotificationSettingsUpdate: hatırlatmalar 30 dk – 3 gün, tekrarsız, en fazla 3.
const schema = z.object({
  reminderOffsetsMinutes: z
    .array(z.number().int().min(reminderBounds.min).max(reminderBounds.max))
    .max(
      reminderBounds.count,
      `En fazla ${String(reminderBounds.count)} hatırlatma ekleyebilirsiniz.`,
    )
    .refine((list) => new Set(list).size === list.length, 'Aynı zaman iki kez eklenemez.'),
  reviewInviteEnabled: z.boolean(),
  /** Şablon anahtarı → salon metni (null = sistem şablonu). */
  templates: z.record(z.string(), z.string().nullable()),
});

type FormValues = z.infer<typeof schema>;

function toForm(settings: NotificationSettings): FormValues {
  return {
    reminderOffsetsMinutes: [...settings.reminderOffsetsMinutes],
    reviewInviteEnabled: settings.reviewInviteEnabled,
    templates: Object.fromEntries(settings.templates.map((t) => [templateKey(t), t.customBody])),
  };
}

/** Yalnız değişen şablonlar gönderilir; gönderilmeyen şablon olduğu gibi kalır. */
function toBody(settings: NotificationSettings, values: FormValues): NotificationSettingsUpdate {
  const changed = settings.templates.filter(
    (t) => (values.templates[templateKey(t)] ?? null) !== t.customBody,
  );
  return {
    reminderOffsetsMinutes: values.reminderOffsetsMinutes,
    reviewInviteEnabled: values.reviewInviteEnabled,
    ...(changed.length > 0
      ? {
          templates: changed.map((t) => ({
            eventCode: t.eventCode,
            variant: t.variant,
            customBody: values.templates[templateKey(t)] ?? null,
          })),
        }
      : {}),
  };
}

/** templates[i].customBody → gönderilen i. şablonun anahtarı ve hata metni. */
function templateErrors(
  settings: NotificationSettings,
  body: NotificationSettingsUpdate,
  fields: Record<string, string[]>,
): Record<string, string> {
  const result: Record<string, string> = {};
  for (const [path, codes] of Object.entries(fields)) {
    const index = /^templates\[(\d+)\]/.exec(path)?.[1];
    const sent = index === undefined ? undefined : body.templates?.[Number(index)];
    const template = settings.templates.find(
      (t) => sent?.eventCode === t.eventCode && t.variant === (sent.variant ?? null),
    );
    if (template) {
      result[templateKey(template)] = templateServerProblem(codes[0], template.maxLength);
    }
  }

  return result;
}

/**
 * Ayarlar → Bildirimler ve SMS (salon-paneli/AyarlarBildirim). Kontör satın alma bu sayfada yok: OMVILA ödeme almaz,
 * kontör alma ucu (managePurchaseCredits) henüz sözleşmede değil (acik-sorular.md T-70).
 */
export function NotificationSettingsPage() {
  const api = useApi();
  const toast = useToast();
  const query = useSuspenseQuery(notificationSettingsQuery(api));
  const sms = useSuspenseQuery(smsOverviewQuery(api));
  const settings = query.data;
  const update = useUpdateNotificationSettings();
  const [conflict, setConflict] = useState(false);
  const [adding, setAdding] = useState(false);
  const [editing, setEditing] = useState<string | null>(null);
  const [senderOpen, setSenderOpen] = useState(false);
  const [serverErrors, setServerErrors] = useState<Record<string, string>>({});
  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: toForm(settings),
  });
  const offsets = useWatch({ control: form.control, name: 'reminderOffsetsMinutes' });
  const templates = useWatch({ control: form.control, name: 'templates' });
  const offsetError = form.formState.errors.reminderOffsetsMinutes;

  const setOffsets = (next: number[]) => {
    form.setValue(
      'reminderOffsetsMinutes',
      [...next].sort((a, b) => b - a),
      { shouldDirty: true, shouldValidate: true },
    );
  };

  const onSubmit = form.handleSubmit(async (values) => {
    const body = toBody(settings, values);
    try {
      const saved = await update.mutateAsync({ version: settings.version, body });
      form.reset(toForm(saved));
      setConflict(false);
      setServerErrors({});
      toast.show({ tone: 'basari', title: 'Değişiklikler kaydedildi' });
    } catch (error) {
      const view = describeSaveError(error);
      setConflict(view.conflict);
      if (view.conflict) {
        return;
      }

      const rowErrors = templateErrors(settings, body, view.fields);
      setServerErrors(rowErrors);
      if (Object.keys(rowErrors).length > 0) {
        return;
      }

      if (view.fields.reminderOffsetsMinutes) {
        form.setError('reminderOffsetsMinutes', {
          message: 'Hatırlatma zamanlarını kontrol edin.',
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

    setServerErrors({});
    setConflict(false);
  };

  const available = reminderChoices.filter((m) => !offsets.includes(m));
  const editingTemplate = settings.templates.find((t) => templateKey(t) === editing);

  return (
    <SettingsLayout context="Bildirimler ve SMS" formId={formId} saving={update.isPending}>
      {conflict ? <ConflictBanner onReload={() => void reload()} /> : null}
      <SmsSummary
        overview={sms.data}
        onRequestSender={() => {
          setSenderOpen(true);
        }}
      />
      <form
        id={formId}
        className={settingsStyles.sections}
        noValidate
        onSubmit={(e) => void onSubmit(e)}
      >
        <SettingsCard title="Hatırlatmalar">
          <SettingRows>
            <li className={styles.reminderRow}>
              <div className={settingsStyles.rowText}>
                <span id="hatirlatma-baslik" className={settingsStyles.rowTitle}>
                  Randevu hatırlatma zamanları
                </span>
                <span id="hatirlatma-aciklama" className={settingsStyles.rowDetail}>
                  Her hatırlatma ayrı mesajdır. Uygulama yüklüyse önce bildirim gider, ulaşmazsa
                  SMS.
                </span>
                {offsetError?.message ? (
                  <span className={styles.rowError} role="alert">
                    {offsetError.message}
                  </span>
                ) : null}
              </div>
              <ul className={styles.chips} aria-labelledby="hatirlatma-baslik">
                {offsets.length === 0 ? (
                  <li className={settingsStyles.rowDetail}>Hatırlatma gönderilmez.</li>
                ) : null}
                {offsets.map((minutes) => (
                  <li key={minutes} className={styles.chip}>
                    {reminderLabel(minutes)}
                    <IconButton
                      size="sm"
                      variant="ghost"
                      className={styles.chipRemove}
                      label={`${reminderLabel(minutes)} hatırlatmasını kaldır`}
                      icon={<X size={14} />}
                      onClick={() => {
                        setOffsets(offsets.filter((m) => m !== minutes));
                      }}
                    />
                  </li>
                ))}
                {offsets.length < reminderBounds.count && available.length > 0 ? (
                  <li>
                    {adding ? (
                      <Select
                        label="Hatırlatma zamanı"
                        hideLabel
                        // eslint-disable-next-line jsx-a11y/no-autofocus -- "Ekle" ile açılan seçim hemen kullanılır
                        autoFocus
                        defaultValue=""
                        options={[
                          { value: '', label: 'Zaman seçin' },
                          ...available.map((m) => ({ value: String(m), label: reminderLabel(m) })),
                        ]}
                        onChange={(event) => {
                          if (event.target.value !== '') {
                            setOffsets([...offsets, Number(event.target.value)]);
                          }

                          setAdding(false);
                        }}
                        onBlur={() => {
                          setAdding(false);
                        }}
                      />
                    ) : (
                      <Button
                        size="sm"
                        icon={<Plus size={14} aria-hidden="true" />}
                        onClick={() => {
                          setAdding(true);
                        }}
                      >
                        Ekle
                      </Button>
                    )}
                  </li>
                ) : null}
              </ul>
            </li>
            <SettingRow
              id="degerlendirme-daveti"
              narrow
              title="Değerlendirme daveti"
              description="Randevu tamamlandığında müşteriye gönderilir."
            >
              <Switch
                label="Değerlendirme daveti"
                hideLabel
                aria-describedby="degerlendirme-daveti-aciklama"
                {...form.register('reviewInviteEnabled')}
              />
            </SettingRow>
          </SettingRows>
        </SettingsCard>
        <SettingsCard title="Mesaj şablonları">
          {settings.templates.length === 0 ? (
            <p className={settingsStyles.rowDetail}>Düzenlenebilen şablon yok.</p>
          ) : (
            <ul className={styles.templates}>
              {settings.templates.map((template) => {
                const key = templateKey(template);
                const custom = templates[key] ?? null;
                const name = templateName(template);
                return (
                  <li key={key} className={styles.template}>
                    <div className={styles.templateText}>
                      <span className={settingsStyles.rowTitle}>{name}</span>
                      <span className={styles.templateBody}>{custom ?? template.systemBody}</span>
                      {serverErrors[key] ? (
                        <span className={styles.rowError}>{serverErrors[key]}</span>
                      ) : null}
                    </div>
                    <div className={styles.templateActions}>
                      {custom !== null ? (
                        <Badge tone="marka">Sizin metniniz</Badge>
                      ) : (
                        <Badge>Sistem şablonu</Badge>
                      )}
                      <Button
                        size="sm"
                        variant="ghost"
                        aria-label={`${name} şablonunu düzenle`}
                        onClick={() => {
                          setEditing(key);
                        }}
                      >
                        Düzenle
                      </Button>
                    </div>
                  </li>
                );
              })}
            </ul>
          )}
        </SettingsCard>
      </form>
      <Banner tone="uyari" title="Kontör biterse SMS gönderilmez.">
        Bakiye {formatCount(sms.data.lowCreditThreshold)} kontörün altına düşünce size haber
        veririz.
      </Banner>
      {editingTemplate ? (
        <TemplateEditModal
          template={editingTemplate}
          customBody={templates[templateKey(editingTemplate)] ?? null}
          onClose={() => {
            setEditing(null);
          }}
          onApply={(customBody) => {
            const key = templateKey(editingTemplate);
            // Anahtar nokta içerir (appointment.confirmed); RHF yolu olarak değil, nesnenin tamamı yazılır.
            form.setValue('templates', { ...templates, [key]: customBody }, { shouldDirty: true });
            setServerErrors((current) =>
              Object.fromEntries(Object.entries(current).filter(([k]) => k !== key)),
            );
            setEditing(null);
          }}
        />
      ) : null}
      <SmsSenderModal
        open={senderOpen}
        onClose={() => {
          setSenderOpen(false);
        }}
      />
    </SettingsLayout>
  );
}
