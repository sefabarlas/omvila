import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatCount, formatMoney } from '@omvila/ui-web/format';
import { Banner, Button, EmptyState, Modal, Skeleton } from '@omvila/ui-web/panel';
import { useQuery } from '@tanstack/react-query';
import { Upload } from 'lucide-react';
import { Fragment, useState } from 'react';
import { useApi } from '@/app/api-context';
import { creditPackagesQuery, usePurchaseCredits } from './api';
import type { CreditPurchaseInstructions } from './api';
import { NotifyTransferForm } from './NotifyTransferForm';
import styles from './Subscription.module.css';
import { TransferDetails } from './TransferDetails';

type Step =
  { kind: 'packages' } | { kind: 'transfer' | 'notify'; info: CreditPurchaseInstructions };

const notifyFormId = 'kontor-havale-bildir';

export interface CreditPurchaseModalProps {
  open: boolean;
  onClose: () => void;
}

/**
 * Kontör al (K1: v1'de yalnız havale; T-125). Paket seçimi (manageListCreditPackages) → havale bilgisi
 * (managePurchaseCredits: alıcı, IBAN, KNT- kodu, tutar) → "Havaleyi bildir" (purpose Credits). Kontör, ekip havaleyi
 * banka hareketiyle eşleştirince eklenir.
 */
export function CreditPurchaseModal({ open, onClose }: CreditPurchaseModalProps) {
  return open ? <CreditPurchaseDialog onClose={onClose} /> : null;
}

function CreditPurchaseDialog({ onClose }: { onClose: () => void }) {
  const api = useApi();
  const packages = useQuery(creditPackagesQuery(api));
  const purchase = usePurchaseCredits();
  const [selected, setSelected] = useState<string | null>(null);
  const [step, setStep] = useState<Step>({ kind: 'packages' });
  const [sending, setSending] = useState(false);
  const items = packages.data?.items ?? [];
  const chosen = selected ?? items[0]?.id ?? null;

  const next = () => {
    if (!chosen) return;
    purchase.mutate(chosen, {
      onSuccess: (info) => {
        setStep({ kind: 'transfer', info });
      },
    });
  };

  // Adım başına ayrı anahtar: aynı yerdeki buton yeniden kullanılırsa "Havaleyi bildir" tıklaması yeni "Bildir"
  // (type=submit) butonunu tetikleyip formu boş gönderir.
  const footer =
    step.kind === 'packages' ? (
      <Fragment key="packages">
        <Button onClick={onClose}>Vazgeç</Button>
        <Button
          variant="primary"
          disabled={chosen === null}
          loading={purchase.isPending}
          loadingLabel="Hazırlanıyor…"
          onClick={next}
        >
          Havale bilgisini göster
        </Button>
      </Fragment>
    ) : step.kind === 'transfer' ? (
      <Fragment key="transfer">
        <Button onClick={onClose}>Kapat</Button>
        <Button
          variant="primary"
          icon={<Upload size={16} aria-hidden="true" />}
          disabled={step.info.omvilaBankAccount === null}
          onClick={() => {
            setStep({ kind: 'notify', info: step.info });
          }}
        >
          Havaleyi bildir
        </Button>
      </Fragment>
    ) : (
      <Fragment key="notify">
        <Button
          onClick={() => {
            setStep({ kind: 'transfer', info: step.info });
          }}
        >
          Geri
        </Button>
        <Button
          variant="primary"
          type="submit"
          form={notifyFormId}
          loading={sending}
          loadingLabel="Gönderiliyor…"
        >
          Bildir
        </Button>
      </Fragment>
    );

  return (
    <Modal
      open
      onClose={onClose}
      title="Kontör al"
      description={
        step.kind === 'packages'
          ? 'Paketi seçin; havale bilgisini gösterelim. Kontör, havale eşleşince bakiyenize eklenir.'
          : `${formatCount(step.info.credits)} SMS kontörü · havale ile`
      }
      footer={footer}
    >
      {step.kind === 'packages' ? (
        packages.isPending ? (
          <Skeleton variant="block" height="160px" />
        ) : packages.isError ? (
          <EmptyState
            headingLevel={3}
            title="Paketler yüklenemedi"
            description={errorMessage(
              packages.error instanceof ApiProblem ? packages.error.code : undefined,
            )}
            actions={<Button onClick={() => void packages.refetch()}>Tekrar dene</Button>}
          />
        ) : items.length === 0 ? (
          <EmptyState
            headingLevel={3}
            title="Satışta kontör paketi yok"
            description="Paketler tanımlandığında burada görünür."
          />
        ) : (
          <div className={styles.packages}>
            {purchase.isError ? (
              <Banner tone="hata" title="Havale bilgisi alınamadı">
                {errorMessage(
                  purchase.error instanceof ApiProblem ? purchase.error.code : undefined,
                )}
              </Banner>
            ) : null}
            <fieldset className={styles.packageList}>
              <legend className={styles.visuallyHidden}>Kontör paketi</legend>
              {items.map((item) => (
                <label key={item.id} className={styles.packageOption}>
                  <input
                    type="radio"
                    name="kontor-paketi"
                    value={item.id}
                    checked={chosen === item.id}
                    className={styles.packageRadio}
                    onChange={() => {
                      setSelected(item.id);
                    }}
                  />
                  <span className={styles.packageName}>{item.name}</span>
                  <span
                    className={styles.packageCredits}
                  >{`${formatCount(item.credits)} SMS`}</span>
                  <span className={styles.packagePrice}>
                    {item.price ? formatMoney(item.price) : 'Fiyat henüz tanımlı değil'}
                  </span>
                </label>
              ))}
            </fieldset>
          </div>
        )
      ) : step.kind === 'transfer' ? (
        <div className={styles.packages}>
          <TransferDetails
            account={step.info.omvilaBankAccount}
            transferCode={step.info.transferCode}
            amount={step.info.amount}
          />
          {step.info.omvilaBankAccount ? (
            <p className={styles.muted}>
              Açıklamaya kodu yazmayı unutmayın. Gönderdiyseniz bildirin; ekibimiz banka hareketiyle
              eşleştirince kontör bakiyenize eklenir.
            </p>
          ) : null}
        </div>
      ) : (
        <NotifyTransferForm
          formId={notifyFormId}
          purpose="Credits"
          creditPackageId={step.info.creditPackageId}
          onDone={onClose}
          onPendingChange={setSending}
        />
      )}
    </Modal>
  );
}
