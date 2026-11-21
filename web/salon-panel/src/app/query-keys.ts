// Sorgu anahtarları tek yerden (kodlama-typescript.md § API erişimi).
export const queryKeys = {
  session: () => ['session'] as const,
  onboarding: () => ['onboarding'] as const,
  bookingPage: () => ['booking-page'] as const,
  brand: () => ['brand'] as const,
  brandSettings: () => ['brand-settings'] as const,
  bankAccount: () => ['bank-account'] as const,
  notificationSettings: () => ['notification-settings'] as const,
  smsOverview: () => ['sms-overview'] as const,
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
  /** Müşterinin paketleri ("Paketten" seçeneği); önek ['customers'] ile birlikte geçersizlenir. */
  customerPackages: (brandCustomerId: string) =>
    ['customers', brandCustomerId, 'packages'] as const,
  /** Kasa (şube + yerel gün); önek ['cash'] bütün kasa sorgularını geçersizler. */
  cashDay: (branchId: string, date: string) => ['cash', branchId, date] as const,
  /** Müşteri kartı Paketler sekmesi (imleçli); "Paketten" sorgusundan ayrı anahtar, aynı önek. */
  customerPackageList: (brandCustomerId: string) =>
    ['customers', brandCustomerId, 'packages', 'list'] as const,
  customerPayments: (brandCustomerId: string) =>
    ['customers', brandCustomerId, 'payments'] as const,
  customerReviews: (brandCustomerId: string) => ['customers', brandCustomerId, 'reviews'] as const,
  customerDeletionPreview: (brandCustomerId: string) =>
    ['customers', brandCustomerId, 'deletion'] as const,
  /** KVKK silme talepleri (Ayarlar → KVKK ve veri). */
  deletionRequests: () => ['deletion-requests'] as const,
  /** Değerlendirmeler; önek ['reviews'] bütün filtreleri geçersizler. */
  reviews: (filter: string, branchId: string | undefined) =>
    ['reviews', filter, branchId ?? 'all'] as const,
  /** Bildirim paneli (zil): ilk sayfa ve okunmamış sayısı. */
  notifications: () => ['notifications'] as const,
};
