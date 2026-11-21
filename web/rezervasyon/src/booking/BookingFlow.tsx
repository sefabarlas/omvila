'use client';

import {
  ANY_STAFF,
  BookingButton,
  BookingSteps,
  SalonHeader,
  StickyFooter,
  branchOpenStatus,
  defaultBookingSteps,
} from '@omvila/ui-web/booking';
import { formatDate } from '@omvila/ui-web/format';
import { branchToday, systemClock } from '@omvila/ui-web/time';
import type { Clock } from '@omvila/ui-web/time';
import { ArrowLeft, CalendarX, WifiOff } from 'lucide-react';
import {
  Suspense,
  lazy,
  useCallback,
  useEffect,
  useRef,
  useState,
  useSyncExternalStore,
} from 'react';
import type { ReactNode } from 'react';
import { BookingDisabled } from '@/components/BookingDisabled';
import type { WaitlistChoice } from '@/waitlist/WaitlistForm';
import type { WaitlistEntry } from '@/waitlist/waitlist-api';
import { windowsLabel } from '@/waitlist/windows';
import { depositHintTotal } from '@/lib/deposit';
import { StateBanner } from '@/components/StateBanner';
import type { PublicBrand } from '@/lib/brand';
import { fetchCatalog, searchSlots } from './booking-api';
import type {
  BookingToken,
  OtpChallenge,
  Money,
  PublicAppointment,
  SlotSearchRequest,
} from './booking-api';
import { problemCode } from './problem';
import {
  effectiveSameStaff,
  emptySelection,
  itemRequests,
  parseSelection,
  reconcile,
  serializeSelection,
  staffComplete,
  withServices,
} from './selection';
import type { Selection, Step } from './selection';
import { AsideSummary, SummaryCard } from './SummaryCard';
import { BranchStep } from './steps/BranchStep';
import { emptyDetails } from './steps/DetailsStep';
import type { DetailsForm } from './steps/DetailsStep';
import { ServicesStep } from './steps/ServicesStep';
import { compactWhen, countLabel, longWhen, summarize, totalLabel } from './summary';
import { useDays } from './use-days';
import { useRemote } from './use-remote';
import styles from './BookingFlow.module.css';

// Hizmet adımı ilk boyamada gerekir; diğer adımlar ayrı parçalar halinde, katalog gelince arka planda yüklenir
// (salon sayfasının JS bütçesi, T-55/T-56).
const StaffStep = lazy(() => import('./steps/StaffStep').then((m) => ({ default: m.StaffStep })));
const TimeStep = lazy(() => import('./steps/TimeStep').then((m) => ({ default: m.TimeStep })));
const DetailsStep = lazy(() =>
  import('./steps/DetailsStep').then((m) => ({ default: m.DetailsStep })),
);
const CodeStep = lazy(() => import('./steps/CodeStep').then((m) => ({ default: m.CodeStep })));
const ConfirmStep = lazy(() =>
  import('./steps/ConfirmStep').then((m) => ({ default: m.ConfirmStep })),
);
const DoneStep = lazy(() => import('./steps/DoneStep').then((m) => ({ default: m.DoneStep })));
const RequestStep = lazy(() =>
  import('./steps/RequestStep').then((m) => ({ default: m.RequestStep })),
);
const DepositStep = lazy(() =>
  import('./steps/DepositStep').then((m) => ({ default: m.DepositStep })),
);
const WaitlistJoin = lazy(() =>
  import('@/waitlist/WaitlistJoin').then((m) => ({ default: m.WaitlistJoin })),
);
const WaitlistJoined = lazy(() =>
  import('@/waitlist/WaitlistJoin').then((m) => ({ default: m.WaitlistJoined })),
);

function preloadSteps() {
  void import('./steps/StaffStep');
  void import('./steps/TimeStep');
  void import('./steps/DetailsStep');
  void import('./steps/CodeStep');
  void import('./steps/ConfirmStep');
  void import('./steps/DoneStep');
  void import('./steps/RequestStep');
  void import('./steps/DepositStep');
  void import('@/waitlist/WaitlistJoin');
}

