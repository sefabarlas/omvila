import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatDate, formatMoney, formatTime } from '@omvila/ui-web/format';
import { Bento, BentoBlock, Button, EmptyState, Skeleton } from '@omvila/ui-web/panel';
import { addDays, branchToday, toBranchDate } from '@omvila/ui-web/time';
import { useQuery } from '@tanstack/react-query';
import { UserRound } from 'lucide-react';
import { useState } from 'react';
import { useApi } from '@/app/api-context';
import { useClock } from '@/app/clock';
import { defaultTimeZone } from '@/app/config';
import { weekStart } from '@/features/calendar/calendar-view';
import { Page } from '@/shared/Page';
import { myCommissionQuery } from './api';
import type { MyCommissionEntry, MyCommissionSummary } from './api';
import { BarChart } from './BarChart';
import { formatPercent, formatRating, periodLabel, relativeDay } from './period';
import styles from './Reports.module.css';

/** "Son primler"in ilk görünen satır sayısı (işletme uygulaması Profil ile aynı düzen; "Tümünü göster" hepsini açar). */
const visibleEntries = 5;

/**
 * Primim (personel, commission.view; manageGetMyCommissionSummary): işletme uygulaması Profil'deki prim içeriğinin web
 * düzeni (ay toplamı, Randevu / Puan / Gelmedi, "Bu hafta" grafiği, "Son primler"). Panel tasarımı olmadığı için
 * Raporlar desenine yakın (T-122). Yalnız oturumdaki kullanıcının kendi primi; personel kaydı yoksa 404.
 */
export function MyCommissionPage() {
  const api = useApi();
  const clock = useClock();
  const month = branchToday(clock, defaultTimeZone).slice(0, 7);
  const query = useQuery(myCommissionQuery(api, month));
  const summary = query.data;

  return (
    <Page
      title="Primim"
      {...(summary
        ? { context: `${summary.staffName} · ${periodLabel(summary.from, summary.to)}` }
        : {})}
    >
      {query.isPending ? (
        <>
          <Skeleton variant="block" height="120px" />
          <Skeleton variant="block" height="280px" />
        </>
      ) : query.isError ? (
        <section className={styles.card} aria-label="Hata">
          {query.error instanceof ApiProblem && query.error.status === 404 ? (
            <EmptyState
              icon={<UserRound size={24} />}
              headingLevel={2}
              title="Personel kaydınız yok"
              description="Prim, personel kaydı olan kullanıcılar için tamamlanan randevulardan hesaplanır."
            />
          ) : (
            <EmptyState
              headingLevel={2}
              title="Primim yüklenemedi"
              description={errorMessage(
                query.error instanceof ApiProblem ? query.error.code : undefined,
              )}
              actions={<Button onClick={() => void query.refetch()}>Tekrar dene</Button>}
            />
          )}
        </section>
      ) : summary ? (
        <Summary summary={summary} />
      ) : null}
    </Page>
  );
}

function Summary({ summary: s }: { summary: MyCommissionSummary }) {
  const [all, setAll] = useState(false);
  const money = (amountMinor: number) => formatMoney({ amountMinor, currency: s.currency });
  const entries = all ? s.entries : s.entries.slice(0, visibleEntries);
  const monday = weekStart(s.to);
  const busiest = s.thisWeek.reduce<(typeof s.thisWeek)[number] | undefined>(
    (best, day) => (day.appointmentCount > (best?.appointmentCount ?? 0) ? day : best),
    undefined,
  );
  const monthName = formatDate(s.from, 'monthYear').split(' ')[0] ?? '';

  return (
    <>
      <Bento label="Özet">
        <BentoBlock
          tone="marka"
          label={`${monthName} primim`}
          value={money(s.totalMinor)}
          sub={`Hizmet geliri ${money(s.serviceRevenueMinor)}`}
        />
        <BentoBlock
          tone="kum"
          label="Randevu"
          value={String(s.appointmentCount)}
          sub="bu ay tamamlanan"
        />
        <BentoBlock
          label="Puan"
          value={s.averageRating === null ? '—' : formatRating(s.averageRating)}
          sub={s.averageRating === null ? 'değerlendirme yok' : 'son 90 gün'}
        />
        <BentoBlock
          label="Gelmedi"
          value={s.noShowPercent === null ? '—' : formatPercent(s.noShowPercent)}
          sub="bu ay sonuçlanan randevularda"
        />
      </Bento>
      <div className={styles.grid}>
        <section className={styles.card} aria-labelledby="bu-hafta">
          <div className={styles.cardHead}>
            <h2 id="bu-hafta" className={styles.cardTitle}>
              Bu hafta
            </h2>
            {busiest ? (
              <span className={styles.muted}>
                {formatDate(busiest.date, 'weekdayShort')} en yoğun
              </span>
            ) : null}
          </div>
          <BarChart
            caption="Bu haftanın primi"
            valueHeading="Prim"
            currency={s.currency}
            tone="yumusak"
            bars={Array.from({ length: 7 }, (_, i) => {
              const day = addDays(monday, i);
              const row = s.thisWeek.find((d) => d.date === day);
              return {
                day,
                amountMinor: row?.amountMinor ?? 0,
                future: !row && day > s.to,
                highlight: row !== undefined && row === busiest,
              };
            })}
          />
        </section>
        <section className={styles.card} aria-labelledby="son-primler">
          <div className={styles.cardHead}>
            <h2 id="son-primler" className={styles.cardTitle}>
              Son primler
            </h2>
            {s.entries.length > visibleEntries && !all ? (
              <Button
                size="sm"
                variant="ghost"
                onClick={() => {
                  setAll(true);
                }}
              >
                Tümünü göster
              </Button>
            ) : null}
          </div>
          {s.entries.length === 0 ? (
            <p className={styles.note}>Bu ay henüz prim yok.</p>
          ) : (
            <ul className={styles.entries}>
              {entries.map((entry) => (
                <EntryRow
                  key={entry.id}
                  entry={entry}
                  today={s.to}
                  timeZone={s.timeZone}
                  money={money}
                />
              ))}
            </ul>
          )}
          <p className={styles.note}>
            {s.nextAccrual
              ? `Prim randevu tamamlandığında hak edilir. ${s.nextAccrual.customerName} randevusu bitince eklenecek.`
              : 'Prim randevu tamamlandığında hak edilir.'}
          </p>
        </section>
      </div>
    </>
  );
}

function EntryRow({
  entry,
  today,
  timeZone,
  money,
}: {
  entry: MyCommissionEntry;
  today: string;
  timeZone: string;
  money: (amountMinor: number) => string;
}) {
  const negative = entry.status === 'Reversed' || entry.amountMinor < 0;
  const day = relativeDay(toBranchDate(new Date(entry.occurredUtc), timeZone), today);
  return (
    <li className={styles.entry}>
      <span className={styles.entryText}>
        <span className={styles.entryName}>{entry.customerName}</span>
        <span className={styles.entryDetail}>
          {day} {formatTime(entry.occurredUtc, timeZone)} · {entry.description}
        </span>
      </span>
      <span className={negative ? styles.minus : styles.plus}>
        {negative ? '−' : '+'}
        {money(Math.abs(entry.amountMinor))}
      </span>
    </li>
  );
}
