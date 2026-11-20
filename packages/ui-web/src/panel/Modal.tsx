import { X } from 'lucide-react';
import { useId } from 'react';
import type { ReactNode } from 'react';
import { createPortal } from 'react-dom';
import { classNames } from './Button';
import styles from './Dialog.module.css';
import { IconButton } from './IconButton';
import { useDialog } from './useDialog';

export interface DialogProps {
  open: boolean;
  onClose: () => void;
  /** Görevi söyler: "Yeni randevu". */
  title: string;
  /** Bağlam: tarih, şube. */
  description?: string;
  children?: ReactNode;
  /** Sağda en fazla iki buton: "Vazgeç" + asıl eylem. */
  footer?: ReactNode;
  closeLabel?: string;
}

export interface ModalProps extends DialogProps {
  size?: 'md' | 'lg';
}

function DialogFrame({
  kind,
  open,
  onClose,
  title,
  description,
  children,
  footer,
  closeLabel = 'Kapat',
  className,
}: DialogProps & { kind: 'modal' | 'drawer'; className?: string | false | undefined }) {
  const ref = useDialog(open, onClose);
  const titleId = useId();
  const descriptionId = useId();
  if (!open) {
    return null;
  }

  return createPortal(
    <div className={classNames(styles.overlay, kind === 'drawer' && styles.drawerOverlay)}>
      <div
        ref={ref}
        role="dialog"
        aria-modal="true"
        aria-labelledby={titleId}
        aria-describedby={description ? descriptionId : undefined}
        tabIndex={-1}
        className={classNames(kind === 'modal' ? styles.modal : styles.drawer, className)}
      >
        <div className={styles.head}>
          <div>
            <h2 id={titleId} className={styles.title}>
              {title}
            </h2>
            {description ? (
              <p id={descriptionId} className={styles.description}>
                {description}
              </p>
            ) : null}
          </div>
          <IconButton label={closeLabel} icon={<X size={18} />} size="sm" onClick={onClose} />
        </div>
        <div className={styles.body}>{children}</div>
        {footer ? <div className={styles.foot}>{footer}</div> : null}
      </div>
    </div>,
    document.body,
  );
}

/** Odaklı kısa görev. Uzun formlar modal değil sayfa veya yan panel olur. */
export function Modal({ size = 'md', ...props }: ModalProps) {
  return <DialogFrame kind="modal" className={size === 'lg' && styles.lg} {...props} />;
}

/** Yan panel: uzun formlar ve ayrıntılar (personel saatleri, hizmet tanımı) sağdan açılır. */
export function Drawer(props: DialogProps) {
  return <DialogFrame kind="drawer" {...props} />;
}
