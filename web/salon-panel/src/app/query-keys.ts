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
};
