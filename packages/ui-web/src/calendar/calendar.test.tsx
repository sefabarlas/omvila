import { act, cleanup, fireEvent, render, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it, vi } from 'vitest';
import {
  AppointmentBlock,
  CalendarClosedDay,
  CalendarEmptyDay,
  CalendarGrid,
  WeekCalendar,
} from './index';
import type { CalendarBlock } from './index';
import {
  previewBlocks,
  previewDate,
  previewNow,
  previewStaff,
  previewTimeZone,
  previewWeekBlocks,
  previewWeekDays,
} from './preview-data';

// Kural: web/salon-panel/AGENTS.md § Takvim, panel README § Takvim, AppointmentBlock/DayCalendar/WeekCalendar README.
afterEach(cleanup);

const day = {
  date: previewDate,
  timeZone: previewTimeZone,
  openTime: '09:00',
  closeTime: '19:00',
  staff: previewStaff,
  blocks: previewBlocks,
};

function styleVar(element: HTMLElement, name: string): string {
  return element.style.getPropertyValue(name);
}

function must<T>(value: T | null | undefined): T {
  if (value === null || value === undefined) {
    throw new Error('Öğe bulunamadı');
  }

  return value;
}

describe('CalendarGrid (gün)', () => {
  it('bölge adlı, personel başlıkları ve saat etiketleri', () => {
    const { container } = render(<CalendarGrid {...day} />);
    const region = screen.getByRole('region', { name: 'Günlük takvim' });
    expect(within(region).getByText('Elif Kaya')).toBeTruthy();
    expect(within(region).getByText('5 randevu')).toBeTruthy();
    expect(within(region).getByText('4 randevu · bekleyen var')).toBeTruthy();
    const gutter = container.querySelector<HTMLElement>('.gutter');
    expect(gutter?.textContent).toBe('09:0010:0011:0012:0013:0014:0015:0016:0017:0018:00');
  });

  it('personel rengi yalnız avatar halkasında', () => {
    const { container } = render(<CalendarGrid {...day} />);
    const rings = container.querySelectorAll<HTMLElement>('[class*="staffRing"]');
    expect(rings).toHaveLength(4);
    expect(styleVar(must(rings[0]), '--personel-rengi')).toBe('#14857A');
    for (const block of container.querySelectorAll<HTMLElement>('[data-status]')) {
      expect(styleVar(block, '--personel-rengi')).toBe('');
    }
  });

  it('blok erişilebilir adı: müşteri, hizmet, saat, durum', () => {
    render(<CalendarGrid {...day} />);
    expect(
      screen.getByRole('button', { name: 'Selin Öztürk, Boya ve fön, 10:30–12:30, Geldi' }),
    ).toBeTruthy();
    expect(
      screen.getByRole('button', {
        name: 'Derya Şahin, Saç kesimi, 13:00–13:45, Onaylandı, tekrarlayan',
      }),
    ).toBeTruthy();
    expect(
      screen.getByRole('button', { name: 'Gizem Arslan, Fön, 14:30–15:00, Kapora bekleniyor' }),
    ).toBeTruthy();
  });

  it('blok şube saatine göre konumlanır (10:30 → 1,5 saat, 2 saat boy)', () => {
    render(<CalendarGrid {...day} />);
    const block = screen.getByRole('button', { name: /^Selin Öztürk/ });
    expect(styleVar(block, '--blok-ust')).toBe('1.5');
    expect(styleVar(block, '--blok-boy')).toBe('2');
    expect(block.dataset.status).toBe('geldi');
  });

  it('bekleme dilimi taralı "Bekleme · müsait" şeridi', () => {
    render(<CalendarGrid {...day} />);
    const block = screen.getByRole('button', { name: /^Selin Öztürk/ });
    const stripe = within(block).getByTestId('bekleme-seridi');
    expect(stripe.textContent).toBe('Bekleme · müsait');
    // 10:45–11:30, blok başından 15 dk sonra, 45 dk.
    expect(styleVar(stripe, '--bekleme-ust')).toBe('0.25');
    expect(styleVar(stripe, '--bekleme-boy')).toBe('0.75');
    expect(screen.getAllByTestId('bekleme-seridi')).toHaveLength(1);
  });

  it('şu anki saat çizgisi ve etiketi enjekte edilen andan', () => {
    const { container } = render(<CalendarGrid {...day} now={previewNow} />);
    expect(screen.getByText('12:10')).toBeTruthy();
    const lines = container.querySelectorAll<HTMLElement>('[class*="now "], [class$="now"]');
    expect(lines).toHaveLength(4);
    expect(Number(styleVar(must(lines[0]), '--simdi-ust'))).toBeCloseTo(3 + 10 / 60);
  });

  it('başka günde şu anki saat çizilmez', () => {
    render(<CalendarGrid {...day} now={new Date('2025-11-26T09:10:00Z')} />);
    expect(screen.queryByText('12:10')).toBeNull();
  });

  it('boş alana tıklama: personel ve 15 dakikalık UTC an', async () => {
    const onSlotClick = vi.fn();
    render(<CalendarGrid {...day} onSlotClick={onSlotClick} />);
    await userEvent.click(
      screen.getByRole('button', { name: 'Randevu ekle: Zeynep Aksoy, 14:15' }),
    );
    expect(onSlotClick).toHaveBeenCalledWith({
      staffId: 'zeynep',
      date: '2025-11-25',
      startUtc: '2025-11-25T11:15:00.000Z',
    });
  });

  it('bloğa tıklama randevu kimliğini verir', async () => {
    const onBlockClick = vi.fn();
    render(<CalendarGrid {...day} onBlockClick={onBlockClick} />);
    await userEvent.click(screen.getByRole('button', { name: /^Okan Er/ }));
    expect(onBlockClick).toHaveBeenCalledWith(
      'randevu-7',
      expect.objectContaining({ customerName: 'Okan Er' }),
    );
  });

  it('tıklama işleyicisi yokken boş dilimler buton değil, düz hücredir', () => {
    const { container } = render(<CalendarGrid {...day} onBlockClick={vi.fn()} />);
    expect(screen.queryByRole('button', { name: /Randevu ekle/ })).toBeNull();
    expect(container.querySelector('[data-slot]')).toBeNull();
    expect(container.querySelector('[aria-label^="Randevu ekle"]')).toBeNull();
    // Izgara ölçüsü değişmez: her kolonda 15 dakikalık hücreler durur.
    const columns = container.querySelectorAll('[data-column]');
    expect(columns).toHaveLength(4);
    const cells = must(columns[0]).querySelectorAll('[class*="cell"]');
    expect(cells.length).toBeGreaterThan(0);
    for (const cell of cells) {
      expect(cell.tagName).toBe('DIV');
      expect(cell.getAttribute('role')).toBeNull();
      expect(cell.getAttribute('tabindex')).toBeNull();
    }
    // Bloklar açılabilir kalır.
    expect(screen.getByRole('button', { name: /^Okan Er/ })).toBeTruthy();
  });

  it('işleyici varken hücre sayısı salt okunur ızgarayla aynıdır', () => {
    const readOnly = render(<CalendarGrid {...day} />);
    const cellCount = readOnly.container.querySelectorAll('[class*="cell"]').length;
    readOnly.unmount();
    const { container } = render(<CalendarGrid {...day} onSlotClick={vi.fn()} />);
    expect(container.querySelectorAll('[data-slot]')).toHaveLength(cellCount);
  });

  it('boş alanlar tek sekme durağı, oklarla gezilir', async () => {
    render(<CalendarGrid {...day} onSlotClick={vi.fn()} />);
    const first = screen.getByRole('button', { name: 'Randevu ekle: Elif Kaya, 09:00' });
    expect(first.tabIndex).toBe(0);
    expect(screen.getByRole('button', { name: 'Randevu ekle: Elif Kaya, 09:15' }).tabIndex).toBe(
      -1,
    );
    first.focus();
    await userEvent.keyboard('{ArrowDown}{ArrowRight}');
    const focused = screen.getByRole('button', { name: 'Randevu ekle: Mert Demir, 09:15' });
    expect(document.activeElement).toBe(focused);
    expect(focused.tabIndex).toBe(0);
  });

  it('gölgeli aralık etiketiyle çizilir', () => {
    render(
      <CalendarGrid
        {...day}
        unavailable={[
          {
            staffId: 'mert',
            startUtc: '2025-11-25T10:00:00Z',
            endUtc: '2025-11-25T11:00:00Z',
            label: 'Mola',
          },
        ]}
      />,
    );
    const shade = must(screen.getByText('Mola').parentElement);
    expect(styleVar(shade, '--blok-ust')).toBe('4');
    expect(styleVar(shade, '--blok-boy')).toBe('1');
  });

  it('Berlin yaz saati geçiş günü: tıklama ve konum duvar saatine göre', async () => {
    const onSlotClick = vi.fn();
    const block: CalendarBlock = {
      appointmentId: 'b1',
      itemId: 'b1',
      staffId: 'elif',
      startUtc: '2026-03-29T08:00:00Z',
      endUtc: '2026-03-29T09:00:00Z',
      status: 'onaylandi',
      customerName: 'Ayşe Yılmaz',
      serviceName: 'Saç kesimi',
    };
    render(
      <CalendarGrid
        {...day}
        date="2026-03-29"
        timeZone="Europe/Berlin"
        blocks={[block]}
        onSlotClick={onSlotClick}
      />,
    );
    const element = screen.getByRole('button', { name: /^Ayşe Yılmaz, Saç kesimi, 10:00–11:00/ });
    expect(styleVar(element, '--blok-ust')).toBe('1');
    await userEvent.click(screen.getByRole('button', { name: 'Randevu ekle: Elif Kaya, 10:00' }));
    expect(onSlotClick).toHaveBeenCalledWith(
      expect.objectContaining({ startUtc: '2026-03-29T08:00:00.000Z' }),
    );
  });

  it('onMove verilince bloklar taşınabilir rol açıklaması taşır', () => {
    const { rerender } = render(<CalendarGrid {...day} />);
    const block = () => screen.getByRole('button', { name: /^Okan Er/ });
    expect(block().getAttribute('aria-roledescription')).toBeNull();
    rerender(<CalendarGrid {...day} onMove={vi.fn()} />);
    expect(block().getAttribute('aria-roledescription')).toBe('taşınabilir randevu');
  });
});

