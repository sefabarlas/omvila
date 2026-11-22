import {
  ArrowRight,
  Bell,
  CalendarDays,
  Check,
  Link2,
  ListChecks,
  Users,
  Wallet,
} from 'lucide-react';
import Link from 'next/link';
import type { ReactNode } from 'react';
import { Screenshot } from '@/components/Screenshot';
import ui from '@/components/ui.module.css';
import { pageMetadata } from '@/lib/metadata';
import { trialText } from '@/lib/pricing';
import { reminderText, signUpUrl, site } from '@/lib/site';
import styles from './page.module.css';
import { cx } from '@/lib/cx';

export const metadata = pageMetadata({
  title: 'OMVILA · Randevuyla çalışan işletmeler için online randevu',
  description:
    'Müşterileriniz bağlantınızdan uygulama indirmeden randevu alır, SMS ile hatırlatılır. Takviminizi, sıranızı ve kasanızı tek yerden yönetirsiniz.',
  path: '/',
});

interface Feature {
  icon: ReactNode;
  title: string;
  text: string;
  wide?: boolean;
}

// Main.png "Neler var". Metinler tasarımdan; sektöre daraltan ifadeler genelleştirildi (T-117).
const features: Feature[] = [
  {
    icon: <Link2 size={20} aria-hidden />,
    title: 'Kendi randevu bağlantınız',
    text: "Size özel bağlantıdan müşteri hizmeti, kişiyi ve boş saati seçer; telefonuyla onaylar. Bağlantıyı Instagram'a, SMS'e ya da masadaki QR karta koyarsınız.",
    wide: true,
  },
  {
    icon: <Bell size={20} aria-hidden />,
    title: 'SMS hatırlatma',
    text: `Onay, ${reminderText()} önce hatırlatma kendiliğinden gider.`,
  },
  {
    icon: <Wallet size={20} aria-hidden />,
    title: 'Kapora sizin hesabınıza',
    text: "İsterseniz hizmet bazında kapora istersiniz. Müşteri IBAN'ınıza havale eder, gelince tek dokunuşla işaretlersiniz. OMVILA ödemeye aracılık etmez.",
  },
  {
    icon: <ListChecks size={20} aria-hidden />,
    title: 'Bekleme listesi',
    text: `Dolu güne isteyenler listeye girer. Bir saat boşalınca sıradakine teklif gider, saat ${String(site.waitlistOfferHoldMinutes)} dakika onun için tutulur.`,
  },
  {
    icon: <Users size={20} aria-hidden />,
    title: 'Kapıda sıra',
    text: 'Randevusuz gelenleri sıraya alın; sırası yaklaşınca müşteriye SMS gider.',
  },
  {
    icon: <CalendarDays size={20} aria-hidden />,
    title: 'Ekip takvimi',
    text: "Her çalışan kendi takvimini ve primini kendi telefonundan görür. Google Takvim'e tek yönlü aktarılır.",
    wide: true,
  },
];

const steps = [
  { title: 'Telefonla kayıt olun', text: 'Şifre yok; cep telefonunuza gelen kodla girersiniz.' },
  {
    title: 'Hizmet ve ekibi ekleyin',
    text: 'Hazır hizmet listesinden başlayın, süre ve fiyatları girin.',
  },
  {
    title: 'Bağlantınızı paylaşın',
    text: 'Instagram, SMS ya da masadaki QR kart. Randevular gelmeye başlar.',
  },
];

const customerPoints = [
  'Saat değiştirme ve iptal bağlantıdan',
  'İptalde neden sorulur; siz sebebini görürsünüz',
  'KVKK izinleri müşteri bazında tutulur',
];

