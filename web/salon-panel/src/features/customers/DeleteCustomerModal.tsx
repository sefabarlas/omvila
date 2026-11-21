import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { Banner, Button, Modal, Skeleton, TextField, useToast } from '@omvila/ui-web/panel';
import { useQuery } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { useState } from 'react';
import { useApi } from '@/app/api-context';
import { genitive } from '@/shared/turkish';
import type { CustomerSummary } from './api';
import styles from './Customers.module.css';
import { deletionPreviewQuery, useRequestDeletion } from './records-api';

/** Sunucuyla aynı karşılaştırma: büyük/küçük harf ve baştaki/sondaki boşluk yok sayılır. */
function sameName(a: string, b: string): boolean {
  return a.trim().toLocaleLowerCase('tr-TR') === b.trim().toLocaleLowerCase('tr-TR');
}

/**
 * KVKK silme talebi (salon-paneli/MusteriSil): geri alınamaz uyarısı, silinecek ve anonim korunacak kayıtlar, etkin
 * paket uyarısı; müşterinin adı yazılınca "Kalıcı olarak anonimleştir" etkinleşir. customer.kvkk (yalnız sahip).
 */
export function DeleteCustomerModal({
  customer,
  onClose,
}: {
  customer: CustomerSummary;
  onClose: () => void;
}) {
  const api = useApi();
  const toast = useToast();
  const navigate = useNavigate();
  const preview = useQuery(deletionPreviewQuery(api, customer.id));
  const deletion = useRequestDeletion(customer.id);
  const [typed, setTyped] = useState('');
  const fullName = preview.data?.fullName ?? customer.fullName;
  const matches = typed.trim() !== '' && sameName(typed, fullName);
  const mismatch =
    deletion.error instanceof ApiProblem && deletion.error.errors.confirmFullName !== undefined;

  const submit = () => {
    deletion.mutate(typed.trim(), {
      onSuccess: () => {
        toast.show({ tone: 'basari', title: 'Müşteri kaydı anonimleştirildi' });
        onClose();
        void navigate({ to: '/musteriler' });
      },
    });
  };

  const data = preview.data;
  const rows = data
    ? [
        { label: 'Silinecek', value: 'Ad, telefon, e-posta, notlar' },
        {
          label: 'Korunacak (anonim)',
          value: `${String(data.appointmentCount)} randevu, ${String(data.paymentCount)} ödeme`,
        },
        ...(data.upcomingAppointmentCount > 0
          ? [
              {
                label: 'Yaklaşan randevu',
                value: `${String(data.upcomingAppointmentCount)} randevu iptal edilir`,
                warn: true,
              },
            ]
          : []),
        ...(data.activePackages.length > 0
          ? [
              {
                label: 'Aktif paket',
                value: data.activePackages
                  .map((p) => `${p.packageName} · ${String(p.remainingSessions)} seans`)
                  .join(', '),
                warn: true,
              },
            ]
          : []),
      ]
    : [];

  return (
    <Modal
      open
      title="KVKK silme talebi"
      onClose={onClose}
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="danger"
            disabled={!matches || !data}
            loading={deletion.isPending}
            loadingLabel="Anonimleştiriliyor…"
            onClick={submit}
          >
            Kalıcı olarak anonimleştir
          </Button>
        </>
      }
    >
      <div className={styles.dialogBody}>
        <Banner tone="hata" title="Bu işlem geri alınamaz.">
          {`${genitive(fullName)} adı, telefonu ve e-postası maskelenir. Randevu, ödeme ve prim kayıtları anonim olarak saklanır.`}
        </Banner>
        {preview.isPending ? (
          <Skeleton variant="block" height="120px" />
        ) : preview.isError ? (
          <Banner tone="hata" title="Bilgiler yüklenemedi">
            {errorMessage(preview.error instanceof ApiProblem ? preview.error.code : undefined)}
          </Banner>
        ) : (
          <dl className={styles.rows}>
            {rows.map((row) => (
              <div key={row.label} className={styles.row}>
                <dt>{row.label}</dt>
                <dd className={'warn' in row ? styles.warnValue : undefined}>{row.value}</dd>
              </div>
            ))}
          </dl>
        )}
        {deletion.isError && !mismatch ? (
          <Banner tone="hata" title="Kayıt anonimleştirilemedi">
            {errorMessage(deletion.error instanceof ApiProblem ? deletion.error.code : undefined)}
          </Banner>
        ) : null}
        <TextField
          label="Onaylamak için müşterinin adını yazın"
          placeholder={fullName}
          value={typed}
          autoComplete="off"
          maxLength={120}
          error={mismatch ? 'Yazdığınız ad müşterinin adıyla eşleşmiyor.' : undefined}
          onChange={(event) => {
            setTyped(event.target.value);
          }}
        />
      </div>
    </Modal>
  );
}
