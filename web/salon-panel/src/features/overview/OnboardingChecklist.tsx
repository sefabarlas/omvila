import { Link } from '@tanstack/react-router';
import { Check, ChevronRight } from 'lucide-react';
import type { OnboardingStepKey } from './api';
import styles from './Overview.module.css';

interface StepView {
  title: string;
  detail: string;
  /** Adımın yapıldığı ekran (Dilim 1 ekranları). */
  to: string;
}

interface OnboardingChecklistProps {
  steps: readonly { key: OnboardingStepKey; done: boolean }[];
  completedCount: number;
  totalCount: number;
  branchName: string;
  bookingUrl: string | undefined;
}

/** salon-paneli/KayitKurulum: "İlk randevunuza 4 adım" (işletmenin oluşturulması dahil 5 adım). */
export function OnboardingChecklist({
  steps,
  completedCount,
  totalCount,
  branchName,
  bookingUrl,
}: OnboardingChecklistProps) {
  const views: Record<OnboardingStepKey, StepView> = {
    BusinessCreated: {
      title: 'İşletmenizi oluşturun',
      detail: 'Ad ve kategori',
      to: '/ayarlar/isletme',
    },
    WorkingHours: {
      title: 'Çalışma saatlerinizi girin',
      detail: `${branchName} şubesi · 2 dk`,
      to: '/ayarlar/sube',
    },
    Services: {
      title: 'Hizmetlerinizi ekleyin',
      detail: 'Kategoriniz için hazır liste önerdik · 5 dk',
      to: '/hizmetler',
    },
    Staff: {
      title: 'Ekibinizi davet edin',
      detail: 'Her personel kendi telefonuyla girer',
      to: '/personel',
    },
    ShareBookingLink: {
      title: 'Rezervasyon bağlantınızı paylaşın',
      detail: displayUrl(bookingUrl),
      to: '/ayarlar/rezervasyon',
    },
  };

  return (
    <section aria-labelledby="kurulum-baslik">
      <div className={styles.listHead}>
        <h2 id="kurulum-baslik" className={styles.listTitle}>
          İlk randevunuza {totalCount - 1} adım
        </h2>
        <span className={styles.progress}>
          {completedCount} / {totalCount} tamamlandı
        </span>
      </div>
      <ol className={styles.steps}>
        {steps.map((step, index) => {
          const view = views[step.key];
          const content = (
            <>
              <span className={styles.badge} aria-hidden="true">
                {step.done ? <Check size={16} /> : index + 1}
              </span>
              <span className={styles.stepText}>
                <span className={styles.stepTitle}>{view.title}</span>
                <span className={styles.stepDetail}>{view.detail}</span>
              </span>
              {step.done ? (
                <span className={styles.visuallyHidden}>Tamamlandı</span>
              ) : (
                <span className={styles.chevron} aria-hidden="true">
                  <ChevronRight size={18} />
                </span>
              )}
            </>
          );
          return (
            <li key={step.key}>
              {step.done ? (
                <div className={`${styles.step ?? ''} ${styles.done ?? ''}`}>{content}</div>
              ) : (
                <Link to={view.to} className={styles.step}>
                  {content}
                </Link>
              )}
            </li>
          );
        })}
      </ol>
    </section>
  );
}

/** "https://omvila.com/kuafor-elif" → "omvila.com/kuafor-elif" (KayitKurulum). */
export function displayUrl(url: string | undefined): string {
  return url ? url.replace(/^https?:\/\//, '').replace(/\/$/, '') : '';
}
