import { formatIban, formatMoney } from '@omvila/ui-web/format';
import { Button } from '@omvila/ui-web/panel';
import { Check, Copy } from 'lucide-react';
import { useState } from 'react';
import type { Money, OmvilaBankAccount } from './api';
import styles from './Subscription.module.css';

async function copyText(value: string): Promise<boolean> {
  try {
    await navigator.clipboard.writeText(value);
    return true;
  } catch {
    return false;
  }
}

interface CopyRowProps {
  label: string;
  /** Panoya giden değer (IBAN boşluksuz). */
  value: string;
  display?: string;
}

/**
 * Kopyalanabilir satır: etiket, değer, "Kopyala". ui-web'deki CopyField rezervasyon (mobil ölçek) bileşenidir; panelin
 * yoğun kutusunda aynı davranış panel bileşenleriyle kurulur (T-125).
 */
function CopyRow({ label, value, display }: CopyRowProps) {
  const [copied, setCopied] = useState(false);
  return (
    <div className={styles.transferRow}>
      <dt className={styles.transferLabel}>{label}</dt>
      <dd className={styles.transferValue}>
        <span>{display ?? value}</span>
        <Button
          size="sm"
          variant="ghost"
          icon={
            copied ? <Check size={14} aria-hidden="true" /> : <Copy size={14} aria-hidden="true" />
          }
          aria-label={copied ? `${label} kopyalandı` : `${label} kopyala`}
          onClick={() => {
            void copyText(value).then(setCopied);
          }}
          onBlur={() => {
            setCopied(false);
          }}
        >
          {copied ? 'Kopyalandı' : 'Kopyala'}
        </Button>
      </dd>
    </div>
  );
}

export interface TransferDetailsProps {
  account: OmvilaBankAccount | null;
  transferCode: string;
  amount: Money | null;
}

/**
 * "Havale / EFT ile öde" kutusu: Alıcı, IBAN, Açıklama (ABN-/KNT- kodu), Tutar. Değerler yapılandırmadan gelir (S-14);
 * tanımlı değilse uydurma değer veya köşeli parantezli yer tutucu yazılmaz.
 */
export function TransferDetails({ account, transferCode, amount }: TransferDetailsProps) {
  return (
    <div className={styles.transferBox}>
      {account === null ? (
        <p className={styles.transferNote} role="note">
          OMVILA hesap bilgisi henüz tanımlı değil. Tanımlandığında alıcı ve IBAN burada görünür.
        </p>
      ) : null}
      <dl className={styles.transferList}>
        {account ? (
          <>
            <CopyRow label="Alıcı" value={account.accountHolderName} />
            <CopyRow label="IBAN" value={account.iban} display={formatIban(account.iban)} />
          </>
        ) : null}
        <CopyRow label="Açıklama" value={transferCode} />
        {amount ? (
          <CopyRow label="Tutar" value={formatMoney(amount)} />
        ) : (
          <div className={styles.transferRow}>
            <dt className={styles.transferLabel}>Tutar</dt>
            <dd className={styles.transferMuted}>Fiyat henüz tanımlı değil</dd>
          </div>
        )}
      </dl>
    </div>
  );
}
