import { zodResolver } from '@hookform/resolvers/zod';
import { ApiProblem, idempotencyKey } from '@omvila/api-client-ts';
import { personelRenkleri } from '@omvila/design-tokens';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatDuration, parseMobilePhone } from '@omvila/ui-web/format';
import { Banner, Button, Drawer, Select, Switch, TextField, useToast } from '@omvila/ui-web/panel';
import { useState } from 'react';
import type { CSSProperties } from 'react';
import { useForm, useWatch } from 'react-hook-form';
import { z } from 'zod';
import { useSession } from '@/app/session';
import { CheckboxGroup } from '@/shared/CheckboxGroup';
import type { Service } from '@/features/services/api';
import { useCreateStaff } from './api';
import type { StaffDetail } from './api';
import styles from './Staff.module.css';

const schema = z
  .object({
    fullName: z.string().trim().min(2, 'Ad soyad en az 2 karakter olmalı.').max(120),
    phone: z.string().trim(),
    title: z.string().trim().max(60),
    role: z.enum(['Staff', 'BranchManager']),
    calendarColor: z.string(),
    serviceIds: z.array(z.string()),
    branchIds: z.array(z.string()).min(1, 'En az bir şube seçin.'),
    copyHours: z.enum(['yes', 'no']),
    commission: z.string().trim(),
    isBookableOnline: z.boolean(),
    sendInvite: z.boolean(),
  })
  .superRefine((v, ctx) => {
    if (v.phone !== '' && parseMobilePhone(v.phone) === null) {
      ctx.addIssue({
        code: 'custom',
        path: ['phone'],
        message: 'Cep telefonu 10 haneli olmalı: 0538 274 19 05',
      });
    }

    const percent = Number(v.commission.replace('%', ''));
    if (v.commission !== '' && (!Number.isFinite(percent) || percent < 0 || percent > 100)) {
      ctx.addIssue({
        code: 'custom',
        path: ['commission'],
        message: 'Prim 0 ile 100 arasında olmalı.',
      });
    }
  });

type FormValues = z.infer<typeof schema>;

const roles = [
  { value: 'Staff', label: 'Personel' },
  { value: 'BranchManager', label: 'Şube müdürü' },
];

/**
 * Personel ekle (salon-paneli/PersonelEkle, kabul kriteri 10). Telefon verilirse kişi OMVILA İşletme'ye bu numarayla
 * girer; davet SMS'i isteğe bağlı. Özel süre ve ek ücret eklendikten sonra "Verdiği hizmetler"den. Açıkken bağlanır.
 */
