import { createFileRoute } from '@tanstack/react-router';

interface LoginSearch {
  yonlendir?: string;
}

// Giriş, kod ve kayıt ekranları PR-21'de (Giris, GirisKod, Kayit). Bu rota yönlendirme hedefi olarak şimdiden var.
function Login() {
  return (
    <main>
      <h1>Panele giriş</h1>
    </main>
  );
}

export const Route = createFileRoute('/giris')({
  validateSearch: (search: Record<string, unknown>): LoginSearch =>
    typeof search.yonlendir === 'string' ? { yonlendir: search.yonlendir } : {},
  component: Login,
});