describe('sürükle-bırak (klavye)', () => {
  // jsdom ölçü yapmaz; kolonlara sabit dikdörtgen verilir: her kolon 200 px, ızgara 640 px (10 saat).
  function mockLayout(container: HTMLElement) {
    const columns = [...container.querySelectorAll<HTMLElement>('[data-column]')];
    columns.forEach((column, index) => {
      const rect = { left: 56 + index * 200, top: 0, width: 200, height: 640 };
      column.getBoundingClientRect = () => ({
        ...rect,
        x: rect.left,
        y: rect.top,
        right: rect.left + rect.width,
        bottom: rect.top + rect.height,
        toJSON: () => rect,
      });
      for (const block of column.querySelectorAll<HTMLElement>('[data-status]')) {
        const top = Number(styleVar(block, '--blok-ust')) * 64;
        const height = Number(styleVar(block, '--blok-boy')) * 64;
        const r = { left: rect.left + 4, top, width: 192, height };
        block.getBoundingClientRect = () => ({
          ...r,
          x: r.left,
          y: r.top,
          right: r.left + r.width,
          bottom: r.top + r.height,
          toJSON: () => r,
        });
      }
    });
  }

  // Sürükleme hayaleti (DragOverlay) ve diğer ölçülmeyen öğeler kaynak bloğun yerinde varsayılır.
  function mockDefaultRect() {
    const rect = { left: 60, top: 256, width: 192, height: 48 };
    return vi.spyOn(HTMLElement.prototype, 'getBoundingClientRect').mockImplementation(() => ({
      ...rect,
      x: rect.left,
      y: rect.top,
      right: rect.left + rect.width,
      bottom: rect.top + rect.height,
      toJSON: () => rect,
    }));
  }

  afterEach(() => {
    vi.restoreAllMocks();
  });

  async function press(code: string) {
    await act(async () => {
      fireEvent.keyDown(document.activeElement ?? document.body, { code, key: code });
      await new Promise((resolve) => setTimeout(resolve, 0));
    });
  }

  it('boşluk ile al, sağ ok kolon, aşağı ok 15 dk, boşluk ile bırak → onMove', async () => {
    const onMove = vi.fn();
    const describeDrop = vi.fn(
      (t: { staffId: string; startTime: string; endTime: string }) =>
        `${t.staffId} · ${t.startTime}–${t.endTime} · müşteriye SMS gider`,
    );
    mockDefaultRect();
    const { container } = render(
      <CalendarGrid {...day} onMove={onMove} describeDrop={describeDrop} />,
    );
    mockLayout(container);
    screen.getByRole('button', { name: /^Derya Şahin/ }).focus();
    await press('Space');
    await press('ArrowRight');
    await press('ArrowRight');
    for (let i = 0; i < 4; i += 1) {
      await press('ArrowDown');
    }

    // İpucu hedef kolonda görünür ve ekran okuyucuya duyurulur.
    expect(
      screen.getAllByText('zeynep · 14:00–14:45 · müşteriye SMS gider').length,
    ).toBeGreaterThan(0);
    await press('Space');
    expect(onMove).toHaveBeenCalledWith({
      appointmentId: 'randevu-3',
      itemId: 'kalem-3',
      staffId: 'zeynep',
      startUtc: '2025-11-25T11:00:00.000Z',
    });
  });

  it('Escape ile vazgeçilir, onMove çağrılmaz; veri değişmediği için blok yerinde kalır', async () => {
    const onMove = vi.fn();
    mockDefaultRect();
    const { container } = render(<CalendarGrid {...day} onMove={onMove} />);
    mockLayout(container);
    screen.getByRole('button', { name: /^Derya Şahin/ }).focus();
    await press('Space');
    await press('ArrowDown');
    await press('Escape');
    expect(onMove).not.toHaveBeenCalled();
    expect(styleVar(screen.getByRole('button', { name: /^Derya Şahin/ }), '--blok-ust')).toBe('4');
  });
});

