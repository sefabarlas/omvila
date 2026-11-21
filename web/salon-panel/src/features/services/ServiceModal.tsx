import { zodResolver } from '@hookform/resolvers/zod';
import { ApiProblem, idempotencyKey } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatDuration, parseMoneyInput } from '@omvila/ui-web/format';
import { Button, Modal, Select, Switch, TextField, useToast } from '@omvila/ui-web/panel';
import { useState } from 'react';
import { Controller, useForm, useWatch } from 'react-hook-form';
import { ConflictBanner } from '@/shared/ConflictBanner';
import { CheckboxGroup } from '@/shared/CheckboxGroup';
import { Segmented } from '@/shared/Segmented';
import { useCreateService, useSetBranchPrice, useUpdateService } from './api';
import type { Service, ServiceCategory, StaffSummary } from './api';
import { formToWrite, serviceSchema, totalMinutes } from './service-form';
import type { ServiceFormValues } from './service-form';
import styles from './Services.module.css';
import { StagesEditor } from './StagesEditor';

export interface BranchPriceContext {
  branchId: string;
  branchName: string;
  /** Şubede kayıtlı özel fiyat (yoksa genel fiyat geçerli). */
  current: number | null;
}

export interface ServiceModalProps {
  /** Düzenlenen hizmetin genel kaydı; yoksa yeni hizmet. */
  service?: Service;
  defaults: ServiceFormValues;
  categories: readonly ServiceCategory[];
  staff: readonly StaffSummary[];
  depositAvailable: boolean;
  /** Birden çok şubede düzenlerken seçili şubenin fiyatı (kabul kriteri 9). */
  branchPrice?: BranchPriceContext;
  /** 412 sonrası "Yenile": güncel kayıt okunur ve pencere onunla yeniden kurulur. */
  onReload?: () => void;
  onClose: () => void;
}

const depositModes = [
  { value: 'Inherit', label: 'Genel ayara uy' },
  { value: 'Always', label: 'Her zaman iste' },
  { value: 'Never', label: 'İsteme' },
];

const approvals = [
  { value: 'inherit', label: 'Genel ayara uy' },
  { value: 'yes', label: 'Onay gereksin' },
  { value: 'no', label: 'Onaysız' },
];

/**
 * Hizmet ekle / düzenle (salon-paneli/HizmetEkle). Aşama varsa süre aşama toplamıdır. Kapora alanları görünür ama
 * IBAN yoksa pasif (dilim-01). Oda veya cihaz Dilim 1'de gizli. Açıkken bağlanır; her açılışta yeni Idempotency-Key.
 */
