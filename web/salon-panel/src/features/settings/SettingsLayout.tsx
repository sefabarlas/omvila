import { Button } from '@omvila/ui-web/panel';
import { Link } from '@tanstack/react-router';
import { Check } from 'lucide-react';
import type { ReactNode } from 'react';
import { Page } from '@/shared/Page';
import styles from './Settings.module.css';

// Bölüm menüsü. Bildirimler (D3), kapora ve IBAN (D4), roller, abonelik, KVKK (D5–6)
// ilgili dilim gelince eklenir; yapılmamış bölüm menüde görünmez (acik-sorular.md T-38, T-41).
const sections = [
  { to: '/ayarlar/isletme', label: 'İşletme bilgileri' },
  { to: '/ayarlar/sube', label: 'Şubeler ve saatler' },
  { to: '/ayarlar/randevu-kurallari', label: 'Randevu kuralları' },
  { to: '/ayarlar/rezervasyon', label: 'Rezervasyon sayfası' },
] as const;

export interface SettingsLayoutProps {
  context: string;
  /** Üst bardaki "Değişiklikleri kaydet" bu formu gönderir. */
  formId: string;
  saving: boolean;
  /** Kaydetmenin solunda ikincil eylem ("Şube ekle"). */
  extraAction?: ReactNode;
  children: ReactNode;
}

/** Ayarlar sayfası: üst barda tek birincil eylem, solda bölüm menüsü, sağda kartlar. */
export function SettingsLayout({
  context,
  formId,
  saving,
  extraAction,
  children,
}: SettingsLayoutProps) {
  return (
    <Page
      title="Ayarlar"
      context={context}
      action={
        <>
          {extraAction}
          <Button
            variant="primary"
            type="submit"
            form={formId}
            icon={<Check size={16} aria-hidden="true" />}
            loading={saving}
            loadingLabel="Kaydediliyor…"
          >
            Değişiklikleri kaydet
          </Button>
        </>
      }
    >
      <div className={styles.layout}>
        <nav className={styles.nav} aria-label="Ayar bölümleri">
          <ul className={styles.navList}>
            {sections.map((section) => (
              <li key={section.to}>
                <Link to={section.to} className={styles.navLink}>
                  {section.label}
                </Link>
              </li>
            ))}
          </ul>
        </nav>
        <div className={styles.sections}>{children}</div>
      </div>
    </Page>
  );
}

export interface SettingsCardProps {
  title: string;
  description?: string;
  children: ReactNode;
}

/** Başlıklı ayar kartı (Marka, İletişim, Online randevu…). */
export function SettingsCard({ title, description, children }: SettingsCardProps) {
  return (
    <section className={styles.card} aria-label={title}>
      <div className={styles.cardHead}>
        <h2 className={styles.cardTitle}>{title}</h2>
        {description ? <p className={styles.cardText}>{description}</p> : null}
      </div>
      {children}
    </section>
  );
}

export interface SettingRowProps {
  id: string;
  title: string;
  description: string;
  /** Açık/kapalı anahtar dar, açılır liste ve metin alanı geniş. */
  narrow?: boolean;
  children: ReactNode;
}

/** Solda ad ve açıklama, sağda denetim. Denetim adını `${id}-baslik` ile, açıklamasını `${id}-aciklama` ile alır. */
export function SettingRow({ id, title, description, narrow = false, children }: SettingRowProps) {
  return (
    <li className={styles.row}>
      <div className={styles.rowText}>
        <span id={`${id}-baslik`} className={styles.rowTitle}>
          {title}
        </span>
        <span id={`${id}-aciklama`} className={styles.rowDetail}>
          {description}
        </span>
      </div>
      <div className={narrow ? styles.rowControlNarrow : styles.rowControl}>{children}</div>
    </li>
  );
}

export function SettingRows({ children }: { children: ReactNode }) {
  return <ul className={styles.rows}>{children}</ul>;
}
