import type { KeyboardEvent, ReactNode } from 'react';
import { Avatar } from './Avatar';
import { classNames } from './Button';
import { Checkbox } from './Checkbox';
import styles from './DataTable.module.css';

export interface DataTableColumn<Row> {
  key: string;
  label: string;
  align?: 'left' | 'right' | 'center';
  /** Sayı ve tutar: sağa hizalı, tabular-nums. */
  numeric?: boolean;
  render: (row: Row) => ReactNode;
}

export interface DataTableProps<Row> {
  /** Ekran okuyucu için tablo adı. */
  caption: string;
  columns: readonly DataTableColumn<Row>[];
  rows: readonly Row[];
  rowKey: (row: Row) => string;
  /** Satıra tıklamak (veya Enter) detayı açar. */
  onRowClick?: (row: Row) => void;
  /** Seçim: verilirse ilk kolonda onay kutuları. */
  selection?: {
    selected: ReadonlySet<string>;
    onChange: (selected: Set<string>) => void;
    rowLabel: (row: Row) => string;
  };
  /** Boş liste: EmptyState. */
  empty?: ReactNode;
  /** Toplam ve sayfalama: "1.284 müşteriden 1–4". */
  footer?: ReactNode;
}

/** Listeler (müşteri, hizmet, personel). Satır eylemleri sondaki menüde; satır tıklaması detayı açar. */
export function DataTable<Row>({
  caption,
  columns,
  rows,
  rowKey,
  onRowClick,
  selection,
  empty,
  footer,
}: DataTableProps<Row>) {
  if (rows.length === 0 && empty) {
    return <div className={styles.wrap}>{empty}</div>;
  }

  const allKeys = rows.map(rowKey);
  const allSelected =
    selection !== undefined &&
    allKeys.length > 0 &&
    allKeys.every((k) => selection.selected.has(k));
  const toggle = (key: string) => {
    if (!selection) {
      return;
    }

    const next = new Set(selection.selected);
    if (next.has(key)) {
      next.delete(key);
    } else {
      next.add(key);
    }

    selection.onChange(next);
  };

  const onRowKey = (event: KeyboardEvent<HTMLTableRowElement>, row: Row) => {
    if (event.key === 'Enter' && event.target === event.currentTarget) {
      onRowClick?.(row);
    }
  };

  const cellClass = (column: DataTableColumn<Row>) =>
    classNames(
      column.numeric || column.align === 'right'
        ? styles.right
        : column.align === 'center' && styles.center,
      column.numeric && styles.numeric,
    );

  return (
    <div className={styles.wrap}>
      <div className={styles.scroll}>
        <table className={styles.table}>
          <caption className={styles.caption}>{caption}</caption>
          <thead>
            <tr>
              {selection ? (
                <th className={styles.check}>
                  <Checkbox
                    label="Tümünü seç"
                    hideLabel
                    checked={allSelected}
                    onChange={() => {
                      selection.onChange(allSelected ? new Set() : new Set(allKeys));
                    }}
                  />
                </th>
              ) : null}
              {columns.map((column) => (
                <th key={column.key} scope="col" className={cellClass(column)}>
                  {column.label}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {rows.map((row) => {
              const key = rowKey(row);
              const selected = selection?.selected.has(key) ?? false;
              return (
                <tr
                  key={key}
                  className={classNames(
                    onRowClick && styles.clickable,
                    selected && styles.selected,
                  )}
                  tabIndex={onRowClick ? 0 : undefined}
                  onClick={
                    onRowClick
                      ? () => {
                          onRowClick(row);
                        }
                      : undefined
                  }
                  onKeyDown={
                    onRowClick
                      ? (event) => {
                          onRowKey(event, row);
                        }
                      : undefined
                  }
                >
                  {selection ? (
                    // Seçim hücresine tıklamak satırın detayını açmaz (kutunun etiketi de tıklanabilir).
                    <td
                      className={styles.check}
                      onClick={(event) => {
                        event.stopPropagation();
                      }}
                    >
                      <Checkbox
                        label={selection.rowLabel(row)}
                        hideLabel
                        checked={selected}
                        onChange={() => {
                          toggle(key);
                        }}
                      />
                    </td>
                  ) : null}
                  {columns.map((column) => (
                    <td key={column.key} className={cellClass(column)}>
                      {column.render(row)}
                    </td>
                  ))}
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>
      {footer ? <div className={styles.foot}>{footer}</div> : null}
    </div>
  );
}

export interface PersonCellProps {
  name: string;
  detail?: string;
  photoUrl?: string | null;
}

/** İlk kolon: avatar ve iki satır (ad, ayrıntı). */
export function PersonCell({ name, detail, photoUrl }: PersonCellProps) {
  return (
    <div className={styles.person}>
      <Avatar name={name} src={photoUrl ?? null} size="sm" />
      <div className={styles.personText}>
        <span className={styles.personName}>{name}</span>
        {detail ? <span className={styles.personDetail}>{detail}</span> : null}
      </div>
    </div>
  );
}
