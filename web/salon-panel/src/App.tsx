import '@omvila/design-tokens/fonts.css';
import '@omvila/design-tokens/panel.css';
import './global.css';
import { ToastProvider } from '@omvila/ui-web/panel';
import { QueryClientProvider } from '@tanstack/react-query';
import type { QueryClient } from '@tanstack/react-query';
import { RouterProvider } from '@tanstack/react-router';
import { systemClock } from '@omvila/ui-web/time';
import type { Clock } from '@omvila/ui-web/time';
import type { Api } from './app/api';
import { ClockContext } from './app/clock';
import { ApiContext } from './app/api-context';
import type { createAppRouter } from './router';

interface AppProps {
  api: Api;
  queryClient: QueryClient;
  router: ReturnType<typeof createAppRouter>;
  clock?: Clock;
}

export function App({ api, queryClient, router, clock = systemClock }: AppProps) {
  return (
    <ClockContext.Provider value={clock}>
      <ApiContext.Provider value={api}>
        <QueryClientProvider client={queryClient}>
          <ToastProvider>
            <RouterProvider router={router} />
          </ToastProvider>
        </QueryClientProvider>
      </ApiContext.Provider>
    </ClockContext.Provider>
  );
}
