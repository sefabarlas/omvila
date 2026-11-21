import { BookingButton, SwitchCard } from '@omvila/ui-web/booking';
import { formatDate, formatMoney, formatTime } from '@omvila/ui-web/format';
import { CalendarX, Package } from 'lucide-react';
import { useEffect, useState } from 'react';
import type { ReactNode } from 'react';
import { StateBanner } from '@/components/StateBanner';
import { privacyNoticeVersion } from '@/lib/legal';
import { createAppointment, listMyPackages } from '../booking-api';
import type {
  ItemRequest,
  Money,
  PublicAppointment,
  Slot,
  UsablePackageItem,
} from '../booking-api';
import { packageOptions, sessionsLeftLabel } from '../packages';
import { alternatives, messageFor, problemCode } from '../problem';
import styles from './steps.module.css';

interface ConfirmStepProps {
  branchId: string;
  timeZone: string;
  bookingToken: string;
  /** "Ayşe": yalnız bu markada kayıtlı müşteride (T-51 (9)). */
  knownFirstName: string | null;
  slot: Slot | null;
  items: ItemRequest[];
  sameStaffForAll: boolean;
  fullName: string;
  note: string;
  marketingConsent: boolean;
  /** Seçilen hizmetler (paket anahtarının adı: "Fön paketinizden düşülsün"). */
  services: { id: string; name: string }[];
  /** Özet kartı; paketten düşülen hizmetler "Paketten" yazar. */
  summary: (packaged: readonly string[]) => ReactNode;
  /**
   * Paketten düşülmeyen hizmetlerin kapora ipucu (katalog `depositHint`); yoksa null. Kesin tutar oluşturma
   * yanıtındadır (müşteriye göre çözülür) ve Havale ekranında gösterilir.
   */
  depositHintFor: (packaged: readonly string[]) => Money | null;
  onCreated: (appointment: PublicAppointment) => void;
  /** Saat doldu: alternatif saate geçiş (aynı gün). */
  onPickTime: (localTime: string) => void;
  onChangeTime: () => void;
  /** Jetonun süresi doldu: telefon adımına dönülür. */
  onTokenExpired: () => void;
  onBookingDisabled: () => void;
}

/** "14:45 ve 15:15", "14:45, 15:15 ve 15:45". */
function joinTimes(times: string[]): string {
  if (times.length <= 1) return times.join('');
  return `${times.slice(0, -1).join(', ')} ve ${times.at(-1) ?? ''}`;
}

/**
 * Son kontrol (Onay.png): özet, paket kutusu (müşterinin bu hizmet için kullanılabilir paketi varsa; KK6), kapora
 * kutusu (paketten düşülmeyen hizmette kapora varsa), "Randevuyu oluştur".
 * Saat dolduysa Durumlar.png "saat az önce doldu" deseni ve alternatif saat çipleri.
 */
