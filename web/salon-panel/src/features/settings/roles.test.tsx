import { cleanup, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it } from 'vitest';
import { ownerPermissions, sessionFixture, staffPermissions } from '../../test/fake-api';
import { userIds } from '../../test/fake-access';
import { renderApp } from '../../test/render-app';

// Kural: dilim-06 kapsam (Ayarlar → Roller ve yetkiler: kullanıcı listesi, rol matrisi görüntüleme), K6 (rol/şube
// düzenleme, yeni kullanıcı personel daveti akışıyla, markaya özel rol yok); sözleşme manageListUsers, manageUpdateUser
// (409 last_owner_required, 400 branchIds required, If-Match), manageGetRoleMatrix; roller-yetkiler.md (settings.roles
// yalnız Owner); salon-paneli/AyarlarRoller.
afterEach(cleanup);

const usersTable = () => screen.findByRole('table', { name: 'Kullanıcılar' });

describe('Roller ve yetkiler', () => {
  it('kullanıcılar ve rol matrisi: rol, şube, durum; matris onay, "Kendi" ve "—"', async () => {
    renderApp('/ayarlar/roller');
    const table = await usersTable();
    const nav = screen.getByRole('navigation', { name: 'Ayar bölümleri' });
    expect(
      within(nav).getByRole('link', { name: 'Roller ve yetkiler' }).getAttribute('aria-current'),
    ).toBe('page');
    const elif = within(table).getByText('Elif Kaya').closest('tr') as HTMLElement;
    expect(within(elif).getByText('(siz)')).toBeTruthy();
    expect(within(elif).getByText('İşletme sahibi')).toBeTruthy();
    expect(within(elif).getByText('Tümü')).toBeTruthy();
    expect(within(elif).getByText('Aktif')).toBeTruthy();
    const deniz = within(table).getByText('Deniz Kara').closest('tr') as HTMLElement;
    expect(within(deniz).getByText('Merkez')).toBeTruthy();
    expect(within(deniz).getByText('Davet gönderildi')).toBeTruthy();

    const matrix = await screen.findByRole('table', { name: 'Rol izinleri' });
    expect(screen.getByText('İşletme sahibi her şeyi görür')).toBeTruthy();
    const calendar = within(matrix).getByText('Takvim ve randevu').closest('tr') as HTMLElement;
    expect(within(calendar).getByLabelText('Kendi şubesinde')).toBeTruthy();
    expect(within(calendar).getByText('Kendi')).toBeTruthy();
    const settings = within(matrix).getByText('Ayarlar').closest('tr') as HTMLElement;
    expect(within(settings).getAllByLabelText('Yok')).toHaveLength(2);
    // Matris salt okunur: düzenleme denetimi yok.
    expect(within(matrix).queryByRole('button')).toBeNull();
  });

  it('"Düzenle": rol ve şube If-Match ile kaydedilir; işletme sahibinde şube seçimi yok', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/roller', { calls });
    await usersTable();
    await userEvent.click(screen.getByRole('button', { name: 'Deniz Kara: düzenle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Deniz Kara' });
    await userEvent.selectOptions(within(dialog).getByRole('combobox', { name: 'Rol' }), 'Owner');
    expect(within(dialog).queryByRole('checkbox', { name: 'Merkez' })).toBeNull();
    expect(within(dialog).getByText('İşletme sahibi tüm şubeleri ve ayarları görür.')).toBeTruthy();
    await userEvent.selectOptions(
      within(dialog).getByRole('combobox', { name: 'Rol' }),
      'BranchManager',
    );
    expect(within(dialog).getByRole<HTMLInputElement>('checkbox', { name: 'Merkez' }).checked).toBe(
      true,
    );
    await userEvent.click(within(dialog).getByRole('button', { name: 'Kaydet' }));
    expect(await screen.findByText('Deniz Kara için rol kaydedildi')).toBeTruthy();
    const path = `/api/v1/manage/users/${userIds.deniz}`;
    expect(calls[`PATCH ${path} If-Match`]).toEqual(['W/"44"']);
    expect(calls[path]?.[0]).toEqual({
      role: 'BranchManager',
      branchIds: ['0193a8f0-0000-7000-8000-0000000000b1'],
    });
    const row = within(await usersTable())
      .getByText('Deniz Kara')
      .closest('tr') as HTMLElement;
    expect(await within(row).findByText('Şube yöneticisi')).toBeTruthy();
  });

  it('şubesiz personel kaydedilmez: "En az bir şube seçin."', async () => {
    const calls: Record<string, unknown[]> = {};
    renderApp('/ayarlar/roller', { calls });
    await usersTable();
    await userEvent.click(screen.getByRole('button', { name: 'Can Yücel: düzenle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Can Yücel' });
    await userEvent.click(within(dialog).getByRole('checkbox', { name: 'Merkez' }));
    await userEvent.click(within(dialog).getByRole('button', { name: 'Kaydet' }));
    expect(await within(dialog).findByText('En az bir şube seçin.')).toBeTruthy();
    expect(calls[`/api/v1/manage/users/${userIds.can}`]).toBeUndefined();
  });

  it('son işletme sahibi başka role alınamaz: 409 last_owner_required metni pencerede', async () => {
    renderApp('/ayarlar/roller', { access: { updateError: 'last_owner_required' } });
    await usersTable();
    await userEvent.click(screen.getByRole('button', { name: 'Elif Kaya: düzenle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Elif Kaya' });
    await userEvent.selectOptions(within(dialog).getByRole('combobox', { name: 'Rol' }), 'Staff');
    await userEvent.click(within(dialog).getByRole('checkbox', { name: 'Merkez' }));
    await userEvent.click(within(dialog).getByRole('button', { name: 'Kaydet' }));
    expect(
      await within(dialog).findByText('İşletmede en az bir işletme sahibi kalmalı.'),
    ).toBeTruthy();
    expect(within(dialog).getByText('Rol kaydedilemedi')).toBeTruthy();
  });

  it('412: kayıt çakışması şeridi', async () => {
    renderApp('/ayarlar/roller', { access: { updateError: 'concurrency_conflict' } });
    await usersTable();
    await userEvent.click(screen.getByRole('button', { name: 'Can Yücel: düzenle' }));
    const dialog = await screen.findByRole('dialog', { name: 'Can Yücel' });
    await userEvent.click(within(dialog).getByRole('button', { name: 'Kaydet' }));
    expect(await within(dialog).findByText('Kayıt çakışması')).toBeTruthy();
  });

  it('"Kullanıcı ekle" personel daveti akışını açar', async () => {
    renderApp('/ayarlar/roller');
    await usersTable();
    await userEvent.click(screen.getByRole('button', { name: 'Kullanıcı ekle' }));
    expect(await screen.findByRole('dialog', { name: 'Personel ekle' })).toBeTruthy();
  });

  it('boş liste ve yükleme hatası', async () => {
    renderApp('/ayarlar/roller', { access: { noUsers: true } });
    expect(await screen.findByRole('heading', { name: 'Kullanıcı yok' })).toBeTruthy();
    cleanup();
    renderApp('/ayarlar/roller', { access: { fails: true } });
    expect(await screen.findByRole('heading', { name: 'Kullanıcılar yüklenemedi' })).toBeTruthy();
    expect(screen.getByRole('heading', { name: 'Rol izinleri yüklenemedi' })).toBeTruthy();
  });

  it('settings.roles yoksa menüde görünmez, doğrudan adreste yetkiniz yok', async () => {
    renderApp('/ayarlar/roller', {
      session: sessionFixture({
        membership: { role: 'BranchManager', branchIds: [], staffId: null },
        permissions: ownerPermissions.filter((p) => p !== 'settings.roles'),
      }),
    });
    expect(await screen.findByText('Bu sayfayı görme yetkiniz yok')).toBeTruthy();
    cleanup();
    renderApp('/ayarlar/isletme', {
      session: sessionFixture({
        permissions: ownerPermissions.filter((p) => p !== 'settings.roles'),
      }),
    });
    const nav = await screen.findByRole('navigation', { name: 'Ayar bölümleri' });
    expect(within(nav).queryByRole('link', { name: 'Roller ve yetkiler' })).toBeNull();
    cleanup();
    renderApp('/ayarlar/roller', {
      session: sessionFixture({
        membership: { role: 'Staff', branchIds: [], staffId: null },
        permissions: staffPermissions,
      }),
    });
    expect(await screen.findByText('Bu sayfayı görme yetkiniz yok')).toBeTruthy();
  });
});
