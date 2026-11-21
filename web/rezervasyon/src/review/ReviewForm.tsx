'use client';

import { BookingAvatar, BookingButton } from '@omvila/ui-web/booking';
import { formatDate } from '@omvila/ui-web/format';
import { toBranchDate } from '@omvila/ui-web/time';
import { Check, Star, WifiOff } from 'lucide-react';
import { useEffect, useId, useRef, useState } from 'react';
import { LinkMissing } from '@/components/LinkMissing';
import { StateBanner } from '@/components/StateBanner';
import { messageFor, problemCode } from '@/booking/problem';
import { withLocative } from '@/lib/suffix';
import { createReview, fetchReviewInvite } from './review-api';
import type { ReviewInvite } from './review-api';
import styles from './review.module.css';

const ratingLabels = ['Kötü', 'İdare eder', 'İyi', 'Çok iyi', 'Mükemmel'] as const;
const commentMax = 1000;

type State =
  | { kind: 'loading' }
  | { kind: 'missing' }
  | { kind: 'failed' }
  | { kind: 'ready'; invite: ReviewInvite; justSent: boolean };

/** "Kuaför Elif'te nasıldı?"; ek kurulamayan adda "Randevunuz nasıldı?". */
function question(brandName: string): string {
  const located = withLocative(brandName);
  return located ? `${located} nasıldı?` : 'Randevunuz nasıldı?';
}

/** "Boya ve fön · Elif Kaya · 12 Kasım". */
function subtitle(invite: ReviewInvite): string {
  const date = toBranchDate(new Date(invite.appointmentStartUtc), invite.timeZone);
  return [invite.serviceNames.join(', '), invite.staff?.name, formatDate(date, 'dayMonth')]
    .filter(Boolean)
    .join(' · ');
}

function Stars({ value, label }: { value: number; label: string }) {
  return (
    <span className={styles.staticStars} role="img" aria-label={label}>
      {[1, 2, 3, 4, 5].map((n) => (
        <Star
          key={n}
          size={24}
          aria-hidden="true"
          className={n <= value ? styles.on : styles.off}
        />
      ))}
    </span>
  );
}

/**
 * Değerlendirme bağlantısı (omvila.com/d/{kod}; KK8). Web tasarımı yok: müşteri uygulamasının Degerlendir ekranı
 * rezervasyon sayfasının diline uyarlandı (T-100). Yıldız, personel kartı, isteğe bağlı yorum, "Gönder"; gönderilince
 * teşekkür, daha önce değerlendirildiyse gönderilmiş değerlendirme. Değerlendirme bir kez yapılır (already_reviewed).
 */
