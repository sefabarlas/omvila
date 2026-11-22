import { fixedClock } from '@omvila/ui-web/time';
import { QueryClient } from '@tanstack/react-query';
import { createMemoryHistory } from '@tanstack/react-router';
import { render } from '@testing-library/react';
import { createApi } from '@/app/api';
import { auth } from '@/app/auth';
import { App } from '@/App';
import { createAppRouter } from '@/router';
import { fakeFetch } from './fake-api';
import type { FakeApiOptions } from './fake-api';

/**
 * Uygulamayı verilen adreste sahte API ve sabit saatle (20 Kasım 2026 10:00 İstanbul) açar. `signedIn` (varsayılan)
 * sekmede yenileme jetonu varmış gibi başlar.
 */
export function renderApp(path: string, options: FakeApiOptions = {}) {
  auth.clear();
  if (options.signedIn ?? true) {
    auth.setTokens('', 'yenileme-0');
  }

  const fetcher = fakeFetch(options);
  const api = createApi(fetcher);
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  const router = createAppRouter(
    api,
    queryClient,
    createMemoryHistory({ initialEntries: [path] }),
    fetcher,
  );
  render(
    <App
      api={api}
      queryClient={queryClient}
      router={router}
      clock={fixedClock('2026-11-20T07:00:00Z')}
    />,
  );
  return { router };
}
