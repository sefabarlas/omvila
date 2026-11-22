import { ApiProblem } from '@omvila/api-client-ts';
import {
  CalendarClosedDay,
  CalendarEmptyDay,
  CalendarGrid,
  WeekCalendar,
} from '@omvila/ui-web/calendar';
import type { CalendarDropTarget, CalendarMove, CalendarSlot } from '@omvila/ui-web/calendar';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatDate, formatTime } from '@omvila/ui-web/format';
import { Banner, Button, EmptyState, IconButton, Select, Skeleton } from '@omvila/ui-web/panel';
import { addDays, branchToday } from '@omvila/ui-web/time';
import { keepPreviousData, useQuery, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { CalendarPlus, ChevronLeft, ChevronRight, DoorOpen, Settings } from 'lucide-react';
import { useState } from 'react';
import { useApi } from '@/app/api-context';
import { useSelectedBranch } from '@/app/branch';
import { useClock } from '@/app/clock';
import { defaultTimeZone } from '@/app/config';
import { isPersonalView } from '@/app/menu';
import { useSession } from '@/app/session';
import { blocksNewWork } from '@/app/SubscriptionBanner';
import { NewAppointmentButton } from '@/features/appointments/NewAppointmentButton';
import { NewAppointmentModal } from '@/features/appointments/NewAppointmentModal';
import type { NewAppointmentPrefill } from '@/features/appointments/NewAppointmentModal';
import { ConflictBanner } from '@/shared/ConflictBanner';
import { OutsideHoursDialog } from '@/shared/OutsideHoursDialog';
import { walkInsQuery } from '@/features/walk-ins/api';
import { Page } from '@/shared/Page';
import { Segmented } from '@/shared/Segmented';
import { dative } from '@/shared/turkish';
import { calendarQuery, useMoveAppointment } from './api';
import type { CalendarRange, MoveInput } from './api';
import { AppointmentDrawer } from './AppointmentDrawer';
import styles from './Calendar.module.css';
import { useDayRevenue } from '@/features/cash/api';
import { CalendarSummaryBlocks } from './CalendarSummaryBlocks';
import {
  findDay,
  gridHours,
  toBlocks,
  toStaffColumns,
  unavailableRanges,
  visibleWeekDays,
  weekDates,
  weekLabel,
} from './calendar-view';
import { OpenDayModal } from './OpenDayModal';

export type CalendarMode = 'gun' | 'hafta';

export interface CalendarSearch {
  tarih?: string | undefined;
  gorunum?: CalendarMode | undefined;
  personel?: string | undefined;
  /** Açılacak randevu yan paneli (bildirimden). */
  randevu?: string | undefined;
}

type MoveProblem =
  | { kind: 'slot'; time: string }
  | { kind: 'conflict' }
  | { kind: 'confirm'; move: MoveInput; code: string };

const modes = [
  { value: 'gun', label: 'Gün' },
  { value: 'hafta', label: 'Hafta' },
] as const;

/**
 * Takvim (salon-paneli/Main, HaftaTakvim, TakvimBos, TakvimKapali): özet blokları, gün (personel kolonları) ve hafta
 * (tek personel) görünümü, randevu yan paneli, sürükle-bırak. Tarih ve görünüm adresin arama parametresindedir.
 * "Randevu ekle" ve boş alana tıklama yeni randevu ekranını açar (appointment.create); salt okunur abonelikte buton
 * pasif ve nedeni yazar, boş alan tıklanmaz (T-58).
 */
export function CalendarPage({ search }: { search: CalendarSearch }) {
  const api = useApi();
  const clock = useClock();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { session, can } = useSession();
  const { branchId: selectedBranch } = useSelectedBranch();
  const branchId = selectedBranch ?? session.branches[0]?.id ?? '';
  const branchName = session.branches.find((b) => b.id === branchId)?.name ?? '';
  const personal = isPersonalView(can);

  // Bugünün verisi özet blokları ve hafta görünümünün personel listesi için (gün görünümünde aynı sorgu).
  const initialToday = branchToday(clock, defaultTimeZone);
  const todayQuery = useQuery(
    calendarQuery(api, { branchId, from: initialToday, to: initialToday }),
  );
  const timeZone = todayQuery.data?.timeZone ?? defaultTimeZone;
  const today = branchToday(clock, timeZone);
  const date = search.tarih ?? today;
  const mode: CalendarMode = search.gorunum ?? 'gun';
  // Bugün bloğunda günün tahsilatı (kasa; yalnız payment.collect).
  const revenue = useDayRevenue(branchId, today, mode === 'gun' && date === today);
  const staffOptions = todayQuery.data?.staff ?? [];
  const ownStaffId = session.membership.staffId ?? undefined;
  const weekStaffId =
    search.personel ??
    (staffOptions.some((s) => s.staffId === ownStaffId) ? ownStaffId : staffOptions[0]?.staffId);
  const dates = mode === 'hafta' ? weekDates(date) : [date];
  const range: CalendarRange =
    mode === 'hafta'
      ? { branchId, from: dates[0] ?? date, to: dates.at(-1) ?? date, staffId: weekStaffId }
      : { branchId, from: date, to: date };
  const viewQuery = useQuery({
    ...calendarQuery(api, range),
    placeholderData: keepPreviousData,
    enabled: mode === 'gun' || weekStaffId !== undefined,
  });
  const view = viewQuery.data;
  const move = useMoveAppointment(range);
  const [problem, setProblem] = useState<MoveProblem | null>(null);
  const [selected, setSelected] = useState<string | null>(null);
  const [openingDay, setOpeningDay] = useState(false);
  const [creating, setCreating] = useState<NewAppointmentPrefill | null>(null);
  const canCreate = can('appointment.create');
  const managesWalkIns = can('walkin.manage');
  const walkIns = useQuery({
    ...walkInsQuery(api, branchId),
    enabled: managesWalkIns && branchId !== '',
  });
  const readOnly = blocksNewWork(session.subscription.status);

  // Bloğa tıklama yerel seçimdir; bildirimden gelen ?randevu= de yan paneli açar (kapanınca adresten silinir).
  const openAppointment = selected ?? search.randevu ?? null;
  const setSearch = (patch: Partial<CalendarSearch>) =>
    void navigate({ to: '/takvim', search: (prev) => ({ ...prev, ...patch }) });
  const step = mode === 'hafta' ? 7 : 1;
  const canMove = can('appointment.update');
  const staffNameOf = (id: string) =>
    view?.staff.find((s) => s.staffId === id)?.fullName ??
    staffOptions.find((s) => s.staffId === id)?.fullName ??
    '';
  const weekStaffName = weekStaffId ? staffNameOf(weekStaffId) : '';

  const runMove = (input: MoveInput) => {
    setProblem(null);
    move.mutate(input, {
      onError: (error) => {
        if (!(error instanceof ApiProblem)) {
          setProblem(null);
          return;
        }

        if (error.status === 412) {
          setProblem({ kind: 'conflict' });
        } else if (error.code === 'outside_working_hours' || error.code === 'staff_on_time_off') {
          setProblem({ kind: 'confirm', move: input, code: error.code });
        } else if (error.code === 'slot_taken') {
          setProblem({ kind: 'slot', time: formatTime(input.startUtc, timeZone) });
        } else {
          setProblem({ kind: 'slot', time: '' });
        }
      },
    });
  };

  const onMove = (target: CalendarMove) => {
    const block = view?.appointments.find((item) => item.itemId === target.itemId);
    if (!block) {
      return;
    }

    runMove({
      appointmentId: target.appointmentId,
      itemId: target.itemId,
      fromUtc: block.startUtc,
      startUtc: target.startUtc,
      staffId: target.staffId !== block.staffId ? target.staffId : undefined,
    });
  };

  const describeDrop = (target: CalendarDropTarget) => {
    const where = target.staffChanged
      ? `${dative(staffNameOf(target.staffId))} taşı`
      : mode === 'hafta'
        ? `${formatDate(target.date, 'panel')} gününe taşı`
        : 'Saati değiştir';
    return `${where} · ${target.startTime}–${target.endTime} · müşteriye SMS gider`;
  };

  const onSlotClick = (slot: CalendarSlot) => {
    setCreating({
      staffId: slot.staffId,
      date: slot.date,
      time: formatTime(slot.startUtc, view?.timeZone ?? timeZone),
    });
  };
  const slotProps = canCreate && !readOnly ? { onSlotClick } : {};

  const shownDates = mode === 'hafta' ? visibleWeekDays(view, dates) : dates;
  const hours = view ? gridHours(view, shownDates) : null;
  const day = findDay(view, date);
  const closedEmpty = mode === 'gun' && day?.isClosed === true && hours === null;
  const closedDay = mode === 'gun' && day?.isClosed === true;
  const openDefaults = findDay(todayQuery.data, today)?.branchIntervals ?? [];
  const heading = mode === 'hafta' ? weekLabel(shownDates) : formatDate(date, 'panel');
  const context =
    mode === 'hafta'
      ? [weekLabel(shownDates), weekStaffName, branchName].filter(Boolean).join(' · ')
      : `${formatDate(date, 'panel')} · ${branchName}`;
  const weekday = formatDate(date, 'panel').split(',')[0] ?? '';
  const nextWeekday = formatDate(addDays(date, 1), 'panel').split(',')[0] ?? '';

  return (
    <Page
      title={personal ? 'Takvimim' : 'Takvim'}
      context={context}
      {...(canCreate
        ? {
            action: (
              <NewAppointmentButton
                readOnly={readOnly}
                onClick={() => {
                  setCreating({ date });
                }}
              />
            ),
          }
        : {})}
    >
      {/* Özet yalnız bugünün gün görünümünde (Main); hafta, kapalı ve boş gün tasarımlarında yok. */}
      {mode === 'gun' && date === today ? (
        <CalendarSummaryBlocks
          view={todayQuery.data}
          now={clock.now()}
          showPending={can('appointment.approve')}
          walkIns={
            managesWalkIns
              ? {
                  waitingCount: walkIns.data?.waitingCount ?? null,
                  estimatedWaitMinutesForNew: walkIns.data?.estimatedWaitMinutesForNew ?? null,
                }
              : undefined
          }
          revenue={revenue}
        />
      ) : null}
      {problem?.kind === 'conflict' ? (
        <ConflictBanner
          onReload={() => {
            setProblem(null);
            void queryClient.invalidateQueries({ queryKey: ['calendar'] });
          }}
        />
      ) : null}
      {problem?.kind === 'slot' ? (
        <Banner
          tone="hata"
          title={
            problem.time ? `Saat dolu: ${problem.time} artık boş değil.` : 'Randevu taşınamadı.'
          }
          action={
            <Button
              size="sm"
              onClick={() => {
                setProblem(null);
              }}
            >
              Kapat
            </Button>
          }
        >
          Randevu eski yerine döndü. Başka bir saat veya personel seçin.
        </Banner>
      ) : null}
      {closedDay ? (
        <Banner title={date === today ? 'Salon bugün kapalı.' : 'Salon bu gün kapalı.'}>
          Online randevu sayfasında bu gün seçilemez.
        </Banner>
      ) : null}
      <div className={styles.toolbar}>
        <div className={styles.nav}>
          <Button
            size="sm"
            onClick={() => {
              setSearch({ tarih: undefined });
            }}
          >
            {mode === 'hafta' ? 'Bu hafta' : 'Bugün'}
          </Button>
          <IconButton
            size="sm"
            variant="secondary"
            label={mode === 'hafta' ? 'Önceki hafta' : 'Önceki gün'}
            icon={<ChevronLeft size={16} />}
            onClick={() => {
              setSearch({ tarih: addDays(date, -step) });
            }}
          />
          <IconButton
            size="sm"
            variant="secondary"
            label={mode === 'hafta' ? 'Sonraki hafta' : 'Sonraki gün'}
            icon={<ChevronRight size={16} />}
            onClick={() => {
              setSearch({ tarih: addDays(date, step) });
            }}
          />
          <h2 className={styles.heading} aria-live="polite">
            {heading}
          </h2>
        </div>
        <div className={styles.nav}>
          {managesWalkIns ? (
            <Button
              size="sm"
              icon={<DoorOpen size={16} aria-hidden="true" />}
              onClick={() => {
                void navigate({ to: '/takvim/kapida-sira' });
              }}
            >
              Kapıda sıra
            </Button>
          ) : null}
          {mode === 'hafta' && staffOptions.length > 1 ? (
            <Select
              className={styles.staffSelect}
              label="Personel"
              hideLabel
              value={weekStaffId ?? ''}
              options={staffOptions.map((s) => ({ value: s.staffId, label: s.fullName }))}
              onChange={(event) => {
                setSearch({ personel: event.target.value });
              }}
            />
          ) : null}
          <Segmented
            legend="Görünüm"
            name="gorunum"
            options={modes}
            value={mode}
            onChange={(value) => {
              setSearch({ gorunum: value === 'gun' ? undefined : value });
            }}
          />
        </div>
      </div>
      {viewQuery.isError ? (
        <EmptyState
          title="Takvim yüklenemedi"
          description={errorMessage(
            viewQuery.error instanceof ApiProblem ? viewQuery.error.code : undefined,
          )}
          actions={<Button onClick={() => void viewQuery.refetch()}>Tekrar dene</Button>}
        />
      ) : !view ? (
        <Skeleton variant="block" height="640px" />
      ) : closedEmpty ? (
        <CalendarClosedDay
          {...(can('settings.manage')
            ? {
                actions: (
                  <>
                    <Button
                      variant="primary"
                      icon={<CalendarPlus size={16} aria-hidden="true" />}
                      onClick={() => {
                        setOpeningDay(true);
                      }}
                    >
                      Yalnızca bu gün için aç
                    </Button>
                    <Button
                      icon={<Settings size={16} aria-hidden="true" />}
                      onClick={() => void navigate({ to: '/ayarlar/sube' })}
                    >
                      Çalışma saatleri
                    </Button>
                  </>
                ),
              }
            : { description: 'Bu güne randevu alınmaz.' })}
        />
      ) : hours === null ? (
        <CalendarEmptyDay
          title={mode === 'hafta' ? 'Bu hafta çalışma saati yok' : `${weekday} çalışma saati yok`}
          description="Bu gün için şube veya personel saati girilmemiş. Açık kalmak isterseniz özel gün olarak saat ekleyin."
          actions={
            <>
              {can('settings.manage') && mode === 'gun' ? (
                <Button
                  onClick={() => {
                    setOpeningDay(true);
                  }}
                >
                  Özel saat ekle
                </Button>
              ) : null}
              <Button
                variant="primary"
                onClick={() => {
                  setSearch({ tarih: addDays(date, step) });
                }}
              >
                {mode === 'hafta' ? 'Sonraki haftaya git' : `${dative(nextWeekday, false)} git`}
              </Button>
            </>
          }
        />
      ) : mode === 'hafta' ? (
        <WeekCalendar
          staff={{ id: weekStaffId ?? '', name: weekStaffName }}
          days={shownDates}
          timeZone={view.timeZone}
          openTime={hours.openTime}
          closeTime={hours.closeTime}
          blocks={toBlocks(view, canMove)}
          unavailable={unavailableRanges(view, shownDates, hours)}
          now={clock.now()}
          onBlockClick={setSelected}
          {...slotProps}
          {...(canMove ? { onMove, describeDrop } : {})}
        />
      ) : (
        <CalendarGrid
          date={date}
          staff={toStaffColumns(view)}
          timeZone={view.timeZone}
          openTime={hours.openTime}
          closeTime={hours.closeTime}
          blocks={toBlocks(view, canMove)}
          unavailable={unavailableRanges(view, shownDates, hours)}
          now={clock.now()}
          onBlockClick={setSelected}
          {...slotProps}
          {...(canMove ? { onMove, describeDrop } : {})}
        />
      )}
      {openAppointment ? (
        <AppointmentDrawer
          key={openAppointment}
          appointmentId={openAppointment}
          today={today}
          onClose={() => {
            setSelected(null);
            if (search.randevu) {
              setSearch({ randevu: undefined });
            }
          }}
        />
      ) : null}
      {creating ? (
        <NewAppointmentModal
          branchId={branchId}
          prefill={creating}
          onClose={() => {
            setCreating(null);
          }}
        />
      ) : null}
      {openingDay ? (
        <OpenDayModal
          branchId={branchId}
          date={date}
          defaults={openDefaults}
          onClose={() => {
            setOpeningDay(false);
          }}
        />
      ) : null}
      {problem?.kind === 'confirm' ? (
        <OutsideHoursDialog
          code={problem.code}
          confirmLabel="Yine de taşı"
          consequence="Yine de taşırsanız randevu bu saate yazılır."
          onCancel={() => {
            setProblem(null);
          }}
          onConfirm={() => {
            runMove({ ...problem.move, allowOutsideWorkingHours: true });
          }}
        />
      ) : null}
    </Page>
  );
}
