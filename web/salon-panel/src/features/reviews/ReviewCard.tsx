import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { Avatar, Button, TextField, useToast } from '@omvila/ui-web/panel';
import { MessageSquare, Star } from 'lucide-react';
import { useState } from 'react';
import { defaultTimeZone } from '@/app/config';
import { useReplyReview } from './api';
import type { ReviewView } from './api';
import { reviewMeta } from './labels';
import styles from './Reviews.module.css';

/** Yanıt uzunluğu (sözleşme: ReviewReplyRequest.text 2–1000). */
const replyMin = 2;
const replyMax = 1000;

/** Beş yıldız; dolu yıldız sayısı puandır. */
export function Stars({ rating }: { rating: number }) {
  return (
    <span className={styles.stars} role="img" aria-label={`5 üzerinden ${String(rating)} yıldız`}>
      {[1, 2, 3, 4, 5].map((n) => (
        <Star
          key={n}
          size={14}
          aria-hidden="true"
          className={n <= rating ? styles.starFull : styles.starEmpty}
        />
      ))}
    </span>
  );
}

export interface ReviewCardProps {
  review: ReviewView;
  today: string;
  /** review.reply izni: yanıt alanı ve "Yanıtla" yalnız bununla. */
  canReply: boolean;
  /** Yanıt alanı açık mı (sayfa ilk yanıt bekleyen kartta açık başlatır). */
  replying: boolean;
  onReplyingChange: (open: boolean) => void;
}

/**
 * Değerlendirme kartı (salon-paneli/Degerlendirmeler): kişi, hizmet, personel, yaş, yıldızlar; yorum yoksa
 * "Yorum yazılmadı, yalnızca puan."; verilmiş yanıt "Yanıtınız · {ad}" bloğunda; yanıtsız kartta yanıt alanı.
 */
export function ReviewCard({
  review,
  today,
  canReply,
  replying,
  onReplyingChange,
}: ReviewCardProps) {
  const toast = useToast();
  const reply = useReplyReview();
  const [text, setText] = useState('');
  const trimmed = text.trim();
  const fieldId = `yanit-${review.id}`;

  const submit = () => {
    reply.mutate(
      { reviewId: review.id, text: trimmed },
      {
        onSuccess: () => {
          setText('');
          onReplyingChange(false);
          toast.show({ tone: 'basari', title: 'Yanıtınız yayımlandı' });
        },
      },
    );
  };

  return (
    <article className={styles.card} aria-label={`${review.customer.fullName} değerlendirmesi`}>
      <header className={styles.cardHead}>
        <Avatar name={review.customer.fullName} size="sm" />
        <p className={styles.who}>
          <strong>{review.customer.fullName}</strong>
          <span className={styles.meta}>{` · ${reviewMeta(review, today, defaultTimeZone)}`}</span>
        </p>
        <Stars rating={review.rating} />
      </header>
      {review.comment ? (
        <p className={styles.comment}>{review.comment}</p>
      ) : (
        <p className={styles.noComment}>Yorum yazılmadı, yalnızca puan.</p>
      )}
      {review.reply ? (
        <div className={styles.reply}>
          <p className={styles.replyBy}>
            {review.reply.repliedByName ? `Yanıtınız · ${review.reply.repliedByName}` : 'Yanıtınız'}
          </p>
          <p className={styles.replyText}>{review.reply.text}</p>
        </div>
      ) : canReply && replying ? (
        <form
          className={styles.replyForm}
          onSubmit={(event) => {
            event.preventDefault();
            if (trimmed.length >= replyMin) {
              submit();
            }
          }}
        >
          <TextField
            id={fieldId}
            label={`${review.customer.fullName} için yanıt`}
            hideLabel
            placeholder="Yanıtınızı yazın, müşteri görür"
            value={text}
            maxLength={replyMax}
            error={
              reply.isError
                ? errorMessage(reply.error instanceof ApiProblem ? reply.error.code : undefined)
                : undefined
            }
            onChange={(event) => {
              setText(event.target.value);
            }}
          />
          <div className={styles.replyActions}>
            <Button
              size="sm"
              onClick={() => {
                setText('');
                reply.reset();
                onReplyingChange(false);
              }}
            >
              Vazgeç
            </Button>
            <Button
              size="sm"
              variant="primary"
              type="submit"
              disabled={trimmed.length < replyMin}
              loading={reply.isPending}
              loadingLabel="Gönderiliyor…"
            >
              Yanıtla
            </Button>
          </div>
        </form>
      ) : canReply ? (
        <div>
          <Button
            size="sm"
            variant="ghost"
            icon={<MessageSquare size={16} aria-hidden="true" />}
            onClick={() => {
              onReplyingChange(true);
            }}
          >
            Yanıtla
          </Button>
        </div>
      ) : null}
    </article>
  );
}
