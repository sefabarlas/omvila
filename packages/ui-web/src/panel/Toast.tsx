import { AlertTriangle, CheckCircle2, Info, X, XCircle } from 'lucide-react';
import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useRef,
  useState,
} from 'react';
import type { ReactNode } from 'react';
import { Button, classNames } from './Button';
import { IconButton } from './IconButton';
import styles from './Toast.module.css';

export type ToastTone = 'basari' | 'hata' | 'uyari' | 'bilgi';

export interface ToastOptions {
  tone?: ToastTone;
  /** Sonucu söyler: "Randevu oluşturuldu" ("başarıyla" kullanılmaz). */
  title: string;
  message?: string;
  /** Geri alınabilen işlemde "Geri al". */
  action?: { label: string; onClick: () => void };
}

interface ToastItem extends ToastOptions {
  id: number;
}

interface ToastApi {
  show: (toast: ToastOptions) => void;
}

const ToastContext = createContext<ToastApi | null>(null);
const icons = { basari: CheckCircle2, hata: XCircle, uyari: AlertTriangle, bilgi: Info } as const;
const maxVisible = 3;

export interface ToastProviderProps {
  children: ReactNode;
  /** Kendiliğinden kapanma süresi (ms); hata toast'u kendiliğinden kapanmaz. */
  durationMs?: number;
}

/** Uygulama kökünde bir kez; bildirimler canlı bölgede (aria-live) okunur. */
export function ToastProvider({ children, durationMs = 5000 }: ToastProviderProps) {
  const [items, setItems] = useState<ToastItem[]>([]);
  const nextId = useRef(1);
  const dismiss = useCallback((id: number) => {
    setItems((current) => current.filter((t) => t.id !== id));
  }, []);
  const api = useMemo<ToastApi>(
    () => ({
      show: (toast) => {
        const id = nextId.current++;
        setItems((current) => [...current, { ...toast, id }].slice(-maxVisible));
      },
    }),
    [],
  );

  return (
    <ToastContext.Provider value={api}>
      {children}
      <div className={styles.region} role="region" aria-label="Bildirimler" aria-live="polite">
        {items.map((item) => (
          <ToastView key={item.id} item={item} durationMs={durationMs} onDismiss={dismiss} />
        ))}
      </div>
    </ToastContext.Provider>
  );
}

function ToastView({
  item,
  durationMs,
  onDismiss,
}: {
  item: ToastItem;
  durationMs: number;
  onDismiss: (id: number) => void;
}) {
  const tone = item.tone ?? 'basari';
  const Icon = icons[tone];
  useEffect(() => {
    if (tone === 'hata') {
      return;
    }

    const timer = setTimeout(() => {
      onDismiss(item.id);
    }, durationMs);
    return () => {
      clearTimeout(timer);
    };
  }, [durationMs, item.id, onDismiss, tone]);

  return (
    <div
      className={classNames(styles.toast, styles[tone])}
      role={tone === 'hata' ? 'alert' : 'status'}
    >
      <span className={styles.icon} aria-hidden="true">
        <Icon size={18} />
      </span>
      <div className={styles.text}>
        <strong className={styles.title}>{item.title}</strong>
        {item.message ? <span className={styles.message}>{item.message}</span> : null}
      </div>
      {item.action ? (
        <Button
          size="sm"
          variant="ghost"
          onClick={() => {
            item.action?.onClick();
            onDismiss(item.id);
          }}
        >
          {item.action.label}
        </Button>
      ) : null}
      <IconButton
        label="Kapat"
        size="sm"
        icon={<X size={16} />}
        onClick={() => {
          onDismiss(item.id);
        }}
      />
    </div>
  );
}

/** ToastProvider içinde: useToast().show({ title: "Randevu oluşturuldu" }). */
export function useToast(): ToastApi {
  const api = useContext(ToastContext);
  if (!api) {
    throw new Error('useToast, ToastProvider içinde kullanılmalı.');
  }

  return api;
}
