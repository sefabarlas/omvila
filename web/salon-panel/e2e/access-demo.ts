import type { Page } from '@playwright/test';
import { ownerPermissions } from './calendar-demo';
import { sessionAs } from './support';

// Roller ve yetkiler (salon-paneli/AyarlarRoller) ve Personel'deki Google Takvim kartı (salon-paneli/Personel) için
// tasarımdaki veriler. Prism örneğinde kullanıcı listesi iki, matris üç satır ve personel tek kişi olduğu için yanıtı
// test verir. Kurgusal kişiler.
const corsJson = (body: unknown, status = 200) => ({
  status,
  contentType: 'application/json',
  headers: {
    'Access-Control-Allow-Origin': 'http://localhost:5180',
    'Access-Control-Allow-Credentials': 'true',
  },
  body: JSON.stringify(body),
});

export const kadikoy = '0193a8f0-0000-7000-8000-0000000000b1';
/** Prism'in personel ayrıntısı örneğinin kimliği: oturumdaki kullanıcının kendi personel kaydı. */
const ownStaffId = '497f6eca-6276-4993-bfeb-53cbbbba6f08';

export const demoUsers = [
  ['0193a8f0-0000-7000-8000-000000000001', 'Elif Kaya', 'Owner', 'Active', ownStaffId],
  [
    '0193a8f0-0000-7000-8000-000000000003',
    'Can Yücel',
    'BranchManager',
    'Active',
    '0193a8f0-0000-7000-8000-0000000000c3',
  ],
  [
    '0193a8f0-0000-7000-8000-000000000002',
    'Mert Demir',
    'Staff',
    'Active',
    '0193a8f0-0000-7000-8000-0000000000c2',
  ],
  [
    '0193a8f0-0000-7000-8000-000000000004',
    'Deniz Kara',
    'Staff',
    'Invited',
    '0193a8f0-0000-7000-8000-0000000000c5',
  ],
].map(([userId, fullName, role, status, staffId], index) => ({
  userId,
  fullName,
  phoneMasked: `0532 *** ** ${String(10 + index)}`,
  role,
  branchIds: role === 'Owner' ? [] : [kadikoy],
  status,
  staffId,
  isCurrentUser: index === 0,
  version: `W/"${String(100 + index)}"`,
}));

const matrix = {
  rows: [
    ['CalendarAndAppointments', 'Branch', 'Own'],
    ['CustomerPhone', 'All', 'None'],
    ['CustomerCardAndNotes', 'All', 'All'],
    ['CashAndPayments', 'Branch', 'None'],
    ['ReportsAndCommission', 'Branch', 'Own'],
    ['ServicesAndPrices', 'Branch', 'None'],
    ['Settings', 'None', 'None'],
  ].map(([group, branchManager, staff]) => ({
    group,
    permissions: [],
    owner: 'All',
    branchManager,
    staff,
  })),
};

const staff = [
  [ownStaffId, 'Elif Kaya', 'Kurucu · kuaför', 'Owner', '#0F6E5C', true],
  ['0193a8f0-0000-7000-8000-0000000000c2', 'Mert Demir', 'Berber', 'Staff', '#C2410C', true],
  [
    '0193a8f0-0000-7000-8000-0000000000c4',
    'Zeynep Aksoy',
    'Renk ve bakım',
    'Staff',
    '#7C3AED',
    true,
  ],
  [
    '0193a8f0-0000-7000-8000-0000000000c3',
    'Can Yücel',
    'Kuaför',
    'BranchManager',
    '#1D4ED8',
    false,
  ],
].map(([id, fullName, title, role, calendarColor, isBookableOnline]) => ({
  id,
  fullName,
  displayName: fullName,
  title,
  photoUrl: null,
  calendarColor,
  isBookableOnline,
  isActive: true,
  branchIds: [kadikoy],
  role,
  membershipStatus: 'Active',
}));

async function fulfill(page: Page, pattern: RegExp, body: () => unknown) {
  await page.route(pattern, (route) =>
    route.request().method() === 'GET' ? route.fulfill(corsJson(body())) : route.fallback(),
  );
}

/** Sahip oturumu: settings.roles ile; şube Kadıköy, kendi personel kaydı Prism örneğindeki kişi. */
export async function ownerSession(page: Page) {
  await sessionAs(page, {
    permissions: [...ownerPermissions, 'settings.roles'],
    membership: { role: 'Owner', branchIds: [], staffId: ownStaffId },
    branches: [{ id: kadikoy, name: 'Kadıköy' }],
  });
}

/** AyarlarRoller: dört kullanıcı ve yedi satırlık matris. */
export async function rolesDemo(page: Page) {
  await ownerSession(page);
  await fulfill(page, /\/api\/v1\/manage\/users$/, () => ({ items: demoUsers }));
  await fulfill(page, /\/api\/v1\/manage\/roles$/, () => matrix);
}

/** Personel: dört kişi; seçili kişi (Elif Kaya) oturumdaki kullanıcı, Google Takvim bağlı (Prism örneği). */
export async function staffDemo(page: Page) {
  await ownerSession(page);
  await fulfill(page, /\/api\/v1\/manage\/staff(\?.*)?$/, () => ({ items: staff }));
}