export function AddStaffDrawer({
  services,
  branchId,
  usedColors,
  onClose,
  onCreated,
}: {
  services: readonly Service[];
  branchId: string;
  usedColors: readonly string[];
  onClose: () => void;
  onCreated: (staff: StaffDetail) => void;
}) {
  const toast = useToast();
  const { session } = useSession();
  const create = useCreateStaff();
  const [key] = useState(() => idempotencyKey()['Idempotency-Key']);
  const [limitReached, setLimitReached] = useState(false);
  const freeColor =
    personelRenkleri.find((c) => !usedColors.some((u) => u.toLowerCase() === c.toLowerCase())) ??
    personelRenkleri[0];
  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      fullName: '',
      phone: '',
      title: '',
      role: 'Staff',
      calendarColor: freeColor,
      serviceIds: [],
      branchIds: [branchId],
      copyHours: 'yes',
      commission: '',
      isBookableOnline: true,
      sendInvite: true,
    },
  });
  const { errors } = form.formState;
  const phone = useWatch({ control: form.control, name: 'phone' });
  const multiBranch = session.branches.length > 1;
  const branchName = session.branches.find((b) => b.id === branchId)?.name ?? '';

  const onSubmit = form.handleSubmit(async (values) => {
    try {
      const staff = await create.mutateAsync({
        key,
        body: {
          fullName: values.fullName,
          ...(values.phone !== '' ? { phone: values.phone } : {}),
          title: values.title === '' ? null : values.title,
          role: values.role,
          calendarColor: values.calendarColor,
          branchIds: values.branchIds,
          services: values.serviceIds.map((serviceId) => ({ serviceId })),
          copyBranchWorkingHours: values.copyHours === 'yes',
          serviceCommissionPercent:
            values.commission === '' ? 0 : Number(values.commission.replace('%', '')),
          isBookableOnline: values.isBookableOnline,
          sendInvite: values.phone !== '' && values.sendInvite,
        },
      });
      const invited = values.phone !== '' && values.sendInvite;
      toast.show({
        tone: 'basari',
        title: `${staff.fullName} eklendi`,
        ...(invited ? { message: "Davet SMS'i gönderildi." } : {}),
      });
      onCreated(staff);
      onClose();
    } catch (problem) {
      if (problem instanceof ApiProblem && problem.status === 403 && 'planLimit' in problem.meta) {
        setLimitReached(true);
        return;
      }

      if (problem instanceof ApiProblem && problem.errors.phone) {
        form.setError('phone', { message: 'Cep telefonu 10 haneli olmalı: 0538 274 19 05' });
        return;
      }

      toast.show({
        tone: 'hata',
        title: errorMessage(problem instanceof ApiProblem ? problem.code : undefined),
      });
    }
  });

  return (
    <Drawer
      open
      onClose={onClose}
      title="Personel ekle"
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            type="submit"
            form="personel-ekle"
            loading={create.isPending}
            loadingLabel="Ekleniyor…"
          >
            Personeli ekle
          </Button>
        </>
      }
    >
      <form
        id="personel-ekle"
        className={styles.form}
        noValidate
        onSubmit={(e) => void onSubmit(e)}
      >
        <TextField
          label="Ad soyad"
          placeholder="Deniz Kara"
          error={errors.fullName?.message}
          {...form.register('fullName')}
        />
        <TextField
          label="Cep telefonu"
          type="tel"
          inputMode="tel"
          placeholder="0538 274 19 05"
          hint="OMVILA İşletme'ye bu numarayla girer"
          error={errors.phone?.message}
          {...form.register('phone')}
        />
        <TextField
          label="Unvan"
          placeholder="Renk uzmanı"
          maxLength={60}
          {...form.register('title')}
        />
        <Select label="Rol" options={roles} {...form.register('role')} />
        <fieldset className={styles.group}>
          <legend className={styles.groupTitle}>Takvim rengi</legend>
          <p className={styles.groupHint}>Takvimde bu kişinin sütununu ve randevularını ayırır.</p>
          <div className={styles.swatches}>
            {personelRenkleri.map((color, index) => (
              <label
                key={color}
                className={styles.swatch}
                style={{ '--renk': color } as CSSProperties}
              >
                <input
                  type="radio"
                  className={styles.swatchInput}
                  value={color}
                  aria-label={`Renk ${String(index + 1)}`}
                  {...form.register('calendarColor')}
                />
              </label>
            ))}
          </div>
        </fieldset>
        <fieldset className={styles.group}>
          <legend className={styles.groupTitle}>Verdiği hizmetler</legend>
          <p className={styles.groupHint}>Özel süre ve ek ücreti ekledikten sonra girersiniz.</p>
          {services.length === 0 ? (
            <p className={styles.note}>Henüz hizmet yok.</p>
          ) : (
            <CheckboxGroup
              control={form.control}
              name="serviceIds"
              options={services.map((service) => ({
                value: service.id,
                label: service.name,
                description: formatDuration(service.durationMinutes),
              }))}
            />
          )}
        </fieldset>
        {multiBranch ? (
          <fieldset className={styles.group}>
            <legend className={styles.groupTitle}>Çalıştığı şubeler</legend>
            <p className={styles.groupHint}>
              Aynı kişi iki şubede aynı saatte randevu alamaz; şube arası geçiş süresi otomatik
              eklenir.
            </p>
            <CheckboxGroup
              control={form.control}
              name="branchIds"
              options={session.branches.map((branch) => ({ value: branch.id, label: branch.name }))}
            />
            {errors.branchIds?.message ? <p role="alert">{errors.branchIds.message}</p> : null}
          </fieldset>
        ) : null}
        <Select
          label={multiBranch ? 'Çalışma saatleri' : `${branchName} çalışma saatleri`}
          options={[
            { value: 'yes', label: 'İşletme saatleriyle aynı' },
            { value: 'no', label: 'Sonra gireceğim' },
          ]}
          {...form.register('copyHours')}
        />
        <TextField
          label="Hizmet primi"
          inputMode="decimal"
          suffix="%"
          error={errors.commission?.message}
          {...form.register('commission')}
        />
        <div className={styles.full}>
          <Switch
            label="Online randevuya açık"
            description="Rezervasyon sayfasında personel olarak görünür"
            {...form.register('isBookableOnline')}
          />
        </div>
        {phone.trim() !== '' ? (
          <div className={styles.full}>
            <Switch
              label="Davet SMS'i gönder"
              description="OMVILA İşletme uygulamasının indirme bağlantısı gider"
              {...form.register('sendInvite')}
            />
          </div>
        ) : null}
        {limitReached ? (
          <div className={styles.full}>
            <Banner tone="uyari" title="Planınızın personel sınırına ulaştınız">
              Yeni personel eklemek için aboneliğinizi yükseltmeniz gerekir.
            </Banner>
          </div>
        ) : null}
      </form>
    </Drawer>
  );
}
