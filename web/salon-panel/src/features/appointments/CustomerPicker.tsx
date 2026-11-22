import { Avatar, Button, Skeleton, TextField } from '@omvila/ui-web/panel';
import { keepPreviousData, useQuery } from '@tanstack/react-query';
import { Search, UserPlus } from 'lucide-react';
import { useEffect, useId, useState } from 'react';
import { useApi } from '@/app/api-context';
import type { CustomerSummary } from '@/features/customers/api';
import { customerPhone, dayLabel, looksLikePhone } from '@/features/customers/labels';
import { customerSearchQuery } from './api';
import styles from './NewAppointment.module.css';

export type PickedCustomer = Pick<
  CustomerSummary,
  'id' | 'fullName' | 'phone' | 'phoneMasked' | 'visitCount' | 'lastVisitOn'
>;

export interface NewCustomerDraft {
  fullName: string;
  phone: string;
}

/** Müşteri seçimi: kayıtlı müşteri (brandCustomerId) veya yeni müşteri (newCustomer); ikisi birden gönderilmez. */
export type CustomerChoice =
  | { kind: 'search' }
  | { kind: 'existing'; customer: PickedCustomer }
  | { kind: 'new'; draft: NewCustomerDraft };

/** Arama kutusu yazmayı bitirince (ms) sorgu gider. */
const searchDelayMs = 300;

interface CustomerPickerProps {
  value: CustomerChoice;
  onChange: (value: CustomerChoice) => void;
  today: string;
  /** Gönderimde eksikse gösterilen hata. */
  error?: string | undefined;
  /** Yeni müşteri alanlarının hataları. */
  draftErrors?: { fullName?: string | undefined; phone?: string | undefined };
  /** Yeni müşteri alanlarının altındaki not (bekleme listesinde kayıtla birlikte açılır). */
  newCustomerHint?: string;
  /** Yeni müşteri açılabilir mi (paket satışında yalnız kayıtlı müşteri). */
  allowNew?: boolean;
}

/** "Derya Şahin · 0532 760 21 18 · 6 ziyaret · son 21 Ekim". */
export function customerLine(customer: PickedCustomer, today: string): string[] {
  const phone = customerPhone(customer);
  return [
    ...(phone ? [phone] : []),
    `${String(customer.visitCount)} ziyaret`,
    ...(customer.lastVisitOn ? [`son ${dayLabel(customer.lastVisitOn, today)}`] : []),
  ];
}

/**
 * Yeni randevu › Müşteri (salon-paneli/YeniRandevu): seçili müşteri kartı ve "Değiştir"; ad veya telefonla 300 ms
 * gecikmeli arama (manageListCustomers); sonuç yoksa veya istenirse ad + telefonla yeni müşteri.
 */
export function CustomerPicker({
  value,
  onChange,
  today,
  error,
  draftErrors,
  newCustomerHint = 'Müşteri kartı randevuyla birlikte açılır.',
  allowNew = true,
}: CustomerPickerProps) {
  const labelId = useId();
  return (
    <div className={styles.section} role="group" aria-labelledby={labelId}>
      <h3 id={labelId} className={styles.label}>
        Müşteri
      </h3>
      {value.kind === 'existing' ? (
        <div className={styles.customer}>
          <Avatar name={value.customer.fullName} size="sm" />
          <span className={styles.customerText}>
            <strong>{value.customer.fullName}</strong>
            {customerLine(value.customer, today).map((part) => ` · ${part}`)}
          </span>
          <Button
            size="sm"
            variant="ghost"
            aria-label={`Müşteriyi değiştir: ${value.customer.fullName}`}
            onClick={() => {
              onChange({ kind: 'search' });
            }}
          >
            Değiştir
          </Button>
        </div>
      ) : value.kind === 'new' ? (
        <>
          <div className={styles.row}>
            <TextField
              label="Ad soyad"
              placeholder="Tülin Aksoy"
              autoComplete="off"
              value={value.draft.fullName}
              error={draftErrors?.fullName}
              onChange={(event) => {
                onChange({ kind: 'new', draft: { ...value.draft, fullName: event.target.value } });
              }}
            />
            <TextField
              label="Cep telefonu"
              type="tel"
              inputMode="tel"
              placeholder="0532 901 44 18"
              autoComplete="off"
              value={value.draft.phone}
              error={draftErrors?.phone}
              onChange={(event) => {
                onChange({ kind: 'new', draft: { ...value.draft, phone: event.target.value } });
              }}
            />
          </div>
          <div className={styles.actions}>
            <span className={styles.subLabel}>{newCustomerHint}</span>
            <Button
              size="sm"
              variant="ghost"
              icon={<Search size={16} aria-hidden="true" />}
              onClick={() => {
                onChange({ kind: 'search' });
              }}
            >
              Kayıtlı müşteri ara
            </Button>
          </div>
        </>
      ) : (
        <CustomerSearch onChange={onChange} today={today} error={error} allowNew={allowNew} />
      )}
    </div>
  );
}

function CustomerSearch({
  onChange,
  today,
  error,
  allowNew,
}: {
  onChange: (value: CustomerChoice) => void;
  today: string;
  error?: string | undefined;
  allowNew: boolean;
}) {
  const api = useApi();
  const [text, setText] = useState('');
  const [q, setQ] = useState('');
  useEffect(() => {
    const timer = setTimeout(() => {
      setQ(text.trim());
    }, searchDelayMs);
    return () => {
      clearTimeout(timer);
    };
  }, [text]);
  const query = useQuery({
    ...customerSearchQuery(api, q),
    enabled: q.length >= 2,
    placeholderData: keepPreviousData,
  });
  const results = q.length >= 2 ? (query.data?.items ?? []) : [];
  const startNew = () => {
    const typed = text.trim();
    const isPhone = looksLikePhone(typed);
    onChange({
      kind: 'new',
      draft: { fullName: isPhone ? '' : typed, phone: isPhone ? typed : '' },
    });
  };

  return (
    <>
      <TextField
        label="Müşteri ara"
        hideLabel
        type="search"
        placeholder="Ad veya telefon: Derya, 4418"
        autoComplete="off"
        icon={<Search size={16} />}
        value={text}
        error={error}
        onChange={(event) => {
          setText(event.target.value);
        }}
      />
      {q.length >= 2 && query.isPending ? <Skeleton variant="block" height="48px" /> : null}
      {results.length > 0 ? (
        <ul className={styles.results} aria-label="Müşteri sonuçları">
          {results.map((customer) => (
            <li key={customer.id}>
              <button
                type="button"
                className={styles.result}
                onClick={() => {
                  onChange({ kind: 'existing', customer });
                }}
              >
                <Avatar name={customer.fullName} size="sm" />
                <span>
                  <strong>{customer.fullName}</strong>
                  {customerLine(customer, today).map((part) => ` · ${part}`)}
                </span>
              </button>
            </li>
          ))}
        </ul>
      ) : null}
      <div className={styles.actions}>
        {q.length >= 2 && query.isSuccess && results.length === 0 ? (
          <span className={styles.subLabel}>{`"${q}" ile kayıtlı müşteri yok.`}</span>
        ) : null}
        {allowNew ? (
          <Button size="sm" icon={<UserPlus size={16} aria-hidden="true" />} onClick={startNew}>
            Yeni müşteri ekle
          </Button>
        ) : null}
      </div>
    </>
  );
}
