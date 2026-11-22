import { ApiProblem, idempotencyKey } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatMoney } from '@omvila/ui-web/format';
import { Banner, Button, Modal, Select, Skeleton, useToast } from '@omvila/ui-web/panel';
import { branchToday } from '@omvila/ui-web/time';
import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { useApi } from '@/app/api-context';
import { useClock } from '@/app/clock';
import { defaultTimeZone } from '@/app/config';
import { useSession } from '@/app/session';
import { CustomerPicker } from '@/features/appointments/CustomerPicker';
import type { CustomerChoice, PickedCustomer } from '@/features/appointments/CustomerPicker';
import { staffQuery } from '@/features/services/api';
import { Segmented } from '@/shared/Segmented';
import { packagesQuery, useSellPackage } from './api';
import type { PackageSaleRequest } from './api';
import { addDays, contentsLabel, untilLabel } from './labels';
import styles from './Packages.module.css';

type Method = PackageSaleRequest['paymentMethod'];

const methods: readonly { value: Method; label: string }[] = [
  { value: 'Card', label: 'Kart' },
  { value: 'Cash', label: 'Nakit' },
  { value: 'Transfer', label: 'Havale' },
];

/**
 * Müşteriye paket sat (salon-paneli/PaketSat; package.sell). Müşteri şeridi ve "Değiştir" (yalnız kayıtlı müşteri),
 * satıştaki paketler, satan personel (satış şubesinde etkin), özet (içerik, "… kadar", tutar) ve ödeme yöntemi.
 * OMVILA ödeme almaz: "Kart" salonun kendi POS'udur, yalnız yöntem kaydedilir. Pencere başına bir Idempotency-Key.
 */
export function SellPackageModal({
  branchId,
  customer,
  onClose,
}: {
  branchId: string;
  customer?: PickedCustomer | undefined;
  onClose: () => void;
}) {
  const api = useApi();
  const clock = useClock();
  const toast = useToast();
  const { session } = useSession();
  const packages = useQuery(packagesQuery(api, branchId));
  const staff = useQuery(staffQuery(api));
  const sell = useSellPackage();
  const [key] = useState(() => idempotencyKey()['Idempotency-Key']);
  const [choice, setChoice] = useState<CustomerChoice>(
    customer ? { kind: 'existing', customer } : { kind: 'search' },
  );
  const [packageId, setPackageId] = useState('');
  const [staffId, setStaffId] = useState('');
  const [method, setMethod] = useState<Method>('Card');
  const [showErrors, setShowErrors] = useState(false);
  const [staffError, setStaffError] = useState<string | undefined>();
  const [problem, setProblem] = useState<string | null>(null);
  const today = branchToday(clock, defaultTimeZone);

  const onSale = (packages.data?.items ?? []).filter((p) => p.isActive);
  const selected = onSale.find((p) => p.id === packageId) ?? onSale[0];
  const sellers = (staff.data?.items ?? []).filter(
    (s) => s.isActive && (s.branchIds.length === 0 || s.branchIds.includes(branchId)),
  );
  const ownStaff = session.membership.staffId ?? '';
  const seller =
    sellers.find((s) => s.id === staffId) ?? sellers.find((s) => s.id === ownStaff) ?? sellers[0];
  const loading = packages.isPending || staff.isPending;

  const submit = () => {
    setShowErrors(true);
    setProblem(null);
    if (choice.kind !== 'existing' || !selected || !seller) {
      return;
    }

    const brandCustomerId = choice.customer.id;
    sell.mutate(
      {
        brandCustomerId,
        key,
        body: {
          packageId: selected.id,
          branchId,
          soldByStaffId: seller.id,
          paymentMethod: method,
        },
      },
      {
        onSuccess: (sold) => {
          toast.show({
            tone: 'basari',
            title: 'Paket tanımlandı',
            message: `${sold.customer.fullName} · ${sold.packageName}`,
          });
          onClose();
        },
        onError: (error) => {
          if (error instanceof ApiProblem && error.errors.soldByStaffId) {
            setStaffError('Bu personel bu şubede çalışmıyor. Başka personel seçin.');
            return;
          }

          if (error instanceof ApiProblem && error.code === 'invalid_state_transition') {
            setProblem('Bu paket satıştan kaldırılmış. Başka paket seçin.');
            void packages.refetch();
            return;
          }

          setProblem(errorMessage(error instanceof ApiProblem ? error.code : undefined));
        },
      },
    );
  };

  return (
    <Modal
      open
      onClose={onClose}
      title="Müşteriye paket sat"
      size="lg"
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            disabled={!selected || !seller}
            loading={sell.isPending}
            loadingLabel="Kaydediliyor…"
            onClick={submit}
          >
            {selected ? `${formatMoney(selected.price)} al ve paketi tanımla` : 'Paketi tanımla'}
          </Button>
        </>
      }
    >
      <div className={styles.form}>
        {problem ? <Banner tone="hata" title={problem} /> : null}
        <CustomerPicker
          value={choice}
          onChange={setChoice}
          today={today}
          allowNew={false}
          error={showErrors && choice.kind !== 'existing' ? 'Müşteri seçin.' : undefined}
        />
        {loading ? (
          <Skeleton variant="block" height="160px" />
        ) : packages.isError || staff.isError ? (
          <Banner
            tone="hata"
            title="Paketler yüklenemedi"
            action={
              <Button
                size="sm"
                onClick={() => {
                  void packages.refetch();
                  void staff.refetch();
                }}
              >
                Tekrar dene
              </Button>
            }
          />
        ) : !selected ? (
          <Banner tone="bilgi" title="Satışta paket yok">
            Paketler sayfasından bir paketi satışa alın veya yeni paket oluşturun.
          </Banner>
        ) : (
          <>
            <div className={styles.row}>
              <Select
                label="Paket"
                value={selected.id}
                options={onSale.map((p) => ({ value: p.id, label: p.name }))}
                onChange={(event) => {
                  setPackageId(event.target.value);
                }}
              />
              <Select
                label="Satan personel"
                value={seller?.id ?? ''}
                error={staffError}
                options={sellers.map((s) => ({ value: s.id, label: s.displayName ?? s.fullName }))}
                onChange={(event) => {
                  setStaffError(undefined);
                  setStaffId(event.target.value);
                }}
              />
            </div>
            <dl className={styles.summary}>
              <div className={styles.summaryLine}>
                <dt>İçerik</dt>
                <dd>{contentsLabel(selected.items)}</dd>
              </div>
              <div className={styles.summaryLine}>
                <dt>Geçerlilik</dt>
                <dd>{untilLabel(addDays(today, selected.validityDays))}</dd>
              </div>
              <div className={[styles.summaryLine, styles.summaryTotal].join(' ')}>
                <dt>Tutar</dt>
                <dd>{formatMoney(selected.price)}</dd>
              </div>
            </dl>
            <section className={styles.method} aria-labelledby="paket-odeme-yontemi">
              <h3 id="paket-odeme-yontemi" className={styles.sectionTitle}>
                Ödeme yöntemi
              </h3>
              <Segmented<Method>
                legend="Ödeme yöntemi"
                name="paket-odeme-yontemi"
                options={methods}
                value={method}
                onChange={setMethod}
              />
            </section>
            <p className={styles.hint}>
              Satış primi satan personele yazılır. Seanslar kullanıldıkça gelir sayılır.
            </p>
          </>
        )}
      </div>
    </Modal>
  );
}
