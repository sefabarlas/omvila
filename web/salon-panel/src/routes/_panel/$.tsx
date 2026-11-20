import { createFileRoute } from '@tanstack/react-router';
import { NotFoundPage } from '@/shared/NotFoundPage';

// Panel içindeki bilinmeyen adres: 404 kenar menüyle birlikte (salon-paneli/Durumlar).
export const Route = createFileRoute('/_panel/$')({ component: NotFoundPage });