/** Bekleme listesi formunun seçimi ve dolu gün (telefon doğrulanana kadar bellekte). */
type WaitlistDraft = WaitlistChoice & { date: string };

const loadCatalog = (request: { branchId: string }) => fetchCatalog(request.branchId);
const loadSlots = (request: { branchId: string; body: SlotSearchRequest }) =>
  searchSlots(request.branchId, request.body);

const desktopQuery = '(min-width: 960px)';
function subscribeDesktop(onChange: () => void) {
  const media = typeof window.matchMedia === 'function' ? window.matchMedia(desktopQuery) : null;
  media?.addEventListener('change', onChange);
  return () => media?.removeEventListener('change', onChange);
}
/** Masaüstü düzeni (alt çubuk satır içi). Sunucuda ve ilk boyamada mobil. */
function useDesktop(): boolean {
  return useSyncExternalStore(
    subscribeDesktop,
    () => typeof window.matchMedia === 'function' && window.matchMedia(desktopQuery).matches,
    () => false,
  );
}

const stepTitles: Partial<Record<Step, string>> = {
  staff: 'Personel seçin',
  time: 'Gün ve saat',
  details: 'Bilgileriniz',
  confirm: 'Son kontrol',
};

/** Adres çubuğundan ilk seçim; tek şubede şube adımı atlanır, bilinmeyen şube şube adımına döner. */
function initialSelection(brand: PublicBrand, search: string): Selection {
  const parsed = parseSelection(search);
  const [only, ...rest] = brand.branches;
  if (only && rest.length === 0) {
    return {
      ...parsed,
      branchId: only.id,
      step: parsed.step === 'branch' ? 'services' : parsed.step,
    };
  }
  return brand.branches.some((branch) => branch.id === parsed.branchId)
    ? parsed
    : { ...emptySelection, step: 'branch' };
}

export interface BookingFlowProps {
  brand: PublicBrand;
  /** Sunucunun gördüğü arama dizesi ("?branch=…"); ilk çizim adresle aynı olsun diye. */
  initialSearch: string;
  /** "Şimdi" (testte sabit saat). */
  clock?: Clock;
}

/**
 * Online randevu akışı (Dilim 2): Şube → Hizmet → Personel → Saat → Bilgiler → Kod → Onay → Başarı.
 * Seçim adres çubuğunda (yenileme ve geri tuşu korunur); ad, telefon, not ve rezervasyon jetonu yalnız bellekte.
 */