export function ConfirmStep({
  branchId,
  timeZone,
  bookingToken,
  knownFirstName,
  slot,
  items,
  sameStaffForAll,
  fullName,
  note,
  marketingConsent,
  services,
  summary,
  depositHintFor,
  onCreated,
  onPickTime,
  onChangeTime,
  onTokenExpired,
  onBookingDisabled,
}: ConfirmStepProps) {
  const [failure, setFailure] = useState<{ error: unknown; time: string } | null>(null);
  const [busy, setBusy] = useState(false);
  const [packages, setPackages] = useState<UsablePackageItem[]>([]);
  const [packagesVersion, setPackagesVersion] = useState(0);
  // Hizmet → kapatılan paket anahtarı. Anahtar varsayılan açıktır (Onay.png); müşteri kapatabilir (T-100).
  const [declined, setDeclined] = useState<readonly string[]>([]);

  // Paket listesi isteğe bağlı bilgidir: hata veya 401'de kutu gösterilmez, randevu paketsiz oluşturulur.
  useEffect(() => {
    let live = true;
    listMyPackages(branchId, bookingToken).then(
      (items) => {
        if (live) setPackages(items);
      },
      () => {
        if (live) setPackages([]);
      },
    );
    return () => {
      live = false;
    };
  }, [branchId, bookingToken, packagesVersion]);

  const options = packageOptions(packages, services);
  const used = options.filter((option) => !declined.includes(option.serviceId));
  const packaged = used.map((option) => option.serviceId);
  const depositHint = depositHintFor(packaged);

  const submit = async () => {
    if (!slot) return;
    setBusy(true);
    setFailure(null);
    try {
      onCreated(
        await createAppointment(branchId, bookingToken, {
          items,
          sameStaffForAll,
          startUtc: slot.startUtc,
          fullName: fullName.trim(),
          customerNote: note.trim() || null,
          privacyNoticeAcknowledged: true,
          privacyNoticeVersion,
          marketingConsent,
          ...(used.length > 0
            ? {
                usePackageItems: used.map((option) => ({
                  serviceId: option.serviceId,
                  customerPackageItemId: option.item.customerPackageItemId,
                })),
              }
            : {}),
        }),
      );
    } catch (error) {
      const code = problemCode(error);
      if (code === 'unauthorized' || code === 'token_expired') onTokenExpired();
      else if (code === 'booking_disabled') onBookingDisabled();
      else if (code === 'package_session_unavailable') {
        // Seans bu arada kullanılmış veya süresi geçmiş: liste yenilenir, hata metni gösterilir.
        setPackagesVersion((value) => value + 1);
        setFailure({ error, time: slot.localTime });
      } else setFailure({ error, time: slot.localTime });
    } finally {
      setBusy(false);
    }
  };

  const taken = failure && problemCode(failure.error) === 'slot_taken';
  const alternativeTimes = taken
    ? alternatives(failure.error).map((utc) => formatTime(utc, timeZone))
    : [];

  return (
    <div className={styles.stack}>
      <p className={[styles.text, styles.welcome].join(' ')}>
        {knownFirstName ? (
          <>
            Tekrar hoş geldiniz, <strong>{knownFirstName}</strong>.{' '}
          </>
        ) : null}
        Randevunuzu onaylamadan önce son bir kez bakın.
      </p>
      {summary(packaged)}
      {options.map((option) => (
        <SwitchCard
          key={option.serviceId}
          icon={<Package size={20} />}
          label={`${option.serviceName} paketinizden düşülsün`}
          description={[
            sessionsLeftLabel(option.item),
            option.item.expiresOn
              ? `son kullanım ${formatDate(option.item.expiresOn, 'dayMonth')}`
              : null,
          ]
            .filter(Boolean)
            .join(' · ')}
          checked={!declined.includes(option.serviceId)}
          onCheckedChange={(checked) => {
            setDeclined((current) =>
              checked
                ? current.filter((id) => id !== option.serviceId)
                : [...current, option.serviceId],
            );
          }}
        />
      ))}
      {depositHint ? (
        <section className={styles.deposit} aria-labelledby="kapora-kutusu">
          <p className={styles.depositHead}>
            <span id="kapora-kutusu">Kapora</span>
            <span className={styles.depositAmount}>{formatMoney(depositHint)}</span>
          </p>
          <p>
            Bu randevu {formatMoney(depositHint)} kapora ile kesinleşir. Randevuyu oluşturunca
            salonun IBAN bilgisini göstereceğiz; havaleyi belirtilen süre içinde yapmanız gerekir.
            Kapora salondaki tutardan düşülür.
          </p>
        </section>
      ) : null}
      {taken ? (
        <StateBanner
          tone="hata"
          icon={<CalendarX size={20} />}
          title={`${failure.time} artık boş değil.`}
          description={
            alternativeTimes.length > 0
              ? `Siz seçerken başka biri aldı. En yakın boş saatler ${joinTimes(alternativeTimes)}.`
              : 'Siz seçerken başka biri aldı. Başka bir saat seçin.'
          }
        >
          {alternativeTimes.map((time) => (
            <BookingButton
              key={time}
              variant="secondary"
              size="sm"
              onClick={() => {
                setFailure(null);
                onPickTime(time);
              }}
            >
              {time}
            </BookingButton>
          ))}
          {alternativeTimes.length === 0 ? (
            <BookingButton variant="secondary" size="sm" onClick={onChangeTime}>
              Saati değiştir
            </BookingButton>
          ) : null}
        </StateBanner>
      ) : failure ? (
        <p className={styles.note} role="alert">
          {messageFor(failure.error)}
        </p>
      ) : null}
      <BookingButton
        block
        disabled={!slot || Boolean(taken)}
        loading={busy}
        loadingLabel="Oluşturuluyor…"
        onClick={() => {
          void submit();
        }}
      >
        Randevuyu oluştur
      </BookingButton>
      <p className={[styles.note, styles.center].join(' ')}>
        {depositHint
          ? 'Ödeme sayfadan alınmaz; havaleyi kendi bankanızdan yaparsınız.'
          : 'Ödeme sayfadan alınmaz; ücreti salonda ödersiniz.'}
      </p>
    </div>
  );
}
