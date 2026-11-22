import { cx } from '@/lib/cx';
import styles from './Screenshot.module.css';

// Ürün ekran görüntüleri public/ekranlar'da (kaynak: salon paneli ve rezervasyon sayfasının Playwright ekran
// görüntüleri, demo verisi; T-117). Site sistem temasını izlediği için koyu sürüm prefers-color-scheme ile seçilir.
// Panel görüntüsü telefonda LCP öğesidir: 680 px'lik hafif kopyası srcset ile seçilir (yavaş 4G bütçesi).
const shots = {
  panel: {
    file: 'salon-takvim',
    width: 960,
    height: 765,
    small: 680,
    sizes: '(width < 900px) calc(100vw - 40px), 640px',
  },
  phone: { file: 'rezervasyon-saat', width: 420, height: 909, small: null, sizes: undefined },
} as const;

function srcSet(base: string, width: number, small: number | null): string | undefined {
  if (small === null) return undefined;
  return `/ekranlar/${base}-${String(small)}.webp ${String(small)}w, /ekranlar/${base}.webp ${String(width)}w`;
}

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
  const dark = `${shot.file}-koyu`;
  return (
    <picture className={cx(styles.frame, styles[kind], className)}>
      <source
        srcSet={srcSet(dark, shot.width, shot.small) ?? `/ekranlar/${dark}.webp`}
        sizes={shot.sizes}
        media="(prefers-color-scheme: dark)"
      />
      <img
        src={`/ekranlar/${shot.file}.webp`}
        srcSet={srcSet(shot.file, shot.width, shot.small)}
        sizes={shot.sizes}
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
