import { Button, EmptyState } from '@omvila/ui-web/panel';
import { Clock, Plus, Scissors, Sparkles, Tag, Wallet } from 'lucide-react';
import styles from './Services.module.css';

export interface ServicesEmptyProps {
  industryLabel: string;
  templateCount: number;
  onTemplates: () => void;
  onBlank: () => void;
}

/** salon-paneli/HizmetlerBos: hazır liste önerisi ve boş hizmet. */
export function ServicesEmpty({
  industryLabel,
  templateCount,
  onTemplates,
  onBlank,
}: ServicesEmptyProps) {
  return (
    <section className={styles.emptyCard} aria-label="Hizmet yok">
      <EmptyState
        icon={<Tag size={24} />}
        headingLevel={2}
        title="İlk hizmetinizi ekleyin"
        description="Online randevu için en az bir hizmet gerekir. Her hizmetin süresi, fiyatı ve kimlerin yaptığı olur."
        actions={
          <>
            {templateCount > 0 ? (
              <Button
                variant="primary"
                icon={<Sparkles size={16} aria-hidden="true" />}
                onClick={onTemplates}
              >
                Hazır listeden başla
              </Button>
            ) : null}
            <Button icon={<Plus size={16} aria-hidden="true" />} onClick={onBlank}>
              Boş hizmet ekle
            </Button>
          </>
        }
      />
      <div className={styles.tips}>
        {templateCount > 0 ? (
          <div className={styles.tip}>
            <Scissors size={18} className={styles.tipIcon} aria-hidden="true" />
            <div>
              <p className={styles.tipTitle}>{industryLabel} hazır listesi</p>
              <p className={styles.tipText}>{templateCount} hizmet; fiyatları siz girersiniz.</p>
            </div>
          </div>
        ) : null}
        <div className={styles.tip}>
          <Clock size={18} className={styles.tipIcon} aria-hidden="true" />
          <div>
            <p className={styles.tipTitle}>Süreler önemli</p>
            <p className={styles.tipText}>
              Takvimdeki boşluklar hizmet sürelerine göre hesaplanır.
            </p>
          </div>
        </div>
        <div className={styles.tip}>
          <Wallet size={18} className={styles.tipIcon} aria-hidden="true" />
          <div>
            <p className={styles.tipTitle}>Kapora isteğe bağlı</p>
            <p className={styles.tipText}>
              IBAN&apos;ınızı ekleyince hizmet bazında açılır; müşteri havale ile gönderir.
            </p>
          </div>
        </div>
      </div>
    </section>
  );
}
