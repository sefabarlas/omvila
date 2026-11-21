import { cleanup, render, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { useState } from 'react';
import { afterEach, describe, expect, it, vi } from 'vitest';
import { fixedClock } from '../time';
import {
  ANY_STAFF,
  AppointmentTicket,
  BookingButton,
  BookingStatusBadge,
  BookingSteps,
  branchOpenStatus,
  CategoryChips,
  CopyField,
  DayStrip,
  FullDayNotice,
  SalonHeader,
  ServiceItem,
  StaffPicker,
  StickyFooter,
  SwitchCard,
  TimeSlotGrid,
} from './index';
import type { DayOption, TimeSlot } from './index';

// Kural: packages/ui-web/AGENTS.md (erişilebilirlik, klavye), mobil tasarım sistemi README'leri ve
// design/ekranlar/rezervasyon-sayfasi metinleri.
afterEach(cleanup);

const tz = 'Europe/Istanbul';

describe('SalonHeader', () => {
  it('ad başlık, kategori, puan, açık durumu ve adres', () => {
    render(
      <SalonHeader
        name="Kuaför Elif"
        category="Kuaför · Saç, bakım, makyaj"
        rating={{ average: 4.9, count: 312 }}
        openStatus={{ open: true, label: "Şu an açık · 20:00'ye kadar" }}
        address="Moda Cad. 42, Kadıköy · İstanbul"
      />,
    );
    expect(screen.getByRole('heading', { level: 1, name: 'Kuaför Elif' })).toBeTruthy();
    expect(screen.getByText('Kuaför · Saç, bakım, makyaj')).toBeTruthy();
    expect(screen.getByText('Puan 5 üzerinden 4,9, 312 değerlendirme')).toBeTruthy();
    expect(screen.getByText("Şu an açık · 20:00'ye kadar")).toBeTruthy();
    expect(screen.getByText('Moda Cad. 42, Kadıköy · İstanbul')).toBeTruthy();
    expect(screen.getByText('KE')).toBeTruthy();
  });

  it('puan yoksa satır gizlenir; kapak rengi sınıfı', () => {
    const { container } = render(<SalonHeader name="Kuaför Elif" coverColor="mercan" />);
    expect(screen.queryByText(/değerlendirme/)).toBeNull();
    expect(container.querySelector('.mercan')).toBeTruthy();
  });
});

describe('branchOpenStatus', () => {
  // 2026-11-24 Salı 12:00 İstanbul.
  const clock = fixedClock('2026-11-24T09:00:00Z');

  it('açık: kapanış saatiyle', () => {
    expect(
      branchOpenStatus({ openNow: true, todayClosesAt: '20:00', timeZone: tz }, clock),
    ).toEqual({ open: true, label: "Şu an açık · 20:00'ye kadar" });
  });

  it('kapalı: yarın, bugün ve gün adıyla', () => {
    expect(
      branchOpenStatus(
        { openNow: false, nextOpeningUtc: '2026-11-25T07:00:00Z', timeZone: tz },
        clock,
      ).label,
    ).toBe("Bugün kapalı · Yarın 10:00'da açılır");
    expect(
      branchOpenStatus(
        { openNow: false, nextOpeningUtc: '2026-11-24T10:00:00Z', timeZone: tz },
        clock,
      ).label,
    ).toBe("Şu an kapalı · Bugün 13:00'te açılır");
    expect(
      branchOpenStatus(
        { openNow: false, nextOpeningUtc: '2026-11-27T06:30:00Z', timeZone: tz },
        clock,
      ).label,
    ).toBe("Bugün kapalı · Cuma 09:30'da açılır");
    expect(branchOpenStatus({ openNow: false, timeZone: tz }, clock)).toEqual({
      open: false,
      label: 'Şu an kapalı',
    });
  });
});

describe('BookingSteps', () => {
  it('geçerli adım aria-current="step", tamamlananlar okunur', () => {
    render(<BookingSteps current={2} />);
    const nav = screen.getByRole('navigation', { name: /adım 3 \/ 4/ });
    const items = within(nav).getAllByRole('listitem');
    expect(items).toHaveLength(4);
    expect(items[2]?.getAttribute('aria-current')).toBe('step');
    expect(items[0]?.textContent).toBe('Hizmet, tamamlandı');
    expect(items[3]?.getAttribute('aria-current')).toBeNull();
  });

  it('personel adımı kapalıysa üç adım', () => {
    render(<BookingSteps steps={['Hizmet', 'Saat', 'Onay']} current={0} />);
    expect(screen.getAllByRole('listitem')).toHaveLength(3);
  });
});

describe('ServiceItem', () => {
  function Harness({ onChange }: { onChange: (v: boolean) => void }) {
    const [checked, setChecked] = useState(false);
    return (
      <ServiceItem
        name="Balyaj"
        durationMinutes={180}
        price={{ amountMinor: 280000, currency: 'TRY' }}
        priceDisplay="StartingFrom"
        depositHint={{ amountMinor: 30000, currency: 'TRY' }}
        checked={checked}
        onCheckedChange={(v) => {
          setChecked(v);
          onChange(v);
        }}
      />
    );
  }

  it('onay kutusu: ad, açıklama (süre, kapora, fiyat), tıklama ve Boşluk', async () => {
    const onChange = vi.fn();
    render(<Harness onChange={onChange} />);
    const box = screen.getByRole('checkbox', { name: 'Balyaj' });
    expect(box.getAttribute('aria-describedby')).toBeTruthy();
    expect(screen.getByText('3 sa · Kapora 300 ₺')).toBeTruthy();
    expect(screen.getByText("2.800 ₺'den")).toBeTruthy();
    await userEvent.click(screen.getByText('Balyaj'));
    expect(onChange).toHaveBeenLastCalledWith(true);
    expect((box as HTMLInputElement).checked).toBe(true);
    await userEvent.keyboard(' ');
    expect(onChange).toHaveBeenLastCalledWith(false);
  });

  it('fiyat gizliyse gösterilmez; açıklama süreden sonra', () => {
    render(
      <ServiceItem
        name="Dip boya"
        durationMinutes={90}
        description="Uzunluğa göre değişir"
        price={null}
        checked={false}
        onCheckedChange={() => undefined}
      />,
    );
    expect(screen.getByText('1 sa 30 dk · Uzunluğa göre değişir')).toBeTruthy();
    expect(screen.queryByText(/₺/)).toBeNull();
  });

  it('devre dışı', () => {
    render(
      <ServiceItem
        name="Fön"
        durationMinutes={30}
        checked={false}
        disabled
        onCheckedChange={() => undefined}
      />,
    );
    expect(screen.getByRole<HTMLInputElement>('checkbox', { name: 'Fön' }).disabled).toBe(true);
  });
});

describe('CategoryChips', () => {
  it('Tümü ilk ve seçili; seçim aria-pressed', async () => {
    const onChange = vi.fn();
    render(
      <CategoryChips
        categories={[
          { id: 'sac', name: 'Saç' },
          { id: 'renk', name: 'Renk' },
        ]}
        value={null}
        onChange={onChange}
      />,
    );
    const group = screen.getByRole('group', { name: 'Kategori' });
    const buttons = within(group).getAllByRole('button');
    expect(buttons[0]?.textContent).toBe('Tümü');
    expect(buttons[0]?.getAttribute('aria-pressed')).toBe('true');
    await userEvent.click(screen.getByRole('button', { name: 'Renk' }));
    expect(onChange).toHaveBeenCalledWith('renk');
  });
});

const staff = [
  { id: 'elif', name: 'Elif Kaya', title: 'Kurucu · son randevunuz onunla' },
  { id: 'zeynep', name: 'Zeynep Aksoy', title: 'Saç ve renk uzmanı', priceNote: '+50 ₺' },
  { id: 'mert', name: 'Mert Demir', title: 'Kesim ve sakal' },
];

function StaffHarness({ anyStaff = true }: { anyStaff?: boolean }) {
  const [value, setValue] = useState<string | null>('elif');
  return (
    <StaffPicker
      staff={staff}
      value={value}
      onChange={setValue}
      {...(anyStaff ? {} : { anyStaff: false as const })}
    >
      <SwitchCard
        label="İki hizmet de aynı kişiden"
        description="Kapatırsanız her hizmete ayrı kişi seçersiniz"
        checked
        onCheckedChange={() => undefined}
      />
    </StaffPicker>
  );
}

describe('StaffPicker', () => {
  it('radyo grubu, ilk seçenek "Fark etmez", tek sekme durağı', () => {
    render(<StaffHarness />);
    const group = screen.getByRole('radiogroup', { name: 'Personel' });
    const radios = within(group).getAllByRole('radio');
    expect(radios).toHaveLength(4);
    expect(radios[0]?.textContent).toContain('Fark etmez');
    expect(radios[0]?.textContent).toContain('En erken uygun saat');
    expect(radios[1]?.getAttribute('aria-checked')).toBe('true');
    expect(radios.map((r) => r.tabIndex)).toEqual([-1, 0, -1, -1]);
    expect(screen.getByText('+50 ₺')).toBeTruthy();
    expect(screen.getByRole('switch', { name: 'İki hizmet de aynı kişiden' })).toBeTruthy();
  });

  it('oklar seçer ve odağı taşır; sondan başa döner', async () => {
    render(<StaffHarness />);
    const elif = screen.getByRole('radio', { name: /Elif Kaya/ });
    elif.focus();
    await userEvent.keyboard('{ArrowDown}');
    const zeynep = screen.getByRole('radio', { name: /Zeynep Aksoy/ });
    expect(zeynep.getAttribute('aria-checked')).toBe('true');
    expect(document.activeElement).toBe(zeynep);
    await userEvent.keyboard('{End}{ArrowRight}');
    const any = screen.getByRole('radio', { name: /Fark etmez/ });
    expect(any.getAttribute('aria-checked')).toBe('true');
    expect(ANY_STAFF).toBe('any');
  });

  it('"Fark etmez" kapalıysa gösterilmez', () => {
    render(<StaffHarness anyStaff={false} />);
    expect(screen.queryByRole('radio', { name: /Fark etmez/ })).toBeNull();
    expect(screen.getAllByRole('radio')).toHaveLength(3);
  });
});

describe('SwitchCard', () => {
  it('anahtar rolü, açıklama okunur, değişim bildirilir', async () => {
    const onChange = vi.fn();
    render(
      <SwitchCard
        label="İki hizmet de aynı kişiden"
        description="Saç kesimi, Fön"
        checked={false}
        onCheckedChange={onChange}
      />,
    );
    const toggle = screen.getByRole('switch', { name: 'İki hizmet de aynı kişiden' });
    expect(toggle.getAttribute('aria-describedby')).toBeTruthy();
    await userEvent.click(toggle);
    expect(onChange).toHaveBeenCalledWith(true);
  });
});

// 2026-11-24 Salı … 2026-11-29 Pazar.
const days: DayOption[] = [
  { date: '2026-11-24', available: true },
  { date: '2026-11-25', available: true },
  { date: '2026-11-26', available: false },
  { date: '2026-11-27', available: true },
  { date: '2026-11-28', available: true },
  { date: '2026-11-29', available: false, closed: true },
  { date: '2026-11-30', available: true },
];

function DayHarness({ onChange }: { onChange?: (d: string) => void }) {
  const [value, setValue] = useState<string | null>('2026-11-24');
  return (
    <DayStrip
      days={days}
      value={value}
      today="2026-11-24"
      hint="60 gün ileriye kadar"
      onChange={(d) => {
        setValue(d);
        onChange?.(d);
      }}
    />
  );
}

describe('DayStrip', () => {
  it('ay başlığı, not, "Bugün", kapalı gün devre dışı', () => {
    render(<DayHarness />);
    expect(screen.getByRole('heading', { name: 'Kasım 2026' })).toBeTruthy();
    expect(screen.getByText('60 gün ileriye kadar')).toBeTruthy();
    const group = screen.getByRole('radiogroup', { name: 'Gün' });
    const today = within(group).getByRole('radio', { name: '24 Kasım Salı, bugün' });
    expect(today.textContent).toContain('Bugün');
    expect(today.getAttribute('aria-checked')).toBe('true');
    const closed = within(group).getByRole('radio', { name: '29 Kasım Pazar, kapalı' });
    expect((closed as HTMLButtonElement).disabled).toBe(true);
    expect(closed.textContent).toContain('Kapalı');
    expect(
      within(group).getByRole('radio', { name: '26 Kasım Perşembe, boş saat yok' }),
    ).toBeTruthy();
  });

  it('oklar kapalı günü atlar; Home ilk güne döner', async () => {
    const onChange = vi.fn();
    render(<DayHarness onChange={onChange} />);
    screen.getByRole('radio', { name: /24 Kasım/ }).focus();
    await userEvent.keyboard('{ArrowRight}');
    expect(onChange).toHaveBeenLastCalledWith('2026-11-25');
    await userEvent.click(screen.getByRole('radio', { name: /28 Kasım/ }));
    await userEvent.keyboard('{ArrowRight}');
    expect(onChange).toHaveBeenLastCalledWith('2026-11-30');
    await userEvent.keyboard('{Home}');
    expect(onChange).toHaveBeenLastCalledWith('2026-11-24');
    expect(document.activeElement?.getAttribute('aria-label')).toBe('24 Kasım Salı, bugün');
  });

  it('ay başlığı gizlenebilir (Ertele)', () => {
    render(<DayStrip days={days} value={null} onChange={() => undefined} showMonth={false} />);
    expect(screen.queryByRole('heading')).toBeNull();
    // Seçim yoksa ilk etkin gün sekme durağıdır.
    expect(screen.getAllByRole('radio')[0]?.tabIndex).toBe(0);
  });
});

const slots: TimeSlot[] = [
  { time: '10:00', available: true },
  { time: '13:00', available: false },
  { time: '14:30', available: true },
  { time: '15:00', available: false },
  { time: '16:15', available: true },
  { time: '17:00', available: true },
  { time: '18:45', available: false },
];

function SlotHarness({ onChange }: { onChange?: (t: string) => void }) {
  const [value, setValue] = useState<string | null>('14:30');
  return (
    <TimeSlotGrid
      slots={slots}
      value={value}
      onChange={(t) => {
        setValue(t);
        onChange?.(t);
      }}
    />
  );
}

describe('TimeSlotGrid', () => {
  it('Sabah / Öğleden sonra / Akşam grupları; dolu saat devre dışı ve "dolu" okunur', () => {
    render(<SlotHarness />);
    expect(screen.getByRole('radiogroup', { name: 'Saat' })).toBeTruthy();
    const afternoon = screen.getByRole('group', { name: 'Öğleden sonra' });
    expect(
      within(afternoon)
        .getAllByRole('radio')
        .map((r) => r.textContent),
    ).toEqual(['13:00', '14:30', '15:00', '16:15']);
    expect(screen.getByRole('group', { name: 'Sabah' })).toBeTruthy();
    expect(screen.getByRole('group', { name: 'Akşam' })).toBeTruthy();
    const full = screen.getByRole<HTMLButtonElement>('radio', { name: '13:00, dolu' });
    expect(full.disabled).toBe(true);
    expect(screen.getByRole('radio', { name: '14:30' }).getAttribute('aria-checked')).toBe('true');
  });

  it('oklar dolu saatleri atlar, gruplar arasında geçer', async () => {
    const onChange = vi.fn();
    render(<SlotHarness onChange={onChange} />);
    screen.getByRole('radio', { name: '14:30' }).focus();
    await userEvent.keyboard('{ArrowRight}');
    expect(onChange).toHaveBeenLastCalledWith('16:15');
    await userEvent.keyboard('{ArrowDown}');
    expect(onChange).toHaveBeenLastCalledWith('17:00');
    await userEvent.keyboard('{ArrowLeft}{ArrowLeft}{ArrowLeft}');
    expect(onChange).toHaveBeenLastCalledWith('10:00');
  });

  it('uygun saat yoksa boş durum; aksiyon çalışır', async () => {
    const onAction = vi.fn();
    render(
      <TimeSlotGrid
        slots={[{ time: '10:00', available: false }]}
        value={null}
        onChange={() => undefined}
        empty={
          <FullDayNotice
            title="Perşembe dolu"
            description="Elif Kaya'nın 26 Kasım'da boş saati yok. En yakın uygun saat Cuma 10:30."
            action={{ label: "Cuma 10:30'u seç", onClick: onAction }}
          />
        }
      />,
    );
    expect(screen.queryByRole('radiogroup')).toBeNull();
    expect(screen.getByRole('heading', { name: 'Perşembe dolu' })).toBeTruthy();
    await userEvent.click(screen.getByRole('button', { name: "Cuma 10:30'u seç" }));
    expect(onAction).toHaveBeenCalledOnce();
  });

  it('grupsuz tek ızgara (Ertele)', () => {
    render(<TimeSlotGrid slots={slots} value={null} grouped={false} onChange={() => undefined} />);
    expect(screen.queryByRole('group')).toBeNull();
    expect(screen.getAllByRole('radio')).toHaveLength(7);
  });
});

describe('StickyFooter', () => {
  it('özet canlı bölge, ana eylem', async () => {
    const onClick = vi.fn();
    const { container } = render(
      <StickyFooter
        summary="700 ₺"
        detail="2 hizmet · 1 sa 15 dk"
        action={<BookingButton onClick={onClick}>Devam et</BookingButton>}
      />,
    );
    const live = container.querySelector('[aria-live="polite"]');
    expect(live?.textContent).toBe('700 ₺2 hizmet · 1 sa 15 dk');
    await userEvent.click(screen.getByRole('button', { name: 'Devam et' }));
    expect(onClick).toHaveBeenCalledOnce();
  });

  it('BookingButton yüklenirken tıklama yok sayılır', async () => {
    const onClick = vi.fn();
    render(
      <BookingButton loading loadingLabel="Oluşturuluyor…" onClick={onClick}>
        Randevuyu oluştur
      </BookingButton>,
    );
    const button = screen.getByRole('button', { name: 'Oluşturuluyor…' });
    expect(button.getAttribute('aria-busy')).toBe('true');
    await userEvent.click(button);
    expect(onClick).not.toHaveBeenCalled();
  });
});

describe('BookingStatusBadge', () => {
  it('durum adı sabit etiketle yazılır, renk yalnız ek bilgi', () => {
    const { container } = render(<BookingStatusBadge status="onay" />);
    expect(screen.getByText('Onay bekliyor')).toBeTruthy();
    expect(container.querySelector('[aria-hidden="true"]')).toBeTruthy();
  });

  it('iptal "İptal" yazar', () => {
    render(<BookingStatusBadge status="iptal" />);
    expect(screen.getByText('İptal')).toBeTruthy();
  });
});

describe('AppointmentTicket', () => {
  it('durum satırı, şube saatinde büyük tarih/saat, ayrıntı ve eylemler', () => {
    render(
      <AppointmentTicket
        salonName="Kuaför Elif"
        branchAddress="Moda Cad. 42, Kadıköy"
        status="onaylandi"
        startUtc="2026-11-24T11:30:00Z"
        timeZone={tz}
        serviceNames={['Saç kesimi', 'Fön']}
        staffName="Elif Kaya"
        durationMinutes={75}
        actions={<BookingButton variant="secondary">Takvime ekle</BookingButton>}
      />,
    );
    expect(screen.getByText('Randevunuz · Onaylandı')).toBeTruthy();
    expect(screen.getByRole('heading', { level: 1, name: 'Sal 24 Kas · 14:30' })).toBeTruthy();
    expect(screen.getByText('Saç kesimi, Fön · Elif Kaya · 1 sa 15 dk')).toBeTruthy();
    expect(screen.getByRole('button', { name: 'Takvime ekle' })).toBeTruthy();
  });

  it('gece yarısına yakın saat şubenin gününe düşer', () => {
    render(
      <AppointmentTicket
        salonName="Kuaför Elif"
        status="onay"
        startUtc="2026-11-24T21:30:00Z"
        timeZone={tz}
        serviceNames={['Fön']}
        durationMinutes={30}
      />,
    );
    expect(screen.getByRole('heading', { name: 'Çar 25 Kas · 00:30' })).toBeTruthy();
    expect(screen.getByText('Randevunuz · Onay bekliyor')).toBeTruthy();
  });
});

describe('CopyField', () => {
  it('değeri geri çağırımla verir, "Kopyalandı" gösterir ve bildirir', async () => {
    const onCopy = vi.fn(() => Promise.resolve());
    render(
      <CopyField
        label="IBAN"
        value="TR120006400000112345678901"
        displayValue="TR12 0006 4000 0011 2345 6789 01"
        onCopy={onCopy}
      />,
    );
    expect(screen.getByText('TR12 0006 4000 0011 2345 6789 01')).toBeTruthy();
    await userEvent.click(screen.getByRole('button', { name: 'Kopyala IBAN' }));
    expect(onCopy).toHaveBeenCalledWith('TR120006400000112345678901');
    expect(screen.getByRole('button', { name: 'Kopyalandı IBAN' })).toBeTruthy();
    expect(screen.getByRole('status').textContent).toBe('IBAN kopyalandı');
    await userEvent.tab();
    expect(screen.getByRole('button', { name: 'Kopyala IBAN' })).toBeTruthy();
  });

  it('kopyalama başarısızsa "Kopyalandı" gösterilmez', async () => {
    render(<CopyField label="Tutar" value="150" onCopy={() => false} />);
    await userEvent.click(screen.getByRole('button', { name: 'Kopyala Tutar' }));
    expect(screen.getByRole('button', { name: 'Kopyala Tutar' })).toBeTruthy();
    expect(screen.getByRole('status').textContent).toBe('');
  });
});
