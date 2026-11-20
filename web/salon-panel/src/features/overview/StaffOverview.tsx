import { formatDate } from '@omvila/ui-web/format';
import { Banner, Bento, BentoBlock, EmptyState } from '@omvila/ui-web/panel';
import { branchToday } from '@omvila/ui-web/time';
import { CalendarCheck, Check, Lock } from 'lucide-react';
import { useSelectedBranch } from '@/app/branch';
import { useClock } from '@/app/clock';
import { defaultTimeZone } from '@/app/config';
import { useSession } from '@/app/session';
import { Page } from '@/shared/Page';
import { firstName } from './greeting';
import styles from './Overview.module.css';

/** Kapalı alanlar: bu izinlerden biri yoksa "kapalı" satırında anılır (roller-yetkiler.md). */
const closedAreas = [
  { permission: 'payment.collect', label: 'kasa' },
  { permission: 'report.view', label: 'raporlar' },
  { permission: 'service.manage', label: 'hizmet' },
  { permission: 'settings.manage', label: 'ayarlar' },
] as const;

function joinTurkish(words: readonly string[]): string {
  return words.length <= 1
    ? (words[0] ?? '')
    : `${words.slice(0, -1).join(', ')} ve ${words.at(-1) ?? ''}`;
}

/**
 * Personel görünümü (salon-paneli/PersonelRolu, kabul kriteri 10): "Merhaba Zeynep", kendi günü ve rolün kapsamı.
 * Randevu ve prim verisi Dilim 2 ve 6'da; bloklar bugün gerçekten 0/— (sahte sayı yok). "Randevu ekle" D2'de.
 */
export function StaffOverview() {
  const clock = useClock();
  const { session, can } = useSession();
  const { branchId } = useSelectedBranch();
  const branchName =
    session.branches.find((b) => b.id === branchId)?.name ?? session.branches[0]?.name ?? '';
  const today = branchToday(clock, defaultTimeZone);
  const name = firstName(session.user.fullName);
  const closed = closedAreas.filter((area) => !can(area.permission)).map((area) => area.label);
  const scope = [
    can('calendar.view') ? 'Kendi takvimini ve randevularını yönetir' : null,
    can('walkin.manage') ? 'Kapıda sıraya müşteri alır' : null,
    can('commission.view') ? 'Kendi primini ve puanını görür' : null,
  ].filter((line): line is string => line !== null);

  return (
    <Page
      title={name ? `Merhaba ${name}` : 'Merhaba'}
      context={`${formatDate(today, 'panel')} · ${branchName} · personel görünümü`}
    >
      <Bento label="Özet">
        <BentoBlock tone="marka" label="Bugün" value="0" sub="randevu" />
        <BentoBlock tone="kum" label="Sıradaki" value="—" sub="randevu yok" />
        {can('commission.view') ? (
          <BentoBlock tone="vurgu" label="Bu ay primim" value="—" sub="tamamlanan randevu yok" />
        ) : null}
        <BentoBlock label="Puanım" value="—" sub="değerlendirme yok" />
      </Bento>
      <div className={styles.staffGrid}>
        <section className={styles.staffCard} aria-labelledby="bugunku-randevular">
          <h2 id="bugunku-randevular" className={styles.listTitle}>
            Bugünkü randevularım
          </h2>
          <EmptyState
            icon={<CalendarCheck size={24} />}
            headingLevel={3}
            title="Bugün randevunuz yok"
            description="Size atanan randevular burada saat sırasıyla görünür."
          />
        </section>
        <div className={styles.staffSide}>
          <section className={styles.staffCard} aria-labelledby="bu-rolde">
            <h2 id="bu-rolde" className={styles.listTitle}>
              Bu rolde
            </h2>
            <ul className={styles.scope}>
              {scope.map((line) => (
                <li key={line}>
                  <Check size={16} aria-hidden="true" />
                  {line}
                </li>
              ))}
              {closed.length > 0 ? (
                <li className={styles.scopeClosed}>
                  <Lock size={16} aria-hidden="true" />
                  {`${joinTurkish(closed).replace(/^./, (c) => c.toLocaleUpperCase('tr'))} kapalı`}
                </li>
              ) : null}
            </ul>
          </section>
          {can('appointment.approve') ? null : (
            <Banner>Onay bekleyen talepleri yalnızca yönetici ve işletme sahibi onaylar.</Banner>
          )}
        </div>
      </div>
    </Page>
  );
}
