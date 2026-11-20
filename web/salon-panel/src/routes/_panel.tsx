import { createFileRoute, redirect } from '@tanstack/react-router';
import { AppLayout } from '@/app/AppLayout';
import { auth } from '@/app/auth';
import { sessionQuery } from '@/app/session';
import { NotFoundPage } from '@/shared/NotFoundPage';

// Oturum gerektiren bütün sayfaların düzeni. Jeton bellekte yoksa yenileme çereziyle alınır (sayfa yenilendi);
// o da yoksa girişe yönlenir. Oturum özeti (rol, izinler, şubeler) burada yüklenir.
export const Route = createFileRoute('/_panel')({
  beforeLoad: async ({ context, location }) => {
    if (!auth.getToken() && !(await context.refresh())) {
      throw redirect({ to: '/giris', search: { yonlendir: location.href } });
    }

    try {
      await context.queryClient.query(sessionQuery(context.api));
    } catch {
      auth.setToken(null);
      throw redirect({ to: '/giris', search: { yonlendir: location.href } });
    }
  },
  component: AppLayout,
  notFoundComponent: NotFoundPage,
});
