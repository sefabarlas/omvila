import type { StoryDefault } from '@ladle/react';
import { CalendarPlus, Settings } from 'lucide-react';
import { useState } from 'react';
import { formatDate } from '../format';
import { Button } from '../panel/Button';
import {
  previewBlocks,
  previewDate,
  previewNow,
  previewStaff,
  previewTimeZone,
  previewWeekBlocks,
  previewWeekDays,
} from './preview-data';
import { CalendarClosedDay, CalendarEmptyDay, CalendarGrid, WeekCalendar } from './index';
import type { CalendarBlock, CalendarDropTarget, CalendarMove } from './index';

export default { title: 'Takvim' } satisfies StoryDefault;

const hours = { openTime: '09:00', closeTime: '19:00', timeZone: previewTimeZone };

function staffName(id: string): string {
  return previewStaff.find((s) => s.id === id)?.name ?? '';
}

/** salon-paneli/TakvimSurukle: "Zeynep Aksoy'a taşı · 14:00–14:45 · müşteriye SMS gider". */
function describeDrop(target: CalendarDropTarget): string {
  const where = target.staffChanged ? `${staffName(target.staffId)}'a taşı` : 'Saati değiştir';
  return `${where} · ${target.startTime}–${target.endTime} · müşteriye SMS gider`;
}

function applyMove(blocks: CalendarBlock[], move: CalendarMove): CalendarBlock[] {
  return blocks.map((block) => {
    if (block.itemId !== move.itemId) {
      return block;
    }

    const shift = Date.parse(move.startUtc) - Date.parse(block.startUtc);
    const at = (iso: string) => new Date(Date.parse(iso) + shift).toISOString();
    return {
      ...block,
      staffId: move.staffId,
      startUtc: move.startUtc,
      endUtc: at(block.endUtc),
      ...(block.segments
        ? {
            segments: block.segments.map((s) => ({
              ...s,
              startUtc: at(s.startUtc),
              endUtc: at(s.endUtc),
            })),
          }
        : {}),
    };
  });
}

/** salon-paneli/Main.png: dört personel, şu anki saat 12:10, sürükle-bırak açık. */
export const Gun = () => {
  const [blocks, setBlocks] = useState(previewBlocks);
  const [last, setLast] = useState('');
  return (
    <div style={{ display: 'grid', gap: 'var(--bosluk-3)' }}>
      <CalendarGrid
        {...hours}
        date={previewDate}
        staff={previewStaff}
        blocks={blocks}
        now={previewNow}
        describeDrop={describeDrop}
        onMove={(move) => {
          setBlocks((current) => applyMove(current, move));
        }}
        onSlotClick={(slot) => {
          setLast(`Yeni randevu: ${staffName(slot.staffId)} ${slot.startUtc}`);
        }}
        onBlockClick={(id) => {
          setLast(`Randevu: ${id}`);
        }}
      />
      <p role="status">{last}</p>
    </div>
  );
};

/** 409 slot_taken benzetimi: onMove veriyi değiştirmez, blok eski yerinde kalır. */
export const GunTasimaReddedilir = () => (
  <CalendarGrid
    {...hours}
    date={previewDate}
    staff={previewStaff}
    blocks={previewBlocks}
    now={previewNow}
    describeDrop={describeDrop}
    onMove={() => undefined}
  />
);

export const GunMolaIle = () => (
  <CalendarGrid
    {...hours}
    date={previewDate}
    staff={previewStaff}
    blocks={previewBlocks}
    unavailable={[
      {
        staffId: 'mert',
        startUtc: '2025-11-25T10:00:00Z',
        endUtc: '2025-11-25T11:00:00Z',
        label: 'Mola',
      },
      {
        staffId: 'zeynep',
        startUtc: '2025-11-25T14:00:00Z',
        endUtc: '2025-11-25T16:00:00Z',
        label: 'İzin',
      },
    ]}
  />
);

/** salon-paneli/HaftaTakvim.png: Elif Kaya, 24–29 Kasım. */
export const Hafta = () => {
  const [blocks, setBlocks] = useState(previewWeekBlocks);
  return (
    <WeekCalendar
      {...hours}
      staff={{ id: 'elif', name: 'Elif Kaya' }}
      days={previewWeekDays}
      blocks={blocks}
      now={previewNow}
      describeDrop={(t) =>
        `${formatDate(t.date, 'panel')} · ${t.startTime}–${t.endTime} · müşteriye SMS gider`
      }
      onMove={(move) => {
        setBlocks((current) => applyMove(current, move));
      }}
    />
  );
};

/** salon-paneli/TakvimKapali.png. */
export const KapaliGun = () => (
  <CalendarClosedDay
    actions={
      <>
        <Button variant="primary" icon={<CalendarPlus size={16} />}>
          Yalnızca bu gün için aç
        </Button>
        <Button icon={<Settings size={16} />}>Çalışma saatleri</Button>
      </>
    }
  />
);

/** salon-paneli/TakvimBos.png. */
export const CalismaSaatiYok = () => (
  <CalendarEmptyDay
    title="Pazartesi kapalısınız"
    description="Bu gün çalışma saati yok. Açık kalmak isterseniz özel gün olarak saat ekleyin."
    actions={
      <>
        <Button>Özel saat ekle</Button>
        <Button variant="primary">Salıya git</Button>
      </>
    }
  />
);
