import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { Banner, Button, Modal, TextField, useToast } from '@omvila/ui-web/panel';
import { FileText, Upload, X } from 'lucide-react';
import { useRef, useState } from 'react';
import type { SyntheticEvent } from 'react';
import { useNotifyTransfer } from './api';
import type { PaymentNoticePurpose } from './api';
import styles from './Subscription.module.css';

// PaymentNoticeCreate.receipt: PDF, JPG veya PNG, en fazla 5 MB (sözleşme ve guvenlik-kvkk.md; sunucu da denetler).
const receiptMaxBytes = 5 * 1024 * 1024;
const receiptTypes = ['application/pdf', 'image/jpeg', 'image/png'];
const receiptAccept = '.pdf,.jpg,.jpeg,.png,application/pdf,image/jpeg,image/png';
const receiptSizeError = 'Dekont en fazla 5 MB olabilir.';
const receiptFormatError = 'Dekont PDF, JPG veya PNG olmalı.';

function receiptProblem(file: File): string | null {
  if (!receiptTypes.includes(file.type)) {
    return receiptFormatError;
  }

  return file.size > receiptMaxBytes ? receiptSizeError : null;
}

function serverReceiptError(codes: readonly string[] | undefined): string | null {
  if (codes?.includes('too_large')) return receiptSizeError;
  if (codes?.includes('invalid_format')) return receiptFormatError;
  return null;
}

export interface NotifyTransferFormProps {
  formId: string;
  purpose: PaymentNoticePurpose;
  /** purpose = Credits ise seçilen paket. */
  creditPackageId?: string;
  onDone: () => void;
  onPendingChange?: (pending: boolean) => void;
}

/**
 * "Havaleyi bildir" formu: gönderen adı ve dekont isteğe bağlı. Bekleyen bildirim varsa 409
 * payment_notice_already_pending metni gösterilir. Gönderim düğmeleri pencerenin altındadır (form={formId}).
 */
export function NotifyTransferForm({
  formId,
  purpose,
  creditPackageId,
  onDone,
  onPendingChange,
}: NotifyTransferFormProps) {
  const toast = useToast();
  const notify = useNotifyTransfer();
  // Ağ hatasından sonra yeniden gönderim aynı anahtarla gider (çift bildirim olmaz).
  const [key] = useState(() => crypto.randomUUID());
  const [senderName, setSenderName] = useState('');
  const [receipt, setReceipt] = useState<File | null>(null);
  const [receiptError, setReceiptError] = useState<string | null>(null);
  const [problem, setProblem] = useState<string | null>(null);
  const input = useRef<HTMLInputElement>(null);

  const submit = async (event: SyntheticEvent) => {
    event.preventDefault();
    if (receiptError) return;
    setProblem(null);
    onPendingChange?.(true);
    try {
      await notify.mutateAsync({
        key,
        purpose,
        ...(creditPackageId ? { creditPackageId } : {}),
        ...(senderName.trim() ? { senderName: senderName.trim() } : {}),
        ...(receipt ? { receipt } : {}),
      });
      toast.show({ tone: 'basari', title: 'Havale bildiriminiz alındı' });
      onDone();
    } catch (error) {
      const code = error instanceof ApiProblem ? error.code : undefined;
      const fieldError =
        error instanceof ApiProblem ? serverReceiptError(error.errors.receipt) : null;
      if (fieldError) {
        setReceiptError(fieldError);
      } else {
        setProblem(errorMessage(code));
      }
    } finally {
      onPendingChange?.(false);
    }
  };

  return (
    <form id={formId} noValidate className={styles.notifyForm} onSubmit={(e) => void submit(e)}>
      {problem ? (
        <Banner tone="uyari" title="Bildirim gönderilemedi">
          {problem}
        </Banner>
      ) : null}
      <TextField
        label="Gönderen adı (isteğe bağlı)"
        hint="Dekonttaki ad; eşleştirmeyi hızlandırır."
        maxLength={120}
        autoComplete="name"
        value={senderName}
        onChange={(event) => {
          setSenderName(event.target.value);
        }}
      />
      <input
        ref={input}
        type="file"
        accept={receiptAccept}
        className={styles.visuallyHidden}
        aria-label="Dekont dosyası"
        tabIndex={-1}
        onChange={(event) => {
          const file = event.target.files?.[0];
          event.target.value = '';
          if (!file) return;
          setReceipt(file);
          setReceiptError(receiptProblem(file));
        }}
      />
      <div className={styles.fileCard}>
        <FileText size={20} aria-hidden="true" className={styles.fileIcon} />
        <div className={styles.fileText}>
          <strong>{receipt ? receipt.name : 'Dekont (isteğe bağlı)'}</strong>
          <span className={styles.muted}>PDF, JPG veya PNG; en fazla 5 MB.</span>
          {receiptError ? (
            <span className={styles.fieldError} role="alert">
              {receiptError}
            </span>
          ) : null}
        </div>
        {receipt ? (
          <Button
            size="sm"
            variant="ghost"
            icon={<X size={14} aria-hidden="true" />}
            onClick={() => {
              setReceipt(null);
              setReceiptError(null);
            }}
          >
            Kaldır
          </Button>
        ) : (
          <Button
            size="sm"
            icon={<Upload size={14} aria-hidden="true" />}
            onClick={() => input.current?.click()}
          >
            Dosya seç
          </Button>
        )}
      </div>
    </form>
  );
}

export interface NotifyTransferModalProps {
  open: boolean;
  onClose: () => void;
}

/** Abonelik havalesi bildirimi (Ayarlar → Abonelik ve fatura). */
export function NotifyTransferModal({ open, onClose }: NotifyTransferModalProps) {
  const formId = 'havale-bildir';
  const [pending, setPending] = useState(false);
  return (
    <Modal
      open={open}
      onClose={onClose}
      title="Havaleyi bildir"
      description="Ekibimiz bildirimi banka hareketiyle eşleştirince aboneliğiniz etkinleşir."
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            type="submit"
            form={formId}
            loading={pending}
            loadingLabel="Gönderiliyor…"
          >
            Bildir
          </Button>
        </>
      }
    >
      {open ? (
        <NotifyTransferForm
          formId={formId}
          purpose="Subscription"
          onDone={onClose}
          onPendingChange={setPending}
        />
      ) : null}
    </Modal>
  );
}