describe('WeekCalendar (hafta)', () => {
  const week = {
    staff: { id: 'elif', name: 'Elif Kaya' },
    days: previewWeekDays,
    timeZone: previewTimeZone,
    openTime: '09:00',
    closeTime: '19:00',
    blocks: previewWeekBlocks,
  };

  it('gün başlıkları, bugün işaretli, çizgi yalnız bugünde', () => {
    const { container } = render(<WeekCalendar {...week} now={previewNow} />);
    expect(screen.getByRole('region', { name: 'Elif Kaya haftalık takvim' })).toBeTruthy();
    expect(screen.getByText('Pzt')).toBeTruthy();
    expect(screen.getByText('Salı, 25 Kasım, bugün')).toBeTruthy();
    const lines = container.querySelectorAll('[class*="now "], [class$="now"]');
    expect(lines).toHaveLength(1);
    expect(screen.queryByText('12:10')).toBeNull();
  });

  it('blok kendi gününde; boş alan tıklaması o günün anını verir', async () => {
    const onSlotClick = vi.fn();
    const { container } = render(<WeekCalendar {...week} onSlotClick={onSlotClick} />);
    const thursday = must(container.querySelector<HTMLElement>('[data-column="2025-11-27"]'));
    expect(
      within(thursday).getByRole('button', { name: /^Aslı Kurt, Balyaj, 13:00–14:30/ }),
    ).toBeTruthy();
    await userEvent.click(
      screen.getByRole('button', { name: 'Randevu ekle: Cumartesi, 29 Kasım, 15:30' }),
    );
    expect(onSlotClick).toHaveBeenCalledWith({
      staffId: 'elif',
      date: '2025-11-29',
      startUtc: '2025-11-29T12:30:00.000Z',
    });
  });
});

