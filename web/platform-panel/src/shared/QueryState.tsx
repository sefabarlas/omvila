import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { Banner, Button, Skeleton } from '@omvila/ui-web/panel';
import type { ReactNode } from 'react';
import { ForbiddenPage } from './ForbiddenPage';
import { NotFoundPage } from './NotFoundPage';

interface QueryStateProps<T> {
  query: {
    data: T | undefined;
    error: unknown;
    isPending: boolean;
    refetch: () => Promise<unknown>;
  };
  /** Yükleniyor iskeleti; verilmezse tek satır iskelet. */
  loading?: ReactNode;
  children: (data: T) => ReactNode;
}

/**
 * Sorgunun yükleniyor, hata (403 → yetki sayfası, 404 → bulunamadı, diğerleri → "Tekrar dene" şeridi) ve dolu
 * hâlleri tek yerde.
 */
export function QueryState<T>({ query, loading, children }: QueryStateProps<T>) {
  if (query.data !== undefined) {
    return children(query.data);
  }

  if (query.isPending) {
    return loading ?? <Skeleton variant="block" height="calc(var(--bosluk-12) * 3)" />;
  }

  if (query.error instanceof ApiProblem && query.error.status === 403) {
    return <ForbiddenPage />;
  }

  if (query.error instanceof ApiProblem && query.error.status === 404) {
    return <NotFoundPage />;
  }

  return (
    <Banner
      tone="hata"
      title="Bilgiler yüklenemedi."
      action={
        <Button size="sm" onClick={() => void query.refetch()}>
          Tekrar dene
        </Button>
      }
    >
      {errorMessage(query.error instanceof ApiProblem ? query.error.code : undefined)}
    </Banner>
  );
}