export function BookingFlow({ brand, initialSearch, clock = systemClock }: BookingFlowProps) {
  const [raw, setRaw] = useState(() => initialSelection(brand, initialSearch));
  const [form, setForm] = useState<DetailsForm>(emptyDetails);
  const [challenge, setChallenge] = useState<{ challenge: OtpChallenge; phone: string } | null>(
    null,
  );
  const [token, setToken] = useState<(BookingToken & { phone: string }) | null>(null);
  const [appointment, setAppointment] = useState<PublicAppointment | null>(null);
  const [disabled, setDisabled] = useState(false);
  const [expiredNotice, setExpiredNotice] = useState(false);
  const [waitlist, setWaitlist] = useState<WaitlistDraft | null>(null);
  const [joined, setJoined] = useState<WaitlistEntry | null>(null);
  const desktop = useDesktop();
  const mainRef = useRef<HTMLElement>(null);

  useEffect(() => {
    const onPop = () => {
      setRaw(initialSelection(brand, window.location.search));
    };
    window.addEventListener('popstate', onPop);
    return () => {
      window.removeEventListener('popstate', onPop);
    };
  }, [brand]);

  const branch = brand.branches.find((candidate) => candidate.id === raw.branchId);
  const catalogQuery = useRemote(
    branch && raw.step !== 'branch' && brand.bookingEnabled ? { branchId: branch.id } : null,
    loadCatalog,
  );
  const catalog = catalogQuery.data;

  // Kataloğa göre düzeltilmiş seçim; bellekteki adımlar (kod, onay, başarı) durumları yoksa geri çekilir.
  let selection = raw;
  const waitlistDay = waitlist !== null && waitlist.date === raw.date && raw.time === null;
  if (catalog && raw.step !== 'branch' && raw.step !== 'done' && raw.step !== 'waitlisted') {
    selection = reconcile(raw, catalog, waitlistDay);
  }
  // Bekleme listesi kipi: dolu gün için form gönderildi, saat seçilmedi (bilgiler → kod → katılım).
  const waitlistMode =
    waitlist !== null && waitlist.date === selection.date && selection.time === null;
  if (
    ['details', 'code', 'confirm'].includes(selection.step) &&
    selection.time === null &&
    !waitlistMode
  ) {
    selection = { ...selection, step: 'time' };
  }
  if ((selection.step === 'code' && !challenge) || (selection.step === 'confirm' && !token)) {
    selection = { ...selection, step: 'details' };
  }
  if (selection.step === 'done' && !appointment) selection = { ...selection, step: 'services' };
  if (selection.step === 'waitlisted' && !joined) selection = { ...selection, step: 'services' };
  const step = selection.step;

  // Sonraki adımların parçaları ilk hizmet seçilince indirilir (ilk yüklemenin JS'ine girmez).
  const preloaded = useRef(false);
  const preload = () => {
    if (preloaded.current) return;
    preloaded.current = true;
    preloadSteps();
  };

  const timeZone = catalog?.timeZone ?? branch?.timeZone ?? 'Europe/Istanbul';
  const today = branchToday(clock, timeZone);
  const sameStaff = catalog ? effectiveSameStaff(selection, catalog) : true;
  const items = catalog ? itemRequests(selection, catalog) : [];
  const staffReady =
    catalog !== undefined && selection.serviceIds.length > 0 && staffComplete(selection, catalog);
  const afterTime = ['time', 'details', 'code', 'confirm'].includes(step);

  const days = useDays(
    staffReady && branch && step === 'time'
      ? { branchId: branch.id, items, sameStaffForAll: sameStaff }
      : null,
    today,
    selection.date,
  );
  const defaultDate =
    days.days.find((day) => day.hasAvailability)?.date ??
    days.days.find((day) => !day.isClosed)?.date ??
    null;
  const date = selection.date ?? (step === 'time' ? defaultDate : null);

  const slotsQuery = useRemote(
    staffReady && branch && date && afterTime
      ? { branchId: branch.id, body: { items, sameStaffForAll: sameStaff, date } }
      : null,
    loadSlots,
  );
  const slot = slotsQuery.data?.slots.find((item) => item.localTime === selection.time) ?? null;
  // Saat seçilmişken yanıtta artık yoksa (yenileme sonrası dolmuş) saat adımına dönülür.
  const lostTime =
    step !== 'time' && afterTime && slotsQuery.data !== undefined && slot === null
      ? selection.time
      : null;
  const shownStep: Step = lostTime ? 'time' : step;

  const summary = catalog ? summarize(catalog, selection, brand.showPrices, slot) : null;
  const bookingDisabled =
    !brand.bookingEnabled ||
    disabled ||
    [catalogQuery.error, days.error, slotsQuery.error].some(
      (error) => problemCode(error) === 'booking_disabled',
    );

  // Adım değişince odak ana bölgeye (ekran okuyucu yeni adımı okur, sayfa başa kayar); ilk açılışta değil.
  const previousStep = useRef(shownStep);
  useEffect(() => {
    if (previousStep.current === shownStep) return;
    previousStep.current = shownStep;
    mainRef.current?.focus();
  }, [shownStep]);

  const multiBranch = brand.branches.length > 1;
  const commit = useCallback(
    (next: Selection, mode: 'push' | 'replace') => {
      setRaw(next);
      const url = `${window.location.pathname}${serializeSelection(next, multiBranch)}`;
      if (mode === 'push') window.history.pushState({ omvilaStep: next.step }, '', url);
      else window.history.replaceState(window.history.state, '', url);
    },
    [multiBranch],
  );

  const base: Selection = { ...selection, step: shownStep, date: selection.date ?? date };
  const update = (patch: Partial<Selection>) => {
    commit({ ...base, ...patch }, 'replace');
  };
  const go = (next: Step, mode: 'push' | 'replace' = 'push') => {
    commit({ ...base, step: next }, mode);
  };

  const allowStaff = catalog?.allowStaffSelection ?? true;
  const previous: Partial<Record<Step, Step>> = {
    ...(multiBranch ? { services: 'branch' as const } : {}),
    staff: 'services',
    time: allowStaff ? 'staff' : 'services',
    details: 'time',
    code: 'details',
    confirm: 'details',
  };
  const back = () => {
    const target = previous[shownStep];
    if (!target) return;
    const state: unknown = window.history.state;
    if (typeof state === 'object' && state !== null && 'omvilaStep' in state) window.history.back();
    else go(target, 'replace');
  };
  const nextStep: Partial<Record<Step, Step>> = {
    services: allowStaff ? 'staff' : 'time',
    staff: 'time',
    time: 'details',
  };

  const progressSteps = allowStaff
    ? defaultBookingSteps
    : defaultBookingSteps.filter((label) => label !== 'Personel');
  const progressIndex: Partial<Record<Step, number>> = {
    services: 0,
    staff: 1,
    time: allowStaff ? 2 : 1,
    details: progressSteps.length - 1,
    confirm: progressSteps.length - 1,
  };

  const staffLabel = (() => {
    if (!catalog || !summary || !staffReady || !allowStaff) return null;
    const names = [...new Set(summary.lines.map((line) => line.staffName ?? 'Fark etmez'))];
    return names.join(', ');
  })();
  const singleStaffName =
    catalog && sameStaff && selection.staff[0] && selection.staff[0] !== ANY_STAFF
      ? (catalog.staff.find((member) => member.id === selection.staff[0])?.displayName ?? null)
      : null;

  const place = branch ? `${brand.name} · ${branch.addressLine}, ${branch.district}` : brand.name;
  // Kapora ipucu (bilgi amaçlı); kesin kapora oluşturma yanıtında, müşteriye göre (sureler-ve-ayarlar.md § Kapora).
  // Paketten düşülen kalemde kapora yok (KK8, publicCreateAppointment usePackageItems).
  const depositHintFor = (packaged: readonly string[] = []) =>
    catalog
      ? depositHintTotal(
          catalog.services.filter(
            (service) =>
              selection.serviceIds.includes(service.id) && !packaged.includes(service.id),
          ),
        )
      : null;
  const summaryCard = (
    action?: ReactNode,
    deposit: Money | null = null,
    packaged: readonly string[] = [],
  ) =>
    summary && slot && date ? (
      <SummaryCard
        summary={summary}
        when={longWhen(date, slot)}
        place={place}
        action={action}
        deposit={deposit}
        packaged={packaged}
      />
    ) : null;
  // Bekleme listesi bilgiler adımının özeti: hizmetler, dolu gün ve seçilen saat dilimleri.
  const waitlistCard =
    summary && waitlist ? (
      <SummaryCard
        summary={summary}
        when={`${formatDate(waitlist.date, 'panel')} · ${windowsLabel(waitlist.timeWindows)}`}
        place={`Bekleme listesi · ${place}`}
        action={
          <button
            type="button"
            className={styles.change}
            onClick={() => {
              go('time');
            }}
          >
            Değiştir
          </button>
        }
      />
    ) : null;

  const onTokenExpired = () => {
    setToken(null);
    setExpiredNotice(true);
    go('details', 'replace');
  };
  const onBookingDisabled = () => {
    setDisabled(true);
  };
  const onJoined = (entry: WaitlistEntry) => {
    setJoined(entry);
    setWaitlist(null);
    setRaw({ ...base, step: 'waitlisted' });
    window.history.replaceState(window.history.state, '', window.location.pathname);
  };
  const waitlistPreferences = (draft: WaitlistDraft) => {
    // Tek personel seçildiyse tercih o kişi; "Fark etmez" veya hizmet başına ayrı kişi seçildiyse tercih yok (T-100).
    const first = selection.staff[0];
    const preferred = sameStaff && first && first !== ANY_STAFF ? first : null;
    return {
      serviceIds: selection.serviceIds,
      preferredStaffId: preferred,
      acceptOtherStaff: preferred ? draft.acceptOtherStaff : true,
      dates: [draft.date],
      timeWindows: draft.timeWindows,
    };
  };

  const loadError = (onRetry: () => void) => (
    <StateBanner
      tone="hata"
      icon={<WifiOff size={20} />}
      title="Bilgiler yüklenemedi."
      description="Bağlantınızı kontrol edip tekrar deneyin."
    >
      <BookingButton variant="secondary" size="sm" onClick={onRetry}>
        Tekrar dene
      </BookingButton>
    </StateBanner>
  );
  const skeleton = (
    <div className={styles.skeletonList} aria-busy="true" aria-label="Yükleniyor">
      <span className={styles.skeleton} />
      <span className={styles.skeleton} />
      <span className={styles.skeleton} />
    </div>
  );

  const content = (() => {
    if (bookingDisabled) return <BookingDisabled brand={brand} />;
    if (shownStep === 'branch' || !branch) {
      return (
        <BranchStep
          brand={brand}
          clock={clock}
          onSelect={(branchId) => {
            commit({ ...emptySelection, branchId, step: 'services' }, 'push');
          }}
        />
      );
    }
    if (catalogQuery.error) return loadError(catalogQuery.reload);
    if (!catalog || !summary) return skeleton;
    if (catalog.services.length === 0) {
      return (
        <p className={styles.empty} role="status">
          Bu şubede online alınabilen hizmet yok. Randevu için salonu arayabilirsiniz.
        </p>
      );
    }

    switch (shownStep) {
      case 'services':
        return (
          <ServicesStep
            catalog={catalog}
            selected={selection.serviceIds}
            showPrices={brand.showPrices}
            onChange={(serviceIds) => {
              preload();
              commit(withServices(base, serviceIds), 'replace');
            }}
          />
        );
      case 'staff':
        return (
          <StaffStep
            catalog={catalog}
            selection={selection}
            showPrices={brand.showPrices}
            onChange={(staff, same) => {
              update({ staff, sameStaff: same, date: null, time: null });
            }}
          />
        );
      case 'time':
        if (days.error) return loadError(days.reload);
        if (days.loading) return skeleton;
        return (
          <>
            {lostTime ? (
              <StateBanner
                tone="hata"
                icon={<CalendarX size={20} />}
                title={`${lostTime} artık boş değil.`}
                description="Siz seçerken başka biri aldı. Başka bir saat seçin."
              />
            ) : null}
            {slotsQuery.error ? (
              loadError(slotsQuery.reload)
            ) : (
              <TimeStep
                days={days.days}
                maxDate={days.maxDate}
                nextAvailableUtc={days.nextAvailableUtc}
                today={today}
                timeZone={timeZone}
                date={date}
                time={lostTime ? null : selection.time}
                slots={slotsQuery.data}
                staffName={singleStaffName}
                onSelectDate={(value) => {
                  update({ date: value, time: null });
                }}
                onSelectTime={(value) => {
                  update({ date, time: value });
                }}
                onJump={(value, time) => {
                  update({ date: value, time });
                }}
                onJoinWaitlist={(choice) => {
                  if (!date) return;
                  preload();
                  setWaitlist({ ...choice, date });
                  commit({ ...base, date, time: null, step: 'details' }, 'push');
                }}
              />
            )}
          </>
        );
      case 'details':
        return (
          <>
            {expiredNotice ? (
              <p className={styles.notice} role="status">
                Doğrulamanın süresi doldu. Kodu yeniden isteyin.
              </p>
            ) : null}
            <DetailsStep
              brandName={brand.name}
              branchId={branch.id}
              form={form}
              onFormChange={setForm}
              existingToken={token}
              waitlist={waitlistMode}
              summary={
                waitlistMode
                  ? waitlistCard
                  : summaryCard(
                      <button
                        type="button"
                        className={styles.change}
                        onClick={() => {
                          go('time');
                        }}
                      >
                        Değiştir
                      </button>,
                    )
              }
              onCodeSent={(next, phone) => {
                setChallenge({ challenge: next, phone });
                go('code');
              }}
              onVerified={(next, phone) => {
                setToken({ ...next, phone });
                setExpiredNotice(false);
                go('confirm');
              }}
            />
          </>
        );
      case 'code':
        return challenge ? (
          <CodeStep
            branchId={branch.id}
            phone={challenge.phone}
            challenge={challenge.challenge}
            onNewChallenge={(next) => {
              setChallenge({ challenge: next, phone: challenge.phone });
            }}
            onChangeNumber={() => {
              go('details', 'replace');
            }}
            onVerified={(next) => {
              setToken({ ...next, phone: challenge.phone });
              setExpiredNotice(false);
              // Kod adımı geçmişte kalmaz: onaydan geri tuşu bilgilere döner.
              go('confirm', 'replace');
            }}
          />
        ) : null;
      case 'confirm':
        if (token && waitlistMode) {
          return (
            <WaitlistJoin
              branchId={branch.id}
              bookingToken={token.bookingToken}
              preferences={waitlistPreferences(waitlist)}
              fullName={form.fullName}
              onJoined={onJoined}
              onTokenExpired={onTokenExpired}
              onBookingDisabled={onBookingDisabled}
            />
          );
        }
        return token ? (
          <ConfirmStep
            branchId={branch.id}
            timeZone={timeZone}
            bookingToken={token.bookingToken}
            knownFirstName={token.knownCustomerFirstName ?? null}
            slot={slot}
            items={items.map((item) => {
              // "Fark etmez": ekranda gösterilen atanmış kişi gönderilir (kriter 6); personel seçimi kapalıysa
              // sunucu atar (T-51 (5)).
              const assigned = slot?.items.find(
                (part) => part.serviceId === item.serviceId,
              )?.staffId;
              return allowStaff && item.staffId === null && assigned
                ? { ...item, staffId: assigned }
                : item;
            })}
            sameStaffForAll={sameStaff}
            fullName={form.fullName}
            note={form.note}
            marketingConsent={form.marketing}
            services={summary.lines.map((line) => ({ id: line.serviceId, name: line.name }))}
            summary={(packaged) => summaryCard(undefined, depositHintFor(packaged), packaged)}
            depositHintFor={depositHintFor}
            onCreated={(created) => {
              setAppointment(created);
              setRaw({ ...base, step: 'done' });
              window.history.replaceState(window.history.state, '', window.location.pathname);
            }}
            onPickTime={(time) => {
              update({ time });
              slotsQuery.reload();
            }}
            onChangeTime={() => {
              slotsQuery.reload();
              go('time');
            }}
            onTokenExpired={onTokenExpired}
            onBookingDisabled={onBookingDisabled}
          />
        ) : null;
      case 'waitlisted':
        return joined ? <WaitlistJoined entry={joined} brandSlug={brand.slug} /> : null;
      case 'done':
        if (!appointment) return null;
        // Kapora isteyen randevu Havale ekranına (KK3); onay gerektiren salonda talep (A1); onaylı randevu Başarı.
        if (
          appointment.displayStatus === 'DepositPending' ||
          appointment.cancellation?.reason === 'DepositNotReceived'
        ) {
          return <DepositStep appointment={appointment} clock={clock} onChange={setAppointment} />;
        }
        return appointment.displayStatus === 'PendingApproval' ? (
          <RequestStep
            appointment={appointment}
            phoneMasked={token?.phoneMasked ?? ''}
            showPrices={brand.showPrices}
            clock={clock}
          />
        ) : (
          <DoneStep appointment={appointment} phoneMasked={token?.phoneMasked ?? ''} />
        );
      default:
        return null;
    }
  })();

  // Dolu günde alt çubuk yok: ekranın eylemi bekleme listesi formudur (DoluGun.png).
  const fullDay = days.days.some(
    (day) => day.date === date && !day.hasAvailability && !day.isClosed,
  );
  const showFooter =
    !bookingDisabled &&
    !(shownStep === 'time' && fullDay) &&
    catalog !== undefined &&
    summary !== null &&
    catalog.services.length > 0 &&
    (shownStep === 'services' || shownStep === 'staff' || shownStep === 'time');
  const canContinue =
    shownStep === 'services'
      ? selection.serviceIds.length > 0
      : shownStep === 'staff'
        ? staffReady
        : slot !== null;
  const total = summary ? totalLabel(summary) : null;
  const when = slot && date ? compactWhen(date, slot) : null;
  const hasLines = summary !== null && summary.lines.length > 0;
  const showHeaderOnMobile = shownStep === 'services' || shownStep === 'branch';
  const showProgress = !bookingDisabled && progressIndex[shownStep] !== undefined;
  const title = waitlistMode && shownStep === 'confirm' ? 'Bekleme listesi' : stepTitles[shownStep];
  const showTopBar =
    !bookingDisabled && previous[shownStep] !== undefined && shownStep !== 'services';

  return (
    <div className={styles.page}>
      <div className={styles.topbar}>
        <span className={styles.topbarBrand}>
          <strong>{brand.name}</strong>
          {branch ? <span>{branch.name} şubesi</span> : null}
        </span>
        <span className={styles.topbarNote}>OMVILA ile randevu</span>
      </div>
      <div className={styles.layout}>
        <div
          className={[styles.headerArea, !showHeaderOnMobile && styles.headerCollapsed]
            .filter(Boolean)
            .join(' ')}
        >
          <SalonHeader
            name={brand.name}
            logoUrl={brand.logoUrl ?? null}
            coverColor={brand.coverColor}
            category={[brand.industryLabel, brand.shortDescription].filter(Boolean).join(' · ')}
            rating={brand.rating ?? null}
            openStatus={branch ? branchOpenStatus(branch, clock) : null}
            address={
              branch
                ? `${branch.addressLine}, ${branch.district} · ${branch.city}`
                : `${String(brand.branches.length)} şube`
            }
          />
        </div>
        {catalog &&
        summary &&
        shownStep !== 'branch' &&
        shownStep !== 'done' &&
        shownStep !== 'waitlisted' &&
        !bookingDisabled ? (
          <aside className={styles.aside}>
            <AsideSummary summary={summary} staff={staffLabel} when={when} showStaff={allowStaff} />
          </aside>
        ) : null}
        <main ref={mainRef} className={styles.main} tabIndex={-1}>
          <div className={styles.content}>
            {showTopBar ? (
              <div className={styles.stepBar}>
                <button type="button" className={styles.back} aria-label="Geri" onClick={back}>
                  <ArrowLeft size={24} aria-hidden="true" />
                </button>
                {title ? <h2 className={styles.stepTitle}>{title}</h2> : null}
              </div>
            ) : null}
            {showProgress ? (
              <BookingSteps steps={progressSteps} current={progressIndex[shownStep] ?? 0} />
            ) : null}
            {shownStep === 'services' && branch && !bookingDisabled ? (
              // Katalog gelmeden sunucuda çizilir: sayfanın en büyük metni (LCP) beklemez.
              <h2 className={styles.title}>Hizmet seçin</h2>
            ) : null}
            <Suspense fallback={skeleton}>{content}</Suspense>
          </div>
          {showFooter ? (
            <StickyFooter
              layout={desktop ? 'inline' : 'sticky'}
              {...(desktop ? { className: styles.footerInline } : {})}
              summary={hasLines ? (total ?? countLabel(summary)) : undefined}
              detail={
                hasLines
                  ? shownStep === 'time' && when
                    ? when
                    : total
                      ? countLabel(summary)
                      : undefined
                  : undefined
              }
              action={
                <BookingButton
                  disabled={!canContinue}
                  onClick={() => {
                    const next = nextStep[shownStep];
                    if (next) go(next);
                  }}
                >
                  Devam et
                </BookingButton>
              }
            />
          ) : null}
        </main>
      </div>
    </div>
  );
}
