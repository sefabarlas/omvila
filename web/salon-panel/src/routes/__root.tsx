import type { QueryClient } from '@tanstack/react-query';
import { createRootRouteWithContext, Outlet } from '@tanstack/react-router';
import type { Api } from '@/app/api';
import { useThemeEffect } from '@/app/theme';
import { NotFoundPage } from '@/shared/NotFoundPage';

export interface RouterContext {
  queryClient: QueryClient;
  api: Api;
  /** Yenileme çereziyle erişim jetonu (testte sahte API). */
  refresh: () => Promise<boolean>;
}

function Root() {
  useThemeEffect();
  return <Outlet />;
}

export const Route = createRootRouteWithContext<RouterContext>()({
  component: Root,
  notFoundComponent: NotFoundPage,
});
