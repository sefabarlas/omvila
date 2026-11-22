import { formatDate, formatMoney } from '@omvila/ui-web/format';
import { Badge, Banner, Button, EmptyState, Skeleton, useToast } from '@omvila/ui-web/panel';
import { useQuery } from '@tanstack/react-query';
import { Link, useNavigate } from '@tanstack/react-router';
import { Check, ChevronLeft, FileText, Landmark } from 'lucide-react';
import { useState } from 'react';
import { useApi } from '@/app/api-context';
import { useNow } from '@/app/clock';
import { Page } from '@/shared/Page';
import { QueryState } from '@/shared/QueryState';
import cardStyles from '@/shared/Card.module.css';
import brandStyles from '@/features/brands/Brands.module.css';
import { downloadFile } from '@/shared/download';
import { downloadReceipt, paymentNoticeQuery, useMatchNotice } from './api';
import type { Candidate, NoticeDetail } from './api';
import styles from './Approvals.module.css';
import { decisionError } from './decision-error';
import {
  candidateWhen,
  checkReason,
  dateTimeShort,
  matchConsequence,
  noticePurposeText,
  periodMonth,
  shortDate,
  waitingText,
  windowText,
} from './labels';
import { ResolveNoticeModal } from './ResolveNoticeModal';

/**
 * platform-paneli/POnayDetay: markanın bildirimi, dekont, aday banka hareketleri ve karar. Tutar veya kod uyuşmayan
 * aday seçilince "Kontrol edin" şeridi çıkar (KK3); operatör yine de eşleştirebilir. Açılış denetime yazılır.
 */
export function PaymentNoticePage({ noticeId }: { noticeId: string }) {
  const api = useApi();
  const now = useNow();
  const query = useQuery(paymentNoticeQuery(api, noticeId));
  const notice = query.data;
  const context = notice
    ? [
        notice.purpose === 'Credits'
          ? 'SMS kontörü'
          : `Abonelik${notice.periodStart ? ` · ${formatDate(notice.periodStart, 'monthYear')}` : ''}`,
        notice.status === 'Pending' ? waitingText(notice.notifiedUtc, now) : null,
      ]
        .filter(Boolean)
        .join(' · ')
    : undefined;

  return (
    <Page
      title={notice ? `${notice.brandName} · havale bildirimi` : 'Havale bildirimi'}
      {...(context ? { context } : {})}
    >
      <Link to="/onay" className={brandStyles.back}>
        <ChevronLeft size={16} aria-hidden="true" />
        Onay kuyrukları
      </Link>
      <QueryState
        query={query}
        loading={<Skeleton variant="block" height="calc(var(--bosluk-12) * 5)" />}
      >
        {(detail) => <NoticeContent notice={detail} onRefresh={() => void query.refetch()} />}
      </QueryState>
    </Page>
  );
}

function NoticeContent({ notice, onRefresh }: { notice: NoticeDetail; onRefresh: () => void }) {
  return (
    <div className={styles.detail}>
      <div className={styles.column}>
        <section className={cardStyles.card} aria-labelledby="bildirim-baslik">
          <div className={cardStyles.cardHead}>
            <h2 id="bildirim-baslik" className={cardStyles.cardTitle}>
              Markanın bildirimi
            </h2>
          </div>
          <dl className={brandStyles.facts}>
            <Fact label="Ne için" value={noticePurposeText(notice)} />
            <Fact
              label="Beklenen tutar"
              value={notice.expectedAmount ? formatMoney(notice.expectedAmount) : 'Belirlenmedi'}
            />
            <Fact label="Açıklama kodu" value={notice.transferCode} />
            <Fact label="Gönderen" value={notice.senderName ?? '—'} />
            <Fact
              label="Bildirim"
              value={`${dateTimeShort(notice.notifiedUtc)} · ${notice.channel === 'App' ? 'uygulamadan' : 'panelden'}`}
            />
          </dl>
        </section>
        <Receipt notice={notice} />
      </div>
      <div className={styles.column}>
        {notice.status === 'Pending' ? (
          <Decision notice={notice} onRefresh={onRefresh} />
        ) : (
          <Resolution notice={notice} />
        )}
      </div>
    </div>
  );
}

