'use client';

import { RotateCw } from 'lucide-react';
import { StatePage } from '@/components/StatePage';
import styles from './error.module.css';

/** API'ye ulaşılamadı veya 5xx: kullanıcıya teknik ayrıntı gösterilmez, tekrar deneme sunulur. */
export default function ErrorPage({ reset }: { error: Error; reset: () => void }) {
  return (
    <StatePage
      icon={<RotateCw size={24} />}
      title="Sayfa şu an açılamadı"
      description="Bağlantınızı kontrol edip tekrar deneyin."
      action={
        <button type="button" className={styles.retry} onClick={reset}>
          Tekrar dene
        </button>
      }
    />
  );
}
