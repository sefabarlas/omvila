import { describe, expect, it, vi } from 'vitest';
import { noIndex, offsetText, reminderText, signInUrl, signUpUrl } from './site';

describe('salon paneli adresleri', () => {
  it('NEXT_PUBLIC_PANEL_URL kökünden /giris ve /kayit', () => {
    vi.stubEnv('NEXT_PUBLIC_PANEL_URL', 'https://salon.test.omvila.com.tr/');
    expect(signInUrl()).toBe('https://salon.test.omvila.com.tr/giris');
    expect(signUpUrl()).toBe('https://salon.test.omvila.com.tr/kayit');
  });
});

describe('noIndex', () => {
  it('yalnız "true" değerinde kapalı', () => {
    vi.stubEnv('NEXT_PUBLIC_NOINDEX', 'true');
    expect(noIndex()).toBe(true);
    vi.stubEnv('NEXT_PUBLIC_NOINDEX', '');
    expect(noIndex()).toBe(false);
  });
});

describe('hatırlatma zamanları', () => {
  it('dakikayı okunur metne çevirir', () => {
    expect(offsetText(1440)).toBe('bir gün');
    expect(offsetText(120)).toBe('iki saat');
    expect(offsetText(30)).toBe('30 dakika');
    expect(reminderText([1440, 120])).toBe('bir gün ve iki saat');
    expect(reminderText([2880, 1440, 60])).toBe('iki gün, bir gün ve bir saat');
    expect(reminderText([120])).toBe('iki saat');
  });
});
