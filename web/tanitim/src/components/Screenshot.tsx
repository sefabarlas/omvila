import { cx } from '@/lib/cx';
import styles from './Screenshot.module.css';

// Ürün ekran görüntüleri public/ekranlar'da (kaynak: salon paneli ve rezervasyon sayfasının Playwright ekran
// görüntüleri, demo verisi; T-117). Site sistem temasını izlediği için koyu sürüm prefers-color-scheme ile seçilir.
const shots = {
  panel: { file: 'salon-takvim', width: 960, height: 765 },
  phone: { file: 'rezervasyon-saat', width: 540, height: 1169 },
} as const;

export function Screenshot({
  kind,
  alt,
  priority = false,
  className,
}: {
  kind: keyof typeof shots;
  alt: string;
  /** Hero görseli (LCP): hemen ve yüksek öncelikle yüklenir; diğerleri tembel. */
  priority?: boolean;
  className?: string | undefined;
}) {
  const shot = shots[kind];
  return (
    <picture className={cx(styles.frame, styles[kind], className)}>
      <source srcSet={`/ekranlar/${shot.file}-koyu.webp`} media="(prefers-color-scheme: dark)" />
      <img
        src={`/ekranlar/${shot.file}.webp`}
        alt={alt}
        width={shot.width}
        height={shot.height}
        loading={priority ? 'eager' : 'lazy'}
        fetchPriority={priority ? 'high' : 'auto'}
        decoding="async"
      />
    </picture>
  );
}
