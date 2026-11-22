import { Check } from 'lucide-react';
import ui from '@/components/ui.module.css';
import { pageMetadata } from '@/lib/metadata';
import {
  creditsLabel,
  featureText,
  monthlyNote,
  pricePendingText,
  priceText,
  pricing,
  trialEndAnswer,
  trialText,
} from '@/lib/pricing';
import { signUpUrl } from '@/lib/site';
import styles from './page.module.css';
import { cx } from '@/lib/cx';

export const metadata = pageMetadata({
  title: 'Fiyatlar · OMVILA',
  description:
    'Bütün planlarda online randevu, hatırlatma ve takvim var. SMS kontörüyle gönderdiğiniz kadar ödersiniz.',
  path: '/fiyatlar/',
});

// Fiyatlar.png. Plan, fiyat, sınır ve kontör paketleri content/pricing.json'dan; değer yoksa (S-14) fiyat yerine
// "Fiyat yakında açıklanacak". SSS'de yalnız tasarımda cevabı olan ilk soru gösterilir (T-117).
export default function PricingPage() {
  const trial = trialText();
  const note = monthlyNote();
  return (
    <>
      <section className={cx(ui.container, styles.intro)}>
        <p className={ui.eyebrow}>Fiyatlar</p>
        <h1 className={styles.title}>İşletmenize göre plan</h1>
        <p className={cx(ui.lead, styles.introText)}>
          Bütün planlarda online randevu, hatırlatma ve takvim var. {trial}, kart gerekmez.
        </p>
      </section>

      <section className={ui.container} aria-label="Planlar">
        <ul className={styles.plans}>
          {pricing.plans.map((plan) => {
            const price = priceText(plan.monthlyPriceMinor);
            return (
              <li
                key={plan.code}
                className={cx(ui.card, styles.plan, plan.recommended && styles.recommended)}
              >
                <div className={styles.planHead}>
                  <h2 className={styles.planName}>{plan.name}</h2>
                  {plan.recommended && <span className={styles.badge}>Önerilen</span>}
                </div>
                <p className={styles.audience}>{plan.audience}</p>
                <div className={styles.price}>
                  {price === null ? (
                    <p className={styles.pricePending}>{pricePendingText}</p>
                  ) : (
                    <>
                      <p className={styles.priceValue}>{price}</p>
                      <p className={styles.priceNote}>{note}</p>
                    </>
                  )}
                </div>
                <a
                  href={signUpUrl()}
                  className={cx(ui.button, ui.large, plan.recommended ? ui.inverse : ui.secondary)}
                >
                  {trial} dene
                </a>
                <ul className={cx(ui.checkList, styles.planFeatures)}>
                  {plan.features.map((feature) => {
                    const text = featureText(feature);
                    return (
                      <li key={text}>
                        <Check size={16} aria-hidden />
                        {text}
                      </li>
                    );
                  })}
                </ul>
              </li>
            );
          })}
        </ul>
      </section>

      <section className={cx(ui.container, styles.creditsWrap)}>
        <div className={cx(ui.card, styles.credits)}>
          <div className={styles.creditsText}>
            <p className={ui.eyebrow}>SMS kontörü</p>
            <h2 className={styles.creditsTitle}>Gönderdiğiniz kadar ödeyin</h2>
            <p className={styles.muted}>
              Uygulaması olan müşterilere bildirim ücretsizdir. SMS&apos;ler kontörle gider;{' '}
              {pricing.smsCharactersPerCredit} karaktere kadar bir mesaj bir kontördür.
            </p>
          </div>
          <dl className={styles.packages}>
            {pricing.creditPackages.map((pack) => (
              <div key={pack.code} className={styles.package}>
                <dt>{creditsLabel(pack.credits)}</dt>
                <dd>{priceText(pack.priceMinor) ?? pricePendingText}</dd>
              </div>
            ))}
          </dl>
        </div>
      </section>

      <section id="sss" className={cx(ui.container, styles.faq)}>
        <h2 className={styles.faqTitle}>Sık sorulanlar</h2>
        <div className={styles.faqItem}>
          <h3 className={styles.question}>Deneme bitince ne olur?</h3>
          <p className={styles.muted}>{trialEndAnswer()}</p>
        </div>
      </section>
    </>
  );
}
