import { formatCount, formatDate } from '@omvila/ui-web/format';
import { DataTable, EmptyState } from '@omvila/ui-web/panel';
import { toBranchDate } from '@omvila/ui-web/time';
import { Coins } from 'lucide-react';
import type { ReactNode } from 'react';
import { platformTimeZone } from '@/app/config';
import styles from './Brands.module.css';
import { creditTransactionLabel, signedCount } from './labels';
import type { CreditTransaction } from './labels';

/** "Kontör hareketleri" tablosu (Özet: son 30 gün; Kontör sekmesi: bütün defter). */
export function CreditTransactionsTable({
  rows,
  footer,
}: {
  rows: readonly CreditTransaction[];
  footer?: ReactNode;
}) {
  return (
    <DataTable
      caption="Kontör hareketleri"
      rows={rows}
      rowKey={(tx) => tx.id}
      columns={[
        {
          key: 'date',
          label: 'Tarih',
          render: (tx) =>
            formatDate(toBranchDate(new Date(tx.occurredUtc), platformTimeZone), 'dayMonthShort'),
        },
        { key: 'type', label: 'Hareket', render: creditTransactionLabel },
        {
          key: 'credits',
          label: 'Miktar',
          numeric: true,
          render: (tx) => <span className={styles.positive}>{signedCount(tx.credits)}</span>,
        },
        {
          key: 'balance',
          label: 'Bakiye',
          numeric: true,
          render: (tx) => formatCount(tx.balanceAfter),
        },
      ]}
      empty={
        <EmptyState
          icon={<Coins size={22} />}
          title="Kontör hareketi yok"
          description="Bu dönemde kontör eklenmedi ve SMS gönderilmedi."
        />
      }
      footer={footer}
    />
  );
}
