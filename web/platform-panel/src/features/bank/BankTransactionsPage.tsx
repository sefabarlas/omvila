import { formatDate, formatMoney, formatTime } from '@omvila/ui-web/format';
import { Badge, Button, DataTable, EmptyState, Switch } from '@omvila/ui-web/panel';
import { keepPreviousData, useInfiniteQuery } from '@tanstack/react-query';
import { Link, useNavigate } from '@tanstack/react-router';
import { ChevronLeft, Landmark, Plus, Search, Upload } from 'lucide-react';
import { useState } from 'react';
import { useApi } from '@/app/api-context';
import { platformTimeZone } from '@/app/config';
import brandStyles from '@/features/brands/Brands.module.css';
import styles from '@/features/approvals/Approvals.module.css';
import { Page } from '@/shared/Page';
import { adminOrFinanceHint, PermissionButton } from '@/shared/PermissionButton';
import { QueryState } from '@/shared/QueryState';
import { AddBankTransactionModal } from './AddBankTransactionModal';
import { bankTransactionsQuery } from './api';
import type { BankTransaction } from './api';
import { ImportBankModal } from './ImportBankModal';

function when(tx: BankTransaction): string {
  const date = formatDate(tx.valueDate, 'dayMonthShort');
  return tx.receivedUtc ? `${date} ${formatTime(tx.receivedUtc, platformTimeZone)}` : date;
}

/**
 * Banka hareketleri ("Banka hareketlerinde ara"; tasarımda ayrı ekranı yok, PMarkalar tablo deseniyle, T-126):
 * arama, eşleşmemiş süzgeci, elle ekleme ve CSV içe aktarma (Yönetici ve Finans).
 */
export function BankTransactionsPage({ q, unmatchedOnly }: { q: string; unmatchedOnly: boolean }) {
  const api = useApi();
  const navigate = useNavigate();
  const [dialog, setDialog] = useState<'add' | 'import' | null>(null);
  const [draft, setDraft] = useState(q);
  const query = useInfiniteQuery({
    ...bankTransactionsQuery(api, q, unmatchedOnly),
    placeholderData: keepPreviousData,
  });
  const rows = query.data?.pages.flatMap((page) => page.items);
  const go = (next: { q?: string; unmatchedOnly?: boolean }) => {
    const nextQ = (next.q ?? q).trim();
    const nextUnmatched = next.unmatchedOnly ?? unmatchedOnly;
    void navigate({
      to: '/onay/banka-hareketleri',
      search: {
        ...(nextQ.length >= 2 ? { q: nextQ } : {}),
        ...(nextUnmatched ? { eslesmemis: true } : {}),
      },
    });
  };

  return (
    <Page
      title="Banka hareketleri"
      context="OMVILA hesabına gelen havaleler"
      action={
        <div className={brandStyles.actions}>
          <PermissionButton
            permission="platform.billing.manage"
            lockedHint={adminOrFinanceHint}
            icon={<Upload size={16} aria-hidden="true" />}
            onClick={() => {
              setDialog('import');
            }}
          >
            CSV içe aktar
          </PermissionButton>
          <PermissionButton
            permission="platform.billing.manage"
            lockedHint={adminOrFinanceHint}
            icon={<Plus size={16} aria-hidden="true" />}
            onClick={() => {
              setDialog('add');
            }}
          >
            Hareket ekle
          </PermissionButton>
        </div>
      }
    >
      <Link to="/onay" className={brandStyles.back}>
        <ChevronLeft size={16} aria-hidden="true" />
        Onay kuyrukları
      </Link>
      <div className={styles.toolbar}>
        <form
          role="search"
          className={styles.search}
          onSubmit={(event) => {
            event.preventDefault();
            go({ q: draft });
          }}
        >
          <Search size={16} aria-hidden="true" />
          <input
            type="search"
            value={draft}
            placeholder="Gönderen, açıklama veya referans ara"
            aria-label="Banka hareketlerinde ara"
            onChange={(event) => {
              setDraft(event.target.value);
            }}
          />
        </form>
        <Switch
          label="Yalnız eşleşmemiş"
          checked={unmatchedOnly}
          onChange={(event) => {
            go({ unmatchedOnly: event.target.checked });
          }}
        />
      </div>
      <QueryState query={{ ...query, data: rows }}>
        {(items) => (
          <DataTable
            caption="Banka hareketleri"
            rows={items}
            rowKey={(tx) => tx.id}
            columns={[
              { key: 'date', label: 'Tarih', render: when },
              { key: 'sender', label: 'Gönderen', render: (tx) => tx.senderName },
              { key: 'description', label: 'Açıklama', render: (tx) => tx.description ?? '—' },
              { key: 'reference', label: 'Referans', render: (tx) => tx.bankReference },
              {
                key: 'amount',
                label: 'Tutar',
                numeric: true,
                render: (tx) => formatMoney(tx.amount),
              },
              {
                key: 'status',
                label: 'Durum',
                render: (tx) =>
                  tx.matchedNoticeId ? (
                    <Badge tone="basari">
                      {tx.matchedBrandName ? `Eşleşti · ${tx.matchedBrandName}` : 'Eşleşti'}
                    </Badge>
                  ) : (
                    <Badge tone="notr">Eşleşmedi</Badge>
                  ),
              },
              {
                key: 'source',
                label: 'Kaynak',
                render: (tx) => (tx.source === 'Manual' ? 'Elle' : 'CSV'),
              },
            ]}
            empty={
              q || unmatchedOnly ? (
                <EmptyState
                  icon={<Search size={22} />}
                  title="Süzgece uyan hareket yok"
                  description="Gönderen adının bir kısmını, açıklama kodunu (ör. ABN-KE-1125) veya banka referansını deneyin."
                />
              ) : (
                <EmptyState
                  icon={<Landmark size={22} />}
                  title="Henüz banka hareketi yok"
                  description="Banka dökümünü CSV olarak içe aktarın veya hareketi elle ekleyin."
                />
              )
            }
            footer={
              query.hasNextPage ? (
                <Button
                  size="sm"
                  loading={query.isFetchingNextPage}
                  loadingLabel="Yükleniyor…"
                  onClick={() => void query.fetchNextPage()}
                >
                  Daha fazla göster
                </Button>
              ) : undefined
            }
          />
        )}
      </QueryState>
      {dialog === 'add' ? (
        <AddBankTransactionModal
          onClose={() => {
            setDialog(null);
          }}
        />
      ) : null}
      {dialog === 'import' ? (
        <ImportBankModal
          onClose={() => {
            setDialog(null);
          }}
        />
      ) : null}
    </Page>
  );
}
