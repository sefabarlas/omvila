import { formatDate, formatTime } from '../format/date';
import { withTimeSuffix } from '../format/suffix';
import { branchToday, daysBetween, toBranchDate } from '../time';
import type { Clock } from '../time';

// Şubenin açık/kapalı satırı (rezervasyon-sayfasi/Main, Sube): "Şu an açık · 20:00'ye kadar",
// "Bugün kapalı · Yarın 10:00'da açılır". Kaynak: PublicBrand.branches[] (openNow, todayClosesAt, nextOpeningUtc).

export interface BranchOpenInfo {
  openNow: boolean;
  /** Şubenin yerel saati, "20:00". */
  todayClosesAt?: string | null;
  nextOpeningUtc?: string | null;
  timeZone: string;
}

export interface OpenStatus {
  open: boolean;
  label: string;
}

/**
 * Açıksa kapanış saatiyle; kapalıysa bir sonraki açılış (bugün, yarın veya gün adı) ile.
 * Bir sonraki açılış bugünse "Şu an kapalı · Bugün 10:00'da açılır" yazılır.
 */
export function branchOpenStatus(branch: BranchOpenInfo, clock: Clock): OpenStatus {
  if (branch.openNow) {
    const until = branch.todayClosesAt
      ? ` · ${withTimeSuffix(branch.todayClosesAt, 'dative')} kadar`
      : '';
    return { open: true, label: `Şu an açık${until}` };
  }

  if (!branch.nextOpeningUtc) {
    return { open: false, label: 'Şu an kapalı' };
  }

  const today = branchToday(clock, branch.timeZone);
  const date = toBranchDate(new Date(branch.nextOpeningUtc), branch.timeZone);
  const diff = daysBetween(today, date);
  const day = diff === 0 ? 'Bugün' : diff === 1 ? 'Yarın' : formatDate(date, 'weekday');
  const time = withTimeSuffix(formatTime(branch.nextOpeningUtc, branch.timeZone), 'locative');
  return {
    open: false,
    label: `${diff === 0 ? 'Şu an kapalı' : 'Bugün kapalı'} · ${day} ${time} açılır`,
  };
}
