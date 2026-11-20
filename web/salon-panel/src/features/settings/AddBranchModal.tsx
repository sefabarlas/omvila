import { zodResolver } from '@hookform/resolvers/zod';
import { ApiProblem, idempotencyKey } from '@omvila/api-client-ts';
import { Banner, Button, Modal, Select, TextField, useToast } from '@omvila/ui-web/panel';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { useSelectedBranch } from '@/app/branch';
import { useSession } from '@/app/session';
import { useCreateBranch } from './branch-api';
import { describeSaveError } from './save-error';
import styles from './Settings.module.css';

// Uzunluk sınırları sözleşmeden (BranchCreate).
const schema = z.object({
  name: z.string().trim().min(2, 'Şube adı en az 2 karakter olmalı.').max(120),
  phone: z.string().trim(),
  addressLine: z.string().trim().min(1, 'Adresi girin.').max(300),
  district: z.string().trim().min(1, 'İlçeyi girin.').max(80),
  city: z.string().trim().min(1, 'İli girin.').max(80),
  copyHoursFrom: z.string(),
  copyPricesFrom: z.string(),
});

type FormValues = z.infer<typeof schema>;

/**
 * Şube ekle (salon-paneli/SubeEkle). Saatler ve fiyatlar seçilen şubeden kopyalanır (bağ tutulmaz). Plan sınırı
 * aşılırsa sunucu 403 + meta.planLimit döner. Açıkken bağlanır; her açılışta form boş ve yeni Idempotency-Key.
 */
export function AddBranchModal({ onClose }: { onClose: () => void }) {
  const toast = useToast();
  const { session } = useSession();
  const { branchId, setBranchId } = useSelectedBranch();
  const create = useCreateBranch();
  const [key] = useState(() => idempotencyKey()['Idempotency-Key']);
  const [limitReached, setLimitReached] = useState(false);
  const source = branchId ?? session.branches[0]?.id ?? '';
  const single = session.branches.length === 1;
  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      name: '',
      phone: '',
      addressLine: '',
      district: '',
      city: '',
      copyHoursFrom: source,
      copyPricesFrom: source,
    },
  });
  const { errors } = form.formState;

  const hoursOptions = [
    ...session.branches.map((b) => ({
      value: b.id,
      label: single ? 'İşletme saatleriyle aynı' : `${b.name} saatleriyle aynı`,
    })),
    { value: '', label: 'Sonra gireceğim' },
  ];
  const priceOptions = [
    ...session.branches.map((b) => ({ value: b.id, label: `${b.name} fiyatlarını kopyala` })),
    { value: '', label: 'Fiyatları sonra gireceğim' },
  ];

  const onSubmit = form.handleSubmit(async (values) => {
    try {
      const branch = await create.mutateAsync({
        key,
        body: {
          name: values.name,
          phone: values.phone === '' ? null : values.phone,
          addressLine: values.addressLine,
          district: values.district,
          city: values.city,
          copyWorkingHoursFromBranchId: values.copyHoursFrom === '' ? null : values.copyHoursFrom,
          copyServicePricesFromBranchId:
            values.copyPricesFrom === '' ? null : values.copyPricesFrom,
        },
      });
      toast.show({ tone: 'basari', title: `${branch.name} şubesi eklendi` });
      setBranchId(branch.id);
      onClose();
    } catch (error) {
      if (error instanceof ApiProblem && error.status === 403 && 'planLimit' in error.meta) {
        setLimitReached(true);
        return;
      }

      const view = describeSaveError(error);
      if (view.fields.phone) {
        form.setError('phone', { message: 'Telefon numarasını kontrol edin: 0216 330 40 40' });
        return;
      }

      toast.show({ tone: 'hata', title: view.message });
    }
  });

  return (
    <Modal
      open
      onClose={onClose}
      title="Şube ekle"
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            type="submit"
            form="sube-ekle"
            loading={create.isPending}
            loadingLabel="Ekleniyor…"
          >
            Şubeyi ekle
          </Button>
        </>
      }
    >
      <form id="sube-ekle" className={styles.grid} noValidate onSubmit={(e) => void onSubmit(e)}>
        <TextField
          label="Şube adı"
          placeholder="Moda Sahil"
          error={errors.name?.message}
          {...form.register('name')}
        />
        <TextField
          label="Şube telefonu"
          type="tel"
          inputMode="tel"
          placeholder="0216 330 40 40"
          error={errors.phone?.message}
          {...form.register('phone')}
        />
        <TextField
          className={styles.full}
          label="Adres"
          placeholder="Moda Cad. 120"
          autoComplete="street-address"
          error={errors.addressLine?.message}
          {...form.register('addressLine')}
        />
        <TextField
          label="İlçe"
          placeholder="Kadıköy"
          error={errors.district?.message}
          {...form.register('district')}
        />
        <TextField
          label="İl"
          placeholder="İstanbul"
          error={errors.city?.message}
          {...form.register('city')}
        />
        <Select
          label="Çalışma saatleri"
          options={hoursOptions}
          {...form.register('copyHoursFrom')}
        />
        <Select
          label="Hizmet ve fiyatlar"
          options={priceOptions}
          {...form.register('copyPricesFrom')}
        />
        {limitReached ? (
          <div className={styles.full}>
            <Banner tone="uyari" title="Planınızın şube sınırına ulaştınız">
              Yeni şube eklemek için aboneliğinizi yükseltmeniz gerekir.
            </Banner>
          </div>
        ) : null}
      </form>
    </Modal>
  );
}