function Fact({ label, value }: { label: string; value: string }) {
  return (
    <div className={brandStyles.fact}>
      <dt>{label}</dt>
      <dd>{value}</dd>
    </div>
  );
}

function Receipt({ notice }: { notice: NoticeDetail }) {
  const api = useApi();
  const toast = useToast();
  const [loading, setLoading] = useState(false);
  const receipt = notice.receipt;
  const kind = receipt?.contentType === 'application/pdf' ? 'PDF' : 'Görsel';

  const open = async () => {
    setLoading(true);
    try {
      const { blob, fileName } = await downloadReceipt(api, notice.id);
      downloadFile(blob, fileName ?? receipt?.fileName ?? 'dekont');
    } catch {
      toast.show({ tone: 'hata', title: 'Dekont açılamadı', message: 'Tekrar deneyin.' });
    } finally {
      setLoading(false);
    }
  };

  return (
    <section className={cardStyles.card} aria-labelledby="dekont-baslik">
      <div className={cardStyles.cardHead}>
        <h2 id="dekont-baslik" className={cardStyles.cardTitle}>
          Dekont
        </h2>
        <span className={cardStyles.muted}>isteğe bağlı</span>
      </div>
      {receipt ? (
        <div className={styles.file}>
          <span className={styles.fileIcon} aria-hidden="true">
            <FileText size={18} />
          </span>
          <div className={styles.candidateText}>
            <strong>{receipt.fileName}</strong>
            <span>{`${kind} · ${shortDate(receipt.uploadedUtc)}`}</span>
          </div>
          <Button
            size="sm"
            loading={loading}
            loadingLabel="Açılıyor…"
            aria-label="Dekontu aç"
            onClick={() => void open()}
          >
            Aç
          </Button>
        </div>
      ) : (
        <p className={cardStyles.muted}>Marka dekont yüklemedi.</p>
      )}
    </section>
  );
}

function Decision({ notice, onRefresh }: { notice: NoticeDetail; onRefresh: () => void }) {
  const toast = useToast();
  const navigate = useNavigate();
  const match = useMatchNotice();
  const firstMatch = notice.candidates.find((c) => c.verdict === 'CodeMatches');
  const [selected, setSelected] = useState<string | null>(firstMatch?.bankTransactionId ?? null);
  const [dialog, setDialog] = useState<'not-found' | 'reject' | null>(null);
  const [failure, setFailure] = useState<ReturnType<typeof decisionError> | null>(null);
  const chosen = notice.candidates.find((c) => c.bankTransactionId === selected);

  // mutateAsync: başarıda bildirim sonuçlanır ve bu kart kalkar; çağrı başına geri çağrı kaybolmasın.
  const submit = async () => {
    if (!chosen) {
      return;
    }

    setFailure(null);
    try {
      await match.mutateAsync({
        noticeId: notice.id,
        version: notice.version,
        bankTransactionId: chosen.bankTransactionId,
      });
      toast.show({
        tone: 'basari',
        title: 'Havale eşleştirildi',
        message: `${notice.brandName} markasına bildirim gitti.`,
      });
      void navigate({ to: '/onay' });
    } catch (problem) {
      setFailure(decisionError(problem));
    }
  };

  return (
    <>
      <section className={cardStyles.card} aria-labelledby="havaleler-baslik">
        <div className={cardStyles.cardHead}>
          <h2 id="havaleler-baslik" className={cardStyles.cardTitle}>
            OMVILA hesabına gelen havaleler
          </h2>
          <span className={cardStyles.muted}>{windowText(notice.candidateWindow)}</span>
        </div>
        {notice.candidates.length === 0 ? (
          <EmptyState
            icon={<Landmark size={22} />}
            title="Bu aralıkta eşleşmemiş havale yok"
            description="Banka dökümünü içe aktarın veya hareketi elle ekleyin; gelmediyse markaya yazın."
            actions={
              <Button
                onClick={() =>
                  void navigate({
                    to: '/onay/banka-hareketleri',
                    search: { q: notice.transferCode },
                  })
                }
              >
                Banka hareketlerinde ara
              </Button>
            }
          />
        ) : (
          <fieldset className={styles.candidates}>
            <legend className={styles.visuallyHidden}>Eşleştirilecek banka hareketi</legend>
            {notice.candidates.map((candidate) => (
              <CandidateOption
                key={candidate.bankTransactionId}
                candidate={candidate}
                checked={candidate.bankTransactionId === selected}
                onSelect={() => {
                  setSelected(candidate.bankTransactionId);
                }}
              />
            ))}
          </fieldset>
        )}
      </section>
      <section className={cardStyles.card} aria-labelledby="karar-baslik">
        <div className={cardStyles.cardHead}>
          <h2 id="karar-baslik" className={cardStyles.cardTitle}>
            Karar
          </h2>
        </div>
        {chosen?.verdict === 'CheckRequired' ? (
          <Banner tone="uyari" title="Kontrol edin:">
            {checkReason(chosen)}
          </Banner>
        ) : null}
        {failure ? (
          <Banner
            tone="hata"
            title={failure.title}
            {...(failure.stale
              ? {
                  action: (
                    <Button
                      size="sm"
                      onClick={() => {
                        setFailure(null);
                        onRefresh();
                      }}
                    >
                      Yenile
                    </Button>
                  ),
                }
              : {})}
          >
            {failure.text}
          </Banner>
        ) : null}
        <div className={styles.decision}>
          <Button
            variant="ghost"
            onClick={() => {
              setDialog('reject');
            }}
          >
            Reddet
          </Button>
          <Button
            onClick={() => {
              setDialog('not-found');
            }}
          >
            Bulunamadı, markaya yaz
          </Button>
          <Button
            variant="primary"
            icon={<Check size={16} aria-hidden="true" />}
            disabled={!chosen}
            loading={match.isPending}
            loadingLabel="Eşleştiriliyor…"
            onClick={() => void submit()}
          >
            Seçili hareketle eşleştir
          </Button>
        </div>
        <p className={cardStyles.muted}>{matchConsequence(notice)}</p>
      </section>
      {dialog ? (
        <ResolveNoticeModal
          kind={dialog}
          notice={notice}
          onClose={() => {
            setDialog(null);
          }}
        />
      ) : null}
    </>
  );
}

