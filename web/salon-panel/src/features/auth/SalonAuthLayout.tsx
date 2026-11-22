import { AuthLayout } from '@omvila/ui-web/panel';
import type { ReactNode } from 'react';

/** Salon paneli giriş ve kayıt düzeni (salon-paneli/Giris): ortak düzen, salonun sloganıyla. */
export function SalonAuthLayout({ children }: { children: ReactNode }) {
  return (
    <AuthLayout
      sloganTitle={
        <>
          Her randevu
          <br />
          tek yerde.
        </>
      }
      sloganText="Takvim, müşteriler, personel ve kasa. Salonunuzun bütün günü tek ekranda."
    >
      {children}
    </AuthLayout>
  );
}