export default function HomePage() {
  const trial = trialText();
  return (
    <>
      <section className={cx(ui.container, styles.hero)}>
        <div className={styles.heroText}>
          <p className={ui.eyebrow}>Randevuyla çalışan işletmeler için online randevu</p>
          <h1 className={styles.heroTitle}>Randevularınız kendiliğinden dolsun</h1>
          <p className={ui.lead}>
            Müşterileriniz bağlantınızdan uygulama indirmeden randevu alır, SMS ile hatırlatılır.
            Siz de takviminizi, sıranızı ve kasanızı tek yerden yönetirsiniz.
          </p>
          <div className={styles.heroActions}>
            <a href={signUpUrl()} className={cx(ui.button, ui.large, ui.primary)}>
              Ücretsiz dene
              <ArrowRight size={18} aria-hidden />
            </a>
            <Link href="/#nasil-calisir" className={cx(ui.button, ui.large, ui.secondary)}>
              Nasıl çalışır?
            </Link>
          </div>
          <p className={styles.note}>{trial} · kart gerekmez · telefonla giriş</p>
        </div>
        <div className={styles.heroVisual}>
          <Screenshot kind="panel" alt="Salon panelinde günlük takvim" priority />
          <Screenshot
            kind="phone"
            alt="Müşterinin saat seçtiği ekran"
            className={styles.heroPhone}
          />
        </div>
      </section>

      <section id="ozellikler" className={cx(ui.container, ui.section)}>
        <p className={ui.eyebrow}>Neler var</p>
        <h2 className={ui.sectionTitle}>Telefon trafiği yerine dolu bir takvim</h2>
        <ul className={styles.features}>
          {features.map((feature) => (
            <li
              key={feature.title}
              className={cx(ui.card, styles.feature, feature.wide && styles.wide)}
            >
              <span className={styles.featureIcon}>{feature.icon}</span>
              <h3 className={styles.cardTitle}>{feature.title}</h3>
              <p className={styles.cardText}>{feature.text}</p>
            </li>
          ))}
        </ul>
      </section>

      <section className={cx(ui.container, ui.section, styles.customer)}>
        <div className={styles.customerVisual}>
          <Screenshot kind="phone" alt="Müşterinin saat seçtiği ekran" />
        </div>
        <div className={styles.customerText}>
          <p className={ui.eyebrow}>Müşteriniz için</p>
          <h2 className={ui.sectionTitle}>Uygulama zorunlu değil</h2>
          <p className={ui.lead}>
            Müşteri bağlantıyı açar, saati seçer, telefonuna gelen kodla onaylar. İsterse OMVILA
            uygulamasıyla tüm randevularını, paketlerini ve hatırlatmalarını tek yerde görür.
          </p>
          <ul className={cx(ui.checkList, styles.customerList)}>
            {customerPoints.map((point) => (
              <li key={point}>
                <Check size={18} aria-hidden />
                {point}
              </li>
            ))}
          </ul>
        </div>
      </section>

      <section id="nasil-calisir" className={cx(ui.container, ui.section)}>
        <p className={ui.eyebrow}>Nasıl çalışır</p>
        <h2 className={ui.sectionTitle}>Üç adımda ilk online randevu</h2>
        <ol className={styles.steps}>
          {steps.map((step, index) => (
            <li key={step.title} className={ui.card}>
              <span className={styles.stepNumber} aria-hidden>
                {index + 1}
              </span>
              <h3 className={styles.cardTitle}>{step.title}</h3>
              <p className={styles.cardText}>{step.text}</p>
            </li>
          ))}
        </ol>
      </section>

      <section className={cx(ui.container, styles.ctaWrap)}>
        <div className={styles.cta}>
          <div>
            <h2 className={styles.ctaTitle}>{trial} deneyin</h2>
            <p className={styles.ctaText}>
              Kart gerekmez. Deneme bitince planınızı seçersiniz; seçmezseniz verileriniz silinmez.
            </p>
          </div>
          <div className={styles.ctaActions}>
            <a href={signUpUrl()} className={cx(ui.button, ui.large, ui.inverse)}>
              Ücretsiz dene
            </a>
            <Link href="/fiyatlar/" className={cx(ui.button, ui.large, ui.inverseGhost)}>
              Fiyatları gör
            </Link>
          </div>
        </div>
      </section>
    </>
  );
}
