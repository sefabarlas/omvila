import type { QueryClient } from '@tanstack/react-query';
import { createRootRouteWithContext, Outlet } from '@tanstack/react-router';
import type { Api } from '@/app/api';
import { ThemeProvider } from '@/app/theme';
import { NotFoundPage } from '@/shared/NotFoundPage';

export interface RouterContext {
  queryClient: QueryClient;
  api: Api;
  /** Saklanan yenileme jetonuyla erişim jetonu (testte sahte API). */
  refresh: () => Promise<boolean>;
}

function Root() {
  return (
    <ThemeProvider>
      <Outlet />
    </ThemeProvider>
  );
}

export const Route = createRootRouteWithContext<RouterContext>()({
  component: Root,
  notFoundComponent: NotFoundPage,
});
