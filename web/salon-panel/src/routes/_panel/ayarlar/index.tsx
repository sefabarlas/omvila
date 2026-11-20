import { createFileRoute, redirect } from '@tanstack/react-router';

// Menüdeki "Ayarlar" ilk bölümü açar.
export const Route = createFileRoute('/_panel/ayarlar/')({
  beforeLoad: () => {
    throw redirect({ to: '/ayarlar/isletme', replace: true });
  },
});
