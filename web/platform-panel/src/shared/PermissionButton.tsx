import { Button, Tooltip } from '@omvila/ui-web/panel';
import type { ButtonProps } from '@omvila/ui-web/panel';
import { Lock } from 'lucide-react';
import type { AdminPermission } from '@/app/api';
import { useSession } from '@/app/session';

export interface PermissionButtonProps extends ButtonProps {
  permission: AdminPermission;
  /** İzin yoksa ipucu; varsayılan "Yönetici yetkisi ister". */
  lockedHint?: string;
}

/** İzinli kullanıcıda buton; izni olmayanda kilitli, pasif ve nedeni ipucunda (KK4). */
export function PermissionButton({
  permission,
  lockedHint = 'Yönetici yetkisi ister',
  children,
  ...button
}: PermissionButtonProps) {
  const { can } = useSession();
  if (can(permission)) {
    return <Button {...button}>{children}</Button>;
  }

  return (
    <Tooltip content={lockedHint}>
      <Button size={button.size ?? 'md'} disabled icon={<Lock size={14} aria-hidden="true" />}>
        {children}
      </Button>
    </Tooltip>
  );
}

/** Yönetici ve Finans rollerinin ortak izinleri (plan, fatura, banka hareketi) için ipucu. */
export const adminOrFinanceHint = 'Yönetici veya Finans yetkisi ister';
