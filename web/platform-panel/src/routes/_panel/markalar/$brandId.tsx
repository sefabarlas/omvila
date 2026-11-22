import { createFileRoute } from '@tanstack/react-router';
import { BrandDetailPage } from '@/features/brands/BrandDetailPage';
import { brandTabs } from '@/features/brands/labels';
import type { BrandTab } from '@/features/brands/labels';

interface BrandSearch {
  sekme?: BrandTab;
}

function BrandDetail() {
  const { brandId } = Route.useParams();
  const { sekme = 'ozet' } = Route.useSearch();
  return <BrandDetailPage brandId={brandId} tab={sekme} />;
}

export const Route = createFileRoute('/_panel/markalar/$brandId')({
  validateSearch: (search: Record<string, unknown>): BrandSearch =>
    brandTabs.some((t) => t.value === search.sekme) ? { sekme: search.sekme as BrandTab } : {},
  component: BrandDetail,
});