describe('AppointmentBlock', () => {
  const base: CalendarBlock = {
    appointmentId: 'a',
    itemId: 'a',
    staffId: 'elif',
    startUtc: '2025-11-25T08:00:00Z',
    endUtc: '2025-11-25T08:30:00Z',
    status: 'onay',
    customerName: 'Deniz Acar',
    serviceName: 'Saç kesimi',
  };

  it('45 dakikadan kısa blok tek satır (hizmet satırı yok)', () => {
    const { container } = render(<AppointmentBlock block={base} timeZone={previewTimeZone} />);
    expect(container.querySelector('[class*="compact"]')).toBeTruthy();
    expect(screen.queryByText('Saç kesimi')).toBeNull();
  });

  it('Onaylandı dışındaki durum adıyla yazılır, iptal üstü çizili sınıf alır', () => {
    render(
      <AppointmentBlock
        block={{ ...base, endUtc: '2025-11-25T09:00:00Z', status: 'iptal' }}
        timeZone={previewTimeZone}
      />,
    );
    const button = screen.getByRole('button', { name: /İptal$/ });
    expect(within(button).getByText('İptal')).toBeTruthy();
    expect(button.className).toContain('iptal');
  });

  it('işaret (note) durumun yanında ve erişilebilir adda; kısa blokta da görünür', () => {
    const block: CalendarBlock = { ...base, status: 'kapora', note: 'Havale bildirildi' };
    const { rerender } = render(<AppointmentBlock block={block} timeZone={previewTimeZone} />);
    const compact = screen.getByRole('button', { name: /Kapora bekleniyor, Havale bildirildi$/ });
    expect(within(compact).getByText('Havale bildirildi')).toBeTruthy();
    rerender(
      <AppointmentBlock
        block={{ ...block, endUtc: '2025-11-25T09:00:00Z' }}
        timeZone={previewTimeZone}
      />,
    );
    const full = screen.getByRole('button', { name: /Havale bildirildi$/ });
    expect(within(full).getByText('Kapora bekleniyor')).toBeTruthy();
    expect(within(full).getByText('Havale bildirildi')).toBeTruthy();
  });
});

describe('Gün bildirimleri', () => {
  it('kapalı gün: başlık, açıklama ve eylem yuvası', () => {
    render(<CalendarClosedDay actions={<button type="button">Yalnızca bu gün için aç</button>} />);
    expect(screen.getByRole('heading', { name: 'Kapalı gün' })).toBeTruthy();
    expect(screen.getByText(/Bu güne randevu alınmaz/)).toBeTruthy();
    expect(screen.getByRole('button', { name: 'Yalnızca bu gün için aç' })).toBeTruthy();
    expect(screen.queryByRole('button', { name: /Randevu ekle/ })).toBeNull();
  });

  it('çalışma saati olmayan gün', () => {
    render(
      <CalendarEmptyDay
        title="Pazartesi kapalısınız"
        description="Bu gün çalışma saati yok. Açık kalmak isterseniz özel gün olarak saat ekleyin."
      />,
    );
    expect(screen.getByRole('region', { name: 'Pazartesi kapalısınız' })).toBeTruthy();
    expect(screen.getByRole('heading', { name: 'Pazartesi kapalısınız' })).toBeTruthy();
  });
});
