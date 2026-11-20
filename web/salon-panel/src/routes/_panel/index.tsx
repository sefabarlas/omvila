import { createFileRoute } from '@tanstack/react-router';
import { Page } from '@/shared/Page';

// Genel bakış: kurulum adımları ve ilk hafta kutuları PR-22'de (GenelBakisBos).
function Overview() {
  return <Page title="Genel bakış" />;
}

export const Route = createFileRoute('/_panel/')({ component: Overview });
