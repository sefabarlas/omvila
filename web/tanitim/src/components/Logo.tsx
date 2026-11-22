import type { StaticImageData } from 'next/image';
import logoFile from '@omvila/design-tokens/assets/logos/omvila-logo-yatay.svg';
import logoInverseFile from '@omvila/design-tokens/assets/logos/omvila-logo-yatay-ters.svg';
import styles from './Logo.module.css';

// Next'in genel tipleri SVG içe aktarımını `any` bildirir; derlemede değer StaticImageData'dır (src, width, height).
const logo = (logoFile as StaticImageData).src;
const logoInverse = (logoInverseFile as StaticImageData).src;

// Yatay logo 96 px'in altına inmez (marka README § Logo); 32 px yükseklikte ~100 px (viewBox 622×200).
// Açık zeminde varsayılan, koyu tema ve marka zemininde ters sürüm.
const height = 32;
const width = Math.round((622 / 200) * height);

export function Logo({ inverse = false }: { inverse?: boolean }) {
  if (inverse) {
    return <img src={logoInverse} alt="OMVILA" width={width} height={height} />;
  }
  return (
    <>
      <img className={styles.light} src={logo} alt="OMVILA" width={width} height={height} />
      <img className={styles.dark} src={logoInverse} alt="OMVILA" width={width} height={height} />
    </>
  );
}
