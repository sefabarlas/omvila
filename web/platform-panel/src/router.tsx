import { QueryClient } from '@tanstack/react-query';
import { createRouter } from '@tanstack/react-router';
import { refreshAccessToken } from './app/api';
import type { Api } from './app/api';
import { routeTree } from './routeTree.gen';

export function createAppRouter(
  api: Api,
  queryClient: QueryClient = new QueryClient(),
  history?: Parameters<typeof createRouter>[0]['history'],
  fetcher?: (request: Request) => Promise<Response>,
) {
  return createRouter({
    routeTree,
    context: { api, queryClient, refresh: () => refreshAccessToken(fetcher) },
    defaultPreload: 'intent',
    ...(history ? { history } : {}),
  });
}

declare module '@tanstack/react-router' {
  interface Register {
    router: ReturnType<typeof createAppRouter>;
  }
}