export function ServiceModal({
  service,
  defaults,
  categories,
  staff,
  depositAvailable,
  branchPrice,
  onReload,
  onClose,
}: ServiceModalProps) {
  const toast = useToast();
  const create = useCreateService();
  const update = useUpdateService();
  const setPrice = useSetBranchPrice();
  const [key] = useState(() => idempotencyKey()['Idempotency-Key']);
  const [conflict, setConflict] = useState(false);
  const form = useForm<ServiceFormValues>({
    resolver: zodResolver(serviceSchema),
    defaultValues: defaults,
  });
  const { errors } = form.formState;
  const [stages, depositMode, depositKind, duration] = useWatch({
    control: form.control,
    name: ['stages', 'depositMode', 'depositKind', 'duration'],
  });
  const saving = create.isPending || update.isPending || setPrice.isPending;

  const onSubmit = form.handleSubmit(async (values) => {
    try {
      const body = formToWrite(values, service);
      const saved = service
        ? await update.mutateAsync({ id: service.id, version: service.version, body })
        : await create.mutateAsync({ key, body });
      if (branchPrice) {
        const next = values.branchPrice === '' ? null : parseMoneyInput(values.branchPrice);
        if (next !== branchPrice.current) {
          await setPrice.mutateAsync({
            serviceId: saved.id,
            branchId: branchPrice.branchId,
            amountMinor: next,
          });
        }
      }

      toast.show({
        tone: 'basari',
        title: service ? 'Hizmet kaydedildi' : `${saved.name} eklendi`,
      });
      onClose();
    } catch (problem) {
      const code = problem instanceof ApiProblem ? problem.code : undefined;
      if (problem instanceof ApiProblem && problem.status === 412) {
        setConflict(true);
        return;
      }

      if (code === 'deposit_unavailable_no_iban') {
        form.setError('depositMode', { message: errorMessage(code) });
        return;
      }

      toast.show({ tone: 'hata', title: errorMessage(code) });
    }
  });

  const categoryOptions = categories.map((c) => ({ value: c.id, label: c.name }));

  return (
    <Modal
      open
      size="lg"
      onClose={onClose}
      title={service ? 'Hizmeti düzenle' : 'Hizmet ekle'}
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            type="submit"
            form="hizmet"
            loading={saving}
            loadingLabel="Kaydediliyor…"
          >
            {service ? 'Hizmeti kaydet' : 'Hizmeti ekle'}
          </Button>
        </>
      }
    >
      <form id="hizmet" className={styles.form} noValidate onSubmit={(e) => void onSubmit(e)}>
        {conflict && onReload ? (
          <div className={styles.full}>
            <ConflictBanner
              onReload={() => {
                setConflict(false);
                onReload();
              }}
            />
          </div>
        ) : null}
        <TextField
          className={styles.span4}
          label="Hizmet adı"
          placeholder="Balyaj"
          maxLength={120}
          error={errors.name?.message}
          {...form.register('name')}
        />
        <Select
          className={styles.span2}
          label="Kategori"
          options={
            categoryOptions.length > 0 ? categoryOptions : [{ value: '', label: 'Kategori yok' }]
          }
          error={errors.categoryId?.message}
          {...form.register('categoryId')}
        />
        <TextField
          className={styles.span2}
          label="Süre"
          inputMode="numeric"
          suffix="dk"
          readOnly={stages.length > 0}
          value={stages.length > 0 ? String(totalMinutes({ duration, stages })) : undefined}
          hint={stages.length > 0 ? 'Aşamaların toplamı' : undefined}
          error={errors.duration?.message}
          {...(stages.length > 0 ? {} : form.register('duration'))}
        />
        <TextField
          className={styles.span2}
          label={branchPrice ? 'Genel fiyat' : 'Fiyat'}
          inputMode="decimal"
          suffix="₺"
          placeholder="450"
          error={errors.price?.message}
          {...form.register('price')}
        />
        <div className={`${styles.group ?? ''} ${styles.span2 ?? ''}`}>
          <span className={styles.groupTitle} id="fiyat-gosterimi">
            Fiyat gösterimi
          </span>
          <Controller
            control={form.control}
            name="priceDisplay"
            render={({ field }) => (
              <Segmented
                legend="Fiyat gösterimi"
                name="priceDisplay"
                options={[
                  { value: 'Fixed', label: 'Sabit' },
                  { value: 'StartingFrom', label: 'En az' },
                ]}
                value={field.value}
                onChange={field.onChange}
              />
            )}
          />
        </div>
        {branchPrice ? (
          <TextField
            className={styles.full}
            label={`${branchPrice.branchName} şubesinde fiyat`}
            inputMode="decimal"
            suffix="₺"
            hint="Boş bırakılırsa genel fiyat geçerli."
            error={errors.branchPrice?.message}
            {...form.register('branchPrice')}
          />
        ) : null}
        <TextField
          className={styles.full}
          label="Açıklama (isteğe bağlı)"
          placeholder="Uzunluğa göre değişir, prova dahil"
          maxLength={1000}
          {...form.register('description')}
        />
        <fieldset className={`${styles.group ?? ''} ${styles.full ?? ''}`}>
          <legend className={styles.groupTitle}>Kimler veriyor?</legend>
          {staff.length > 0 ? (
            <div className={styles.checks}>
              <CheckboxGroup
                control={form.control}
                name="staffIds"
                options={staff.map((person) => ({
                  value: person.id,
                  label: person.displayName ?? person.fullName,
                }))}
              />
            </div>
          ) : (
            <p className={styles.groupHint}>Personel ekleyince burada seçersiniz.</p>
          )}
        </fieldset>
        <Select
          className={styles.span2}
          label="Kapora"
          options={depositModes}
          disabled={!depositAvailable}
          hint={depositAvailable ? undefined : 'Kapora için IBAN gerekir.'}
          error={errors.depositMode?.message}
          {...form.register('depositMode')}
        />
        <div className={`${styles.group ?? ''} ${styles.span2 ?? ''}`}>
          <span className={styles.groupTitle}>Kapora türü</span>
          <Controller
            control={form.control}
            name="depositKind"
            render={({ field }) => (
              <Segmented
                legend="Kapora türü"
                name="depositKind"
                disabled={!depositAvailable || depositMode !== 'Always'}
                options={[
                  { value: 'amount', label: 'Tutar' },
                  { value: 'percent', label: 'Yüzde' },
                ]}
                value={field.value}
                onChange={field.onChange}
              />
            )}
          />
        </div>
        <TextField
          className={styles.span2}
          label="Kapora tutarı"
          inputMode="decimal"
          suffix={depositKind === 'amount' ? '₺' : '%'}
          disabled={!depositAvailable || depositMode !== 'Always'}
          error={errors.depositValue?.message}
          {...form.register('depositValue')}
        />
        <div className={styles.full}>
          <Switch label="Online randevuya açık" {...form.register('isOnlineBookable')} />
        </div>
        <div className={styles.divider} />
        <div className={styles.sectionHead}>
          <span className={styles.groupTitle}>Gelişmiş</span>
          <span className={styles.groupHint}>Boş bırakılanlar genel ayarları kullanır</span>
        </div>
        <TextField
          className={styles.span2}
          label="En az ön bildirim"
          inputMode="numeric"
          suffix="saat"
          error={errors.minNoticeHours?.message}
          {...form.register('minNoticeHours')}
        />
        <TextField
          className={styles.span2}
          label="En ileri tarih"
          inputMode="numeric"
          suffix="gün"
          error={errors.maxAdvanceDays?.message}
          {...form.register('maxAdvanceDays')}
        />
        <TextField
          className={styles.span2}
          label="Tampon süre"
          inputMode="numeric"
          suffix="dk"
          hint="Randevu sonrası temizlik"
          error={errors.bufferAfter?.message}
          {...form.register('bufferAfter')}
        />
        <Select
          className={styles.span3}
          label="Onay gereksin mi?"
          options={approvals}
          {...form.register('approval')}
        />
        <div className={`${styles.group ?? ''} ${styles.full ?? ''}`}>
          <span className={styles.groupTitle}>Aşamalar</span>
          <p className={styles.groupHint}>
            Bekleme aşamasında personel başka müşteriye müsait görünür.
            {stages.length > 0
              ? ` Toplam ${formatDuration(totalMinutes({ duration, stages }))}.`
              : ''}
          </p>
          <Controller
            control={form.control}
            name="stages"
            render={({ field }) => <StagesEditor stages={field.value} onChange={field.onChange} />}
          />
        </div>
      </form>
    </Modal>
  );
}
