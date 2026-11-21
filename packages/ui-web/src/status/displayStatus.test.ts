import { expect, it } from 'vitest';
import { displayStatus, statusLabels } from './index';

// Kural: AGENTS.md § 5 (durum adları sabit), urun-kurallari.md (yedi durum; Draft gösterilmez; "reddedildi" yok).
it('durum adları sabit', () => {
  expect(Object.values(statusLabels)).toEqual([
    'Onay bekliyor',
    'Kapora bekleniyor',
    'Onaylandı',
    'Geldi',
    'Tamamlandı',
    'Gelmedi',
    'İptal',
  ]);
});

it('API durumu eşlenir, Draft gösterilmez', () => {
  expect(displayStatus('AwaitingDeposit')).toBe('kapora');
  expect(displayStatus('DepositPending')).toBe('kapora');
  expect(displayStatus('Cancelled')).toBe('iptal');
  expect(displayStatus('Draft')).toBeNull();
});