function CandidateOption({
  candidate,
  checked,
  onSelect,
}: {
  candidate: Candidate;
  checked: boolean;
  onSelect: () => void;
}) {
  return (
    <label className={styles.candidate}>
      <input type="radio" name="banka-hareketi" checked={checked} onChange={onSelect} />
      <span className={styles.candidateText}>
        <strong>{candidate.senderName}</strong>
        <span>{`${candidateWhen(candidate)} · açıklama: ${candidate.description ?? '—'}`}</span>
      </span>
      <span className={styles.amount}>{formatMoney(candidate.amount)}</span>
      {candidate.verdict === 'CodeMatches' ? (
        <Badge tone="basari">Kod tutuyor</Badge>
      ) : (
        <Badge tone="uyari">Kontrol edin</Badge>
      )}
    </label>
  );
}

const resolutionTitles: Record<Exclude<NoticeDetail['status'], 'Pending'>, string> = {
  Matched: 'Eşleştirildi',
  NotFound: 'Bulunamadı olarak markaya yazıldı',
  Rejected: 'Reddedildi',
};

function Resolution({ notice }: { notice: NoticeDetail }) {
  const status = notice.status === 'Pending' ? 'Matched' : notice.status;
  const resolution = notice.resolution;
  const by = resolution
    ? `${resolution.resolvedByName} · ${dateTimeShort(resolution.resolvedUtc)}`
    : undefined;
  return (
    <section className={cardStyles.card} aria-labelledby="karar-baslik">
      <div className={cardStyles.cardHead}>
        <h2 id="karar-baslik" className={cardStyles.cardTitle}>
          Karar
        </h2>
      </div>
      <Banner tone={status === 'Matched' ? 'basari' : 'bilgi'} title={resolutionTitles[status]}>
        {[
          by,
          status === 'Matched' && notice.periodStart
            ? `${periodMonth(notice.periodStart)} dönemi ödendi sayıldı.`
            : null,
          resolution?.message ? `Not: ${resolution.message}` : null,
        ]
          .filter(Boolean)
          .join(' · ')}
      </Banner>
    </section>
  );
}
