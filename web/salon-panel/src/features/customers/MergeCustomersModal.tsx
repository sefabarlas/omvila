import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { Banner, Button, Checkbox, Modal, TextField, useToast } from '@omvila/ui-web/panel';
import { keepPreviousData, useInfiniteQuery, useQuery } from '@tanstack/react-query';
import { Search } from 'lucide-react';
import { useEffect, useState } from 'react';
import { useApi } from '@/app/api-context';
import { customerSearchQuery } from '@/features/appointments/api';
import type { CustomerSummary } from './api';
import styles from './Customers.module.css';
import { activePackage, customerPhone } from './labels';
import { customerPackagesQuery, useMergeCustomers } from './records-api';

/** Arama kutusu yazmayı bitirince (ms) sorgu gider. */
const searchDelayMs = 300;

/** Kartın satırları: Ad, Telefon, Ziyaret, Paket (etkin paket "Fön paketi · 2 seans", yoksa "—"). */
function RecordRows({ customer }: { customer: CustomerSummary }) {
  const api = useApi();
  const packages = useInfiniteQuery(customerPackagesQuery(api, customer.id));
  const active = activePackage(packages.data?.pages.flatMap((page) => page.items) ?? []);
  const rows = [
    { label: 'Ad', value: customer.fullName },
    { label: 'Telefon', value: customerPhone(customer) || '—' },
    { label: 'Ziyaret', value: String(customer.visitCount) },
    {
      label: 'Paket',
      value: active ? `${active.packageName} · ${String(active.remainingSessions)} seans` : '—',
    },
  ];
  return (
    <dl className={styles.rows}>
      {rows.map((row) => (
        <div key={row.label} className={styles.row}>
          <dt>{row.label}</dt>
          <dd>{row.value}</dd>
        </div>
      ))}
    </dl>
  );
}

/**
 * Kayıtları birleştir (salon-paneli/MusteriBirlestir): açık kart "Kalacak kayıt"tır; "Birleşecek kayıt" ad veya
 * telefonla aranıp seçilir (tasarımda seçim adımı yok, en yakın desen yeni randevu müşteri araması; T-99).
 * "Notları birleştir" varsayılan açık. customer.merge (sahip, şube müdürü).
 */
export function MergeCustomersModal({
  customer,
  onClose,
}: {
  customer: CustomerSummary;
  onClose: () => void;
}) {
  const api = useApi();
  const toast = useToast();
  const merge = useMergeCustomers(customer.id);
  const [text, setText] = useState('');
  const [q, setQ] = useState('');
  const [source, setSource] = useState<CustomerSummary | null>(null);
  const [mergeNotes, setMergeNotes] = useState(true);
  const search = useQuery({
    ...customerSearchQuery(api, q),
    enabled: q !== '' && source === null,
    placeholderData: keepPreviousData,
  });
  const results = (search.data?.items ?? []).filter((c) => c.id !== customer.id);

  useEffect(() => {
    const timer = setTimeout(() => {
      setQ(text.trim());
    }, searchDelayMs);
    return () => {
      clearTimeout(timer);
    };
  }, [text]);

  const submit = () => {
    if (!source) {
      return;
    }

    merge.mutate(
      { sourceBrandCustomerId: source.id, mergeNotes },
      {
        onSuccess: () => {
          toast.show({ tone: 'basari', title: 'Kayıtlar birleştirildi' });
          onClose();
        },
      },
    );
  };

  return (
    <Modal
      open
      size="lg"
      title="Kayıtları birleştir"
      description="Aynı kişiye ait iki kayıt birleşir. Randevular, paketler, ödemeler ve notlar tek kartta toplanır. Birleşen kayıt arşive kalkar."
      onClose={onClose}
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            disabled={source === null}
            loading={merge.isPending}
            loadingLabel="Birleştiriliyor…"
            onClick={submit}
          >
            Kayıtları birleştir
          </Button>
        </>
      }
    >
      <div className={styles.dialogBody}>
        {merge.isError ? (
          <Banner tone="hata" title="Kayıtlar birleştirilemedi">
            {errorMessage(merge.error instanceof ApiProblem ? merge.error.code : undefined)}
          </Banner>
        ) : null}
        <div className={styles.mergeGrid}>
          <section className={styles.keepCard} aria-label="Kalacak kayıt">
            <h3 className={styles.overline}>Kalacak kayıt</h3>
            <RecordRows customer={customer} />
          </section>
          <section className={styles.sourceCard} aria-label="Birleşecek kayıt">
            <div className={styles.sourceHead}>
              <h3 className={styles.overline}>Birleşecek kayıt</h3>
              {source ? (
                <Button
                  size="sm"
                  variant="ghost"
                  onClick={() => {
                    setSource(null);
                    merge.reset();
                  }}
                >
                  Değiştir
                </Button>
              ) : null}
            </div>
            {source ? (
              <RecordRows customer={source} />
            ) : (
              <>
                <TextField
                  label="Birleşecek kaydı ara"
                  hideLabel
                  type="search"
                  placeholder="Ad veya telefon"
                  icon={<Search size={16} />}
                  value={text}
                  maxLength={80}
                  onChange={(event) => {
                    setText(event.target.value);
                  }}
                />
                {q !== '' && search.isSuccess && results.length === 0 ? (
                  <p className={styles.muted}>Bu aramayla başka kayıt bulunamadı.</p>
                ) : null}
                {results.length > 0 ? (
                  <ul className={styles.pickList} aria-label="Bulunan kayıtlar">
                    {results.map((c) => (
                      <li key={c.id}>
                        <button
                          type="button"
                          className={styles.pickItem}
                          onClick={() => {
                            setSource(c);
                          }}
                        >
                          <strong>{c.fullName}</strong>
                          <span className={styles.muted}>
                            {[customerPhone(c), `${String(c.visitCount)} ziyaret`]
                              .filter(Boolean)
                              .join(' · ')}
                          </span>
                        </button>
                      </li>
                    ))}
                  </ul>
                ) : null}
              </>
            )}
          </section>
        </div>
        <Checkbox
          label="Notları birleştir"
          description="İki kaydın notları tarih sırasıyla eklenir"
          checked={mergeNotes}
          onChange={(event) => {
            setMergeNotes(event.target.checked);
          }}
        />
      </div>
    </Modal>
  );
}