export function ReviewForm({ code }: { code: string }) {
  const [state, setState] = useState<State>({ kind: 'loading' });
  const [attempt, setAttempt] = useState(0);
  const [rating, setRating] = useState(0);
  const [comment, setComment] = useState('');
  const [busy, setBusy] = useState(false);
  const [failure, setFailure] = useState<unknown>(null);
  const [missingRating, setMissingRating] = useState(false);
  // Ağ hatasından sonra tekrar denemede aynı anahtar gider (ikinci kayıt oluşmaz).
  const [key] = useState(() => crypto.randomUUID());
  const id = useId();
  const doneRef = useRef<HTMLElement>(null);

  useEffect(() => {
    let live = true;
    fetchReviewInvite(code).then(
      (invite) => {
        if (live)
          setState(invite ? { kind: 'ready', invite, justSent: false } : { kind: 'missing' });
      },
      () => {
        if (live) setState({ kind: 'failed' });
      },
    );
    return () => {
      live = false;
    };
  }, [code, attempt]);

  const reviewed = state.kind === 'ready' && state.invite.alreadyReviewed;
  useEffect(() => {
    if (state.kind === 'missing') document.title = 'Bu bağlantı geçersiz';
    if (reviewed) doneRef.current?.focus();
  }, [state.kind, reviewed]);

  if (state.kind === 'missing') return <LinkMissing title="Bu bağlantı geçersiz" />;
  if (state.kind === 'failed') {
    return (
      <main className={styles.page}>
        <h1 className={styles.srOnly}>Değerlendirme</h1>
        <StateBanner
          tone="hata"
          icon={<WifiOff size={20} />}
          title="Bilgiler yüklenemedi."
          description="Bağlantınızı kontrol edip tekrar deneyin."
        >
          <BookingButton
            variant="secondary"
            size="sm"
            onClick={() => {
              setState({ kind: 'loading' });
              setAttempt((value) => value + 1);
            }}
          >
            Tekrar dene
          </BookingButton>
        </StateBanner>
      </main>
    );
  }
  if (state.kind === 'loading') {
    return (
      <main className={styles.page} aria-busy="true">
        <h1 className={styles.srOnly}>Değerlendirme yükleniyor</h1>
        <span className={styles.skeleton} />
        <span className={styles.skeleton} />
      </main>
    );
  }

  const { invite } = state;
  const header = (
    <BookingAvatar
      name={invite.brand.name}
      src={invite.brand.logoUrl ?? null}
      size="lg"
      decorative
    />
  );

  if (invite.alreadyReviewed) {
    const review = invite.review ?? null;
    return (
      <main ref={doneRef} className={styles.page} tabIndex={-1}>
        {header}
        <span className={styles.doneIcon} aria-hidden="true">
          <Check size={28} strokeWidth={2.5} />
        </span>
        <h1 className={styles.title}>
          {state.justSent ? 'Teşekkür ederiz' : 'Bu randevuyu zaten değerlendirdiniz'}
        </h1>
        <p className={styles.subtitle}>
          {state.justSent
            ? `Değerlendirmeniz ${invite.brand.name} ile paylaşıldı.`
            : 'Her randevu bir kez değerlendirilir. Gönderdiğiniz değerlendirme aşağıda.'}
        </p>
        {review ? (
          <section className={styles.card} aria-label="Değerlendirmeniz">
            <Stars value={review.rating} label={`5 üzerinden ${String(review.rating)} yıldız`} />
            <p className={styles.ratingLabel}>{ratingLabels[review.rating - 1]}</p>
            {review.comment ? <p className={styles.comment}>{review.comment}</p> : null}
          </section>
        ) : null}
        <p className={styles.subtitle}>{subtitle(invite)}</p>
        <a className={styles.link} href={`/${encodeURIComponent(invite.brand.slug)}`}>
          Yeni randevu al
        </a>
      </main>
    );
  }

  const submit = async () => {
    if (rating === 0) {
      setMissingRating(true);
      return;
    }
    setBusy(true);
    setFailure(null);
    try {
      const result = await createReview(code, key, {
        rating,
        comment: comment.trim() || null,
      });
      setState({ kind: 'ready', invite: result, justSent: true });
    } catch (error) {
      if (problemCode(error) === 'already_reviewed') {
        setState({ kind: 'loading' });
        setAttempt((value) => value + 1);
      } else {
        setFailure(error);
      }
    } finally {
      setBusy(false);
    }
  };

  return (
    <main className={styles.page}>
      {header}
      <div>
        <h1 className={styles.title}>{question(invite.brand.name)}</h1>
        <p className={styles.subtitle}>{subtitle(invite)}</p>
      </div>
      <form
        className={styles.form}
        noValidate
        onSubmit={(event) => {
          event.preventDefault();
          void submit();
        }}
      >
        <fieldset
          className={styles.card}
          aria-describedby={missingRating ? `${id}-puan-hata` : undefined}
        >
          <legend className={styles.srOnly}>Puanınız</legend>
          <div className={styles.stars}>
            {ratingLabels.map((label, index) => {
              const value = index + 1;
              return (
                <label key={label} className={styles.star}>
                  <input
                    type="radio"
                    name={`${id}-puan`}
                    value={value}
                    checked={rating === value}
                    aria-label={`${String(value)} yıldız, ${label}`}
                    onChange={() => {
                      setRating(value);
                      setMissingRating(false);
                    }}
                  />
                  <Star
                    size={36}
                    aria-hidden="true"
                    className={value <= rating ? styles.on : styles.off}
                  />
                </label>
              );
            })}
          </div>
          <p className={styles.ratingLabel} aria-live="polite">
            {rating > 0 ? ratingLabels[rating - 1] : 'Puan verin'}
          </p>
          {missingRating ? (
            <p id={`${id}-puan-hata`} className={styles.error}>
              Göndermek için bir puan seçin.
            </p>
          ) : null}
        </fieldset>
        {invite.staff ? (
          <div className={styles.staff}>
            <BookingAvatar
              name={invite.staff.name}
              src={invite.staff.photoUrl ?? null}
              size="sm"
              decorative
            />
            <span className={styles.staffText}>
              <strong>{invite.staff.name}</strong>
              <span>Sizi bu randevuda ağırlayan kişi</span>
            </span>
          </div>
        ) : null}
        <div className={styles.field}>
          <label className={styles.label} htmlFor={`${id}-yorum`}>
            Yorum (isteğe bağlı)
          </label>
          <textarea
            id={`${id}-yorum`}
            className={styles.textarea}
            rows={3}
            maxLength={commentMax}
            placeholder="Deneyiminizi birkaç kelimeyle anlatın"
            value={comment}
            aria-describedby={`${id}-yorum-not`}
            onChange={(event) => {
              setComment(event.target.value);
            }}
          />
          <p id={`${id}-yorum-not`} className={styles.hint}>
            Yorumunuz salonla paylaşılır. Adınız &quot;{invite.customerDisplayName}&quot; olarak
            görünür.
          </p>
        </div>
        {failure ? (
          <p className={styles.error} role="alert">
            {messageFor(failure)}
          </p>
        ) : null}
        <BookingButton
          type="submit"
          block
          loading={busy}
          loadingLabel="Gönderiliyor…"
          className={styles.submit}
        >
          Gönder
        </BookingButton>
      </form>
    </main>
  );
}
