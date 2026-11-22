import { createFileRoute } from '@tanstack/react-router';
import { BrandsPage } from '@/features/brands/BrandsPage';
import { brandFilters } from '@/features/brands/labels';
import type { BrandFilter } from '@/features/brands/labels';

interface BrandsSearch {
  filtre?: BrandFilter;
  q?: string;
}

function Brands() {
  const { filtre = 'All', q = '' } = Route.useSearch();
  return <BrandsPage filter={filtre} q={q} />;
}

export const Route = createFileRoute('/_panel/markalar/')({
  validateSearch: (search: Record<string, unknown>): BrandsSearch => ({
    ...(brandFilters.some((f) => f.value === search.filtre)
      ? { filtre: search.filtre as BrandFilter }
      : {}),
    ...(typeof search.q === 'string' && search.q.trim() !== '' ? { q: search.q.trim() } : {}),
  }),
  component: Brands,
});
