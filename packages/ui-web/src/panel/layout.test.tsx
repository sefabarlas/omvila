import { cleanup, render, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { Calendar, Scissors } from 'lucide-react';
import { useState } from 'react';
import { afterEach, describe, expect, it, vi } from 'vitest';
import { Bento, BentoBlock, DataTable, PersonCell, Sidebar, StatCard, Topbar } from './index';

// Kural: tasarım sistemi README'leri (Sidebar: tek şubede seçici gizli, sayaç yalnız eylem bekleyen; Topbar: ⌘K;
// DataTable: sayılar sağda, seçim; StatCard: anlam rengi), urun-kurallari.md (boş durumda sahte sayı/rozet yok).
afterEach(cleanup);

const groups = [
  {
    items: [
      { key: 'genel', label: 'Genel bakış', icon: <Calendar size={18} />, href: '/genel' },
      { key: 'takvim', label: 'Takvim', icon: <Calendar size={18} />, href: '/takvim', count: 3 },
    ],
  },
  {
    title: 'Yönetim',
    items: [
      {
        key: 'hizmetler',
        label: 'Hizmetler',
        icon: <Scissors size={18} />,
        href: '/hizmetler',
        count: 0,
      },
    ],
  },
];

describe('Sidebar', () => {
  it('etkin öğe aria-current, sayaç yalnız sıfırdan büyükse, tıklama yönlendiriciye gider', async () => {
    const onNavigate = vi.fn();
    render(<Sidebar groups={groups} active="genel" onNavigate={onNavigate} />);
    const nav = screen.getByRole('navigation', { name: 'Ana menü' });
    expect(
      within(nav).getByRole('link', { name: 'Genel bakış' }).getAttribute('aria-current'),
    ).toBe('page');
    expect(within(nav).getByRole('link', { name: /Takvim/ }).textContent).toContain('3');
    expect(within(nav).getByRole('link', { name: 'Hizmetler' }).textContent).toBe('Hizmetler');
    await userEvent.click(within(nav).getByRole('link', { name: 'Hizmetler' }));
    expect(onNavigate).toHaveBeenCalledWith(expect.objectContaining({ key: 'hizmetler' }));
  });

  it('"Çıkış yap" yalnız onSignOut verilince ve tıklanınca çağrılır', async () => {
    const onSignOut = vi.fn();
    const { unmount } = render(
      <Sidebar
        groups={groups}
        active="genel"
        user={{ name: 'Elif Kaya', role: 'İşletme sahibi' }}
      />,
    );
    expect(screen.queryByRole('button', { name: 'Çıkış yap' })).toBeNull();
    unmount();
    render(
      <Sidebar
        groups={groups}
        active="genel"
        user={{ name: 'Elif Kaya', role: 'İşletme sahibi' }}
        onSignOut={onSignOut}
      />,
    );
    await userEvent.click(screen.getByRole('button', { name: 'Çıkış yap' }));
    expect(onSignOut).toHaveBeenCalledTimes(1);
  });

  it('şube seçici yalnız birden çok şubede', async () => {
    const onBranchChange = vi.fn();
    const { rerender } = render(
      <Sidebar groups={groups} active="genel" branches={[{ id: 'k', name: 'Kadıköy' }]} />,
    );
    expect(screen.queryByLabelText('Şube')).toBeNull();
    rerender(
      <Sidebar
        groups={groups}
        active="genel"
        branches={[
          { id: 'k', name: 'Kadıköy' },
          { id: 'n', name: 'Nişantaşı' },
        ]}
        branchId="k"
        onBranchChange={onBranchChange}
      />,
    );
    await userEvent.selectOptions(screen.getByLabelText('Şube'), 'n');
    expect(onBranchChange).toHaveBeenCalledWith('n');
  });
});

describe('Topbar', () => {
  it('tek h1, ⌘K aramaya odaklar, bildirim sayısı adda', async () => {
    const onSearch = vi.fn();
    render(
      <Topbar
        title="Hizmetler"
        context="Kadıköy fiyatları"
        search={{ placeholder: 'Ara', label: 'Genel arama', onSearch }}
        notifications={{ count: 3, onOpen: () => undefined }}
      />,
    );
    expect(screen.getByRole('heading', { level: 1, name: 'Hizmetler' })).toBeTruthy();
    await userEvent.keyboard('{Control>}k{/Control}');
    expect(document.activeElement).toBe(screen.getByRole('searchbox', { name: 'Genel arama' }));
    await userEvent.keyboard('Ayşe{Enter}');
    expect(onSearch).toHaveBeenCalledWith('Ayşe');
    expect(screen.getByRole('button', { name: 'Bildirimler, 3 okunmamış' })).toBeTruthy();
  });

  it('bildirim yoksa rozet yok', () => {
    render(<Topbar title="Genel bakış" notifications={{ count: 0, onOpen: () => undefined }} />);
    expect(screen.getByRole('button', { name: 'Bildirimler' }).parentElement?.textContent).toBe('');
  });

  it('bildirim paneli zilin yanında, zil açık/kapalı durumunu bildirir', () => {
    render(
      <Topbar
        title="Genel bakış"
        notifications={{
          count: 1,
          onOpen: () => undefined,
          expanded: true,
          controls: 'panel',
          panel: <div id="panel">Bildirimler paneli</div>,
        }}
      />,
    );
    const bell = screen.getByRole('button', { name: 'Bildirimler, 1 okunmamış' });
    expect(bell.getAttribute('aria-expanded')).toBe('true');
    expect(bell.getAttribute('aria-controls')).toBe('panel');
    expect(bell.parentElement?.textContent).toContain('Bildirimler paneli');
  });
});

interface Row {
  id: string;
  name: string;
  visits: number;
}

function TableHarness({ onRowClick }: { onRowClick: (row: Row) => void }) {
  const [selected, setSelected] = useState<Set<string>>(new Set());
  return (
    <DataTable<Row>
      caption="Müşteriler"
      rows={[
        { id: '1', name: 'Ayşe Yılmaz', visits: 14 },
        { id: '2', name: 'Selin Öztürk', visits: 3 },
      ]}
      rowKey={(row) => row.id}
      onRowClick={onRowClick}
      selection={{ selected, onChange: setSelected, rowLabel: (row) => `${row.name} seç` }}
      columns={[
        {
          key: 'name',
          label: 'Müşteri',
          render: (row) => <PersonCell name={row.name} detail="0532 *** ** 12" />,
        },
        { key: 'visits', label: 'Ziyaret', numeric: true, render: (row) => row.visits },
      ]}
      footer="2 müşteriden 1–2"
    />
  );
}

describe('DataTable', () => {
  it('başlık, sayısal kolon sağda, seçim satırı tıklamasını tetiklemez, Enter detayı açar', async () => {
    const onRowClick = vi.fn();
    render(<TableHarness onRowClick={onRowClick} />);
    const table = screen.getByRole('table', { name: 'Müşteriler' });
    expect(within(table).getByRole('columnheader', { name: 'Ziyaret' }).className).toContain(
      'right',
    );

    await userEvent.click(screen.getByRole('checkbox', { name: 'Ayşe Yılmaz seç' }));
    const box = screen.getByRole('checkbox', { name: 'Selin Öztürk seç' }).nextElementSibling;
    if (box instanceof HTMLElement) {
      await userEvent.click(box);
      await userEvent.click(box);
    }
    expect(onRowClick).not.toHaveBeenCalled();
    expect(
      screen.getByRole<HTMLInputElement>('checkbox', { name: 'Ayşe Yılmaz seç' }).checked,
    ).toBe(true);

    await userEvent.click(screen.getByRole('checkbox', { name: 'Tümünü seç' }));
    expect(
      screen.getByRole<HTMLInputElement>('checkbox', { name: 'Selin Öztürk seç' }).checked,
    ).toBe(true);

    const row = screen.getAllByRole('row')[2];
    row?.focus();
    await userEvent.keyboard('{Enter}');
    expect(onRowClick).toHaveBeenCalledWith(expect.objectContaining({ id: '2' }));
  });

  it('boş listede tablo yerine boş durum', () => {
    render(
      <DataTable
        caption="Müşteriler"
        rows={[]}
        rowKey={() => ''}
        columns={[]}
        empty={<p>Henüz müşteri yok</p>}
      />,
    );
    expect(screen.queryByRole('table')).toBeNull();
    expect(screen.getByText('Henüz müşteri yok')).toBeTruthy();
  });
});

it('StatCard: değişim anlam rengiyle, veri yoksa tire', () => {
  render(
    <>
      <StatCard
        label="Gelmeyen"
        value="4"
        delta={{ text: '2', direction: 'up', meaning: 'bad' }}
        context="geçen salıya göre"
      />
      <StatCard label="Yeni müşteri" value="—" />
    </>,
  );
  const noShow = screen.getByRole('region', { name: 'Gelmeyen' });
  expect(within(noShow).getByText('2').className).toContain('bad');
  expect(within(screen.getByRole('region', { name: 'Yeni müşteri' })).getByText('—')).toBeTruthy();
});

it('BentoBlock bağlantılı veya düz', () => {
  render(
    <Bento label="Bugün">
      <BentoBlock tone="vurgu" label="Onay bekliyor" value="3" href="/onay" />
      <BentoBlock label="Boş saat" value="5" />
    </Bento>,
  );
  expect(screen.getByRole('group', { name: 'Bugün' })).toBeTruthy();
  expect(screen.getByRole('link', { name: /Onay bekliyor/ }).getAttribute('href')).toBe('/onay');
});
