import { createFileRoute, redirect } from '@tanstack/react-router';
import { auth } from '@/app/auth';
import { LoginScreen } from '@/features/auth/LoginScreen';

interface LoginSearch {
  yonlendir?: string;
}

function Login() {
  const { yonlendir } = Route.useSearch();
  return <LoginScreen redirectTo={yonlendir} />;
}

export const Route = createFileRoute('/giris')({
  validateSearch: (search: Record<string, unknown>): LoginSearch =>
    typeof search.yonlendir === 'string' ? { yonlendir: search.yonlendir } : {},
  // Oturumu süren kullanıcı panele döner.
  beforeLoad: async ({ context }) => {
    if (auth.getToken() || (await context.refresh())) {
      throw redirect({ to: '/' });
    }
  },
  component: Login,
});
