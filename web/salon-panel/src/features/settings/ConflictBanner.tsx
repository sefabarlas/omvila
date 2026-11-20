import { Banner, Button } from '@omvila/ui-web/panel';

/** 412 concurrency_conflict: "Kayıt çakışması · Yenile" (salon-paneli/Durumlar). */
export function ConflictBanner({ onReload }: { onReload: () => void }) {
  return (
    <Banner
      tone="uyari"
      title="Kayıt çakışması"
      action={
        <Button size="sm" onClick={onReload}>
          Yenile
        </Button>
      }
    >
      Bu ayarlar siz düzenlerken başka bir oturumda değişti. Yenileyip değişikliklerinizi tekrar
      girin.
    </Banner>
  );
}
