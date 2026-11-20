import type { StoryDefault } from '@ladle/react';
import { Scissors } from 'lucide-react';
import { Banner } from './Banner';
import { Button } from './Button';
import { EmptyState } from './EmptyState';
import { previewStack } from './preview';
import { Skeleton } from './Skeleton';
import { ToastProvider, useToast } from './Toast';

export default { title: 'Panel/Geri bildirim' } satisfies StoryDefault;

export const Seritler = () => (
  <div style={{ ...previewStack, maxWidth: '720px' }}>
    <Banner
      title="Deneme sürenizin bitmesine 9 gün var"
      action={<Button size="sm">Planları gör</Button>}
    >
      Deneme bitince kartla veya havaleyle devam edebilirsiniz.
    </Banner>
    <Banner tone="uyari" title="SMS kontörünüz azaldı">
      42 SMS kaldı, tahmini 6 gün yeter.
    </Banner>
    <Banner tone="hata" title="Abonelik ödemesi alınamadı" />
    <Banner tone="basari" title="Kapora geldi" />
  </div>
);

export const BosDurum = () => (
  <EmptyState
    icon={<Scissors size={22} />}
    title="İlk hizmetinizi ekleyin"
    description="Online randevu için en az bir hizmet gerekir. Her hizmetin süresi, fiyatı ve kimlerin yaptığı olur."
    actions={
      <>
        <Button variant="primary">Hazır listeden başla</Button>
        <Button>Boş hizmet ekle</Button>
      </>
    }
  />
);

function ToastButtons() {
  const toast = useToast();
  return (
    <div style={{ display: 'flex', gap: 'var(--bosluk-2)' }}>
      <Button
        onClick={() => {
          toast.show({
            title: 'Randevu oluşturuldu',
            message: 'Salı 25 Kasım 14:30 · Saç kesimi',
            action: { label: 'Geri al', onClick: () => undefined },
          });
        }}
      >
        Başarılı
      </Button>
      <Button
        onClick={() => {
          toast.show({
            tone: 'hata',
            title: 'Randevu kaydedilemedi',
            message: 'Bu saat az önce doldu.',
          });
        }}
      >
        Hata
      </Button>
    </div>
  );
}

export const Bildirimler = () => (
  <ToastProvider>
    <ToastButtons />
  </ToastProvider>
);

export const Iskelet = () => (
  <div style={previewStack}>
    <Skeleton delayMs={0} width="60%" />
    <Skeleton delayMs={0} />
    <Skeleton delayMs={0} variant="block" height="var(--tablo-satir)" />
    <Skeleton delayMs={0} variant="circle" width="var(--kontrol-md)" height="var(--kontrol-md)" />
  </div>
);
