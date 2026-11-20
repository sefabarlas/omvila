import { createFileRoute } from '@tanstack/react-router';
import { BranchSettingsPage } from '@/features/settings/BranchSettingsPage';

// Veri seçili şubeye göre sayfada yüklenir (kenar menüdeki seçim yönlendiricide değil, bağlamda).
export const Route = createFileRoute('/_panel/ayarlar/sube')({
  component: BranchSettingsPage,
});
