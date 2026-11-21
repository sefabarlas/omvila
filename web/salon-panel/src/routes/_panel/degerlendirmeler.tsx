import { createFileRoute } from '@tanstack/react-router';
import { ReviewsPage } from '@/features/reviews/ReviewsPage';
import type { ReviewsSearch } from '@/features/reviews/ReviewsPage';
import { RequirePermission } from '@/shared/RequirePermission';

// Değerlendirmeler (review.view: sahip, şube müdürü kendi şubesinde; personel 403). Filtre adreste (?filtre=tumu).
function ReviewsRoute() {
  const search = Route.useSearch();
  return (
    <RequirePermission permission="review.view">
      <ReviewsPage search={search} />
    </RequirePermission>
  );
}

export const Route = createFileRoute('/_panel/degerlendirmeler')({
  validateSearch: (search: Record<string, unknown>): ReviewsSearch =>
    typeof search.filtre === 'string' ? { filtre: search.filtre } : {},
  component: ReviewsRoute,
});
