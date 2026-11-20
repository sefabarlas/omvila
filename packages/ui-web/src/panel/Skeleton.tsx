import { useEffect, useState } from 'react';
import type { CSSProperties } from 'react';
import { classNames } from './Button';
import styles from './Skeleton.module.css';

export interface SkeletonProps {
  variant?: 'text' | 'block' | 'circle';
  /** Genişlik ve yükseklik token değerleri (ör. var(--kontrol-md)). */
  width?: string;
  height?: string;
  /** Kısa yüklemede iskelet gösterilmez (urun-kurallari.md: 150 ms). */
  delayMs?: number;
  className?: string;
}

/** Yükleme yer tutucusu; yalnız gecikmeden sonra görünür, ekran okuyucuya gizlidir. */
export function Skeleton({
  variant = 'text',
  width,
  height,
  delayMs = 150,
  className,
}: SkeletonProps) {
  const [visible, setVisible] = useState(delayMs === 0);
  useEffect(() => {
    if (delayMs === 0) {
      return;
    }

    const timer = setTimeout(() => {
      setVisible(true);
    }, delayMs);
    return () => {
      clearTimeout(timer);
    };
  }, [delayMs]);

  if (!visible) {
    return null;
  }

  const style: CSSProperties = { width, height };
  return (
    <span
      aria-hidden="true"
      className={classNames(styles.skeleton, styles[variant], className)}
      style={style}
    />
  );
}
