// Sorgu anahtarları tek yerden (kodlama-typescript.md § API erişimi).
export const queryKeys = {
  session: () => ['session'] as const,
  onboarding: () => ['onboarding'] as const,
  bookingPage: () => ['booking-page'] as const,
  brand: () => ['brand'] as const,
  brandSettings: () => ['brand-settings'] as const,
  branch: (branchId: string) => ['branch', branchId] as const,
  workingHours: (branchId: string) => ['branch', branchId, 'working-hours'] as const,
  overrides: (branchId: string, from: string) => ['branch', branchId, 'overrides', from] as const,
  services: (branchId: string | undefined) => ['services', branchId ?? 'brand'] as const,
  serviceCategories: () => ['service-categories'] as const,
  serviceTemplates: (industryCode: string) => ['service-templates', industryCode] as const,
  staff: () => ['staff'] as const,
  staffDetail: (staffId: string) => ['staff', staffId] as const,
  /** Takvim (gün: from = to; hafta: tek personel). Önek ['calendar'] bütün takvim sorgularını geçersizler. */
  calendar: (branchId: string, from: string, to: string, staffId?: string) =>
    ['calendar', branchId, from, to, staffId ?? 'all'] as const,
  appointment: (appointmentId: string) => ['appointment', appointmentId] as const,
  /** Onay kuyruğu (manageListPendingApprovals); önek ['approval-queue'] bütün şubeleri geçersizler. */
  approvalQueue: (branchId: string | undefined) => ['approval-queue', branchId ?? 'all'] as const,
  /** Yeni randevu boş saatleri (hizmetler ve personel). */
  slots: (branchId: string, date: string, serviceIds: readonly string[], staffId: string) =>
    ['slots', branchId, date, serviceIds.join(','), staffId] as const,
  /** Müşteri listesi (arama + filtre); önek ['customers'] bütün müşteri sorgularını geçersizler. */
  customers: (q: string, filter: string) => ['customers', 'list', q, filter] as const,
  /** Yeni randevu müşteri araması (önek ['customers'] ile birlikte geçersizlenir). */
  customerSearch: (q: string) => ['customers', 'picker', q] as const,
  customer: (brandCustomerId: string) => ['customers', brandCustomerId] as const,
  customerAppointments: (brandCustomerId: string) =>
    ['customers', brandCustomerId, 'appointments'] as const,
};
