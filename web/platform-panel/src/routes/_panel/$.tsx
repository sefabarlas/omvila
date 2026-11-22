import { createFileRoute } from '@tanstack/react-router';
import { NotFoundPage } from '@/shared/NotFoundPage';

// Panel içindeki bilinmeyen adres (henüz açılmamış bölümler dahil): 404 kenar menüyle birlikte.
export const Route = createFileRoute('/_panel/$')({ component: NotFoundPage });
