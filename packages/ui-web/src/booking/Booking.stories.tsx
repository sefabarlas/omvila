import type { StoryDefault } from '@ladle/react';
import { ArrowLeft, CalendarPlus, MapPin, Package } from 'lucide-react';
import { useState } from 'react';
import { formatDate, formatDuration, formatIban, formatMoney } from '../format';
import { fixedClock } from '../time';
import {
  ANY_STAFF,
  AppointmentTicket,
  BookingButton,
  BookingSteps,
  branchOpenStatus,
  CategoryChips,
  CopyField,
  CopyList,
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
import { groupTitle, Phone, screenPad, sectionTitle, stack } from './preview';

// design/ekranlar/rezervasyon-sayfasi: Main, Personel, Saat, DoluGun, Randevum, Ertele, Havale, Masaustu.
export default { title: 'Rezervasyon/Ekranlar' } satisfies StoryDefault;

const tz = 'Europe/Istanbul';
// Salı 24 Kasım 2026, 12:00 İstanbul.
const clock = fixedClock('2026-11-24T09:00:00Z');
const today = '2026-11-24';
const tl = (lira: number) => ({ amountMinor: lira * 100, currency: 'TRY' });

const categories = [
  { id: 'sac', name: 'Saç' },
  { id: 'renk', name: 'Renk' },
  { id: 'bakim', name: 'Bakım' },
];

const services = [
  { id: 'kesim', categoryId: 'sac', name: 'Saç kesimi', durationMinutes: 45, price: tl(450) },
  { id: 'fon', categoryId: 'sac', name: 'Fön', durationMinutes: 30, price: tl(250) },
  {
    id: 'bakim',
    categoryId: 'sac',
    name: 'Saç bakımı',
    durationMinutes: 40,
    price: tl(600),
    description: 'Keratin maske',
  },
  {
    id: 'dip',
    categoryId: 'renk',
    name: 'Dip boya',
    durationMinutes: 90,
    price: tl(1100),
    priceDisplay: 'StartingFrom' as const,
    description: 'Uzunluğa göre değişir',
  },
  {
    id: 'balyaj',
    categoryId: 'renk',
    name: 'Balyaj',
    durationMinutes: 180,
    price: tl(2800),
    priceDisplay: 'StartingFrom' as const,
    depositHint: tl(300),
  },
  { id: 'manikur', categoryId: 'bakim', name: 'Manikür', durationMinutes: 40, price: tl(350) },
];

const staff = [
  { id: 'elif', name: 'Elif Kaya', title: 'Kurucu · son randevunuz onunla' },
  { id: 'zeynep', name: 'Zeynep Aksoy', title: 'Saç ve renk uzmanı', priceNote: '+50 ₺' },
  { id: 'mert', name: 'Mert Demir', title: 'Kesim ve sakal' },
];

const days: DayOption[] = [
  { date: '2026-11-24', available: true },
  { date: '2026-11-25', available: true },
  { date: '2026-11-26', available: false },
  { date: '2026-11-27', available: true },
  { date: '2026-11-28', available: true },
  { date: '2026-11-29', available: false, closed: true },
  { date: '2026-11-30', available: true },
  { date: '2026-12-01', available: true },
];

const slots: TimeSlot[] = [
  { time: '13:00', available: false },
  { time: '13:15', available: false },
  { time: '13:30', available: false },
  { time: '14:30', available: true },
  { time: '14:45', available: true },
  { time: '15:00', available: false },
  { time: '16:15', available: true },
  { time: '16:30', available: true },
  { time: '17:00', available: true },
  { time: '17:15', available: true },
  { time: '18:30', available: true },
  { time: '18:45', available: false },
];

const openStatus = branchOpenStatus({ openNow: true, todayClosesAt: '20:00', timeZone: tz }, clock);

function TopBar({ title }: { title: string }) {
  return (
    <div
      style={{
        display: 'grid',
        gridTemplateColumns: '44px 1fr 44px',
        alignItems: 'center',
        minHeight: 'var(--ust-bar)',
        padding: '0 var(--bosluk-2)',
      }}
    >
      <BookingButton variant="ghost" size="sm" aria-label="Geri" icon={<ArrowLeft size={22} />} />
      <span
        style={{
          textAlign: 'center',
          fontSize: 'var(--yazi-ust-bar-basligi-boyut)',
          fontWeight: 600,
        }}
      >
        {title}
      </span>
    </div>
  );
}

function ServicesStep({ variant = 'page' }: { variant?: 'page' | 'card' }) {
  const [category, setCategory] = useState<string | null>(null);
  const [selected, setSelected] = useState<string[]>(['kesim', 'fon']);
  const chosen = services.filter((s) => selected.includes(s.id));
  const total = chosen.reduce((sum, s) => sum + s.price.amountMinor, 0);
  const minutes = chosen.reduce((sum, s) => sum + s.durationMinutes, 0);
  const visible = categories.filter((c) => category === null || c.id === category);
  return (
    <>
      <div style={screenPad}>
        <BookingSteps current={0} />
        <h2 style={sectionTitle}>Hizmet seçin</h2>
        <CategoryChips categories={categories} value={category} onChange={setCategory} />
        {visible.map((c) => (
          <section key={c.id} style={stack} aria-label={c.name}>
            <h3 style={groupTitle}>{c.name}</h3>
            {services
              .filter((s) => s.categoryId === c.id)
              .map((s) => (
                <ServiceItem
                  key={s.id}
                  {...s}
                  checked={selected.includes(s.id)}
                  onCheckedChange={(on) => {
                    setSelected((prev) =>
                      on ? [...prev, s.id] : prev.filter((id) => id !== s.id),
                    );
                  }}
                />
              ))}
          </section>
        ))}
      </div>
      <StickyFooter
        layout={variant === 'card' ? 'inline' : 'sticky'}
        summary={formatMoney({ amountMinor: total, currency: 'TRY' })}
        detail={`${String(chosen.length)} hizmet · ${formatDuration(minutes)}`}
        action={<BookingButton disabled={chosen.length === 0}>Devam et</BookingButton>}
      />
    </>
  );
}

export const Hizmet = () => (
  <Phone>
    <SalonHeader
      name="Kuaför Elif"
      category="Kuaför · Saç, bakım, makyaj"
      rating={{ average: 4.9, count: 312 }}
      openStatus={openStatus}
      address="Moda Cad. 42, Kadıköy · İstanbul"
    />
    <ServicesStep />
  </Phone>
);

export const KapakRenkleri = () => (
  <Phone>
    <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--bosluk-4)' }}>
      {(['petrol', 'mercan', 'kum'] as const).map((color) => (
        <SalonHeader
          key={color}
          name="Kuaför Elif"
          coverColor={color}
          headingLevel={2}
          category="Kuaför · Saç, bakım, makyaj"
          openStatus={branchOpenStatus(
            { openNow: false, nextOpeningUtc: '2026-11-25T07:00:00Z', timeZone: tz },
            clock,
          )}
          address="Abdi İpekçi Cad. 18, Şişli"
        />
      ))}
    </div>
  </Phone>
);

export const Personel = () => {
  const [value, setValue] = useState<string | null>('elif');
  const [same, setSame] = useState(true);
  const chosen = staff.find((s) => s.id === value);
  return (
    <Phone>
      <TopBar title="Personel seçin" />
      <div style={screenPad}>
        <BookingSteps current={1} />
        <h2 style={sectionTitle}>Kimden olsun?</h2>
        <StaffPicker staff={staff} value={value} onChange={setValue}>
          <SwitchCard
            label="İki hizmet de aynı kişiden"
            description="Kapatırsanız her hizmete ayrı kişi seçersiniz"
            checked={same}
            onCheckedChange={setSame}
          />
        </StaffPicker>
      </div>
      <StickyFooter
        summary={value === ANY_STAFF ? 'Fark etmez' : chosen?.name}
        detail="700 ₺ · 1 sa 15 dk"
        action={<BookingButton>Devam et</BookingButton>}
      />
    </Phone>
  );
};

export const Saat = () => {
  const [day, setDay] = useState<string | null>(today);
  const [time, setTime] = useState<string | null>('14:30');
  return (
    <Phone>
      <TopBar title="Gün ve saat" />
      <div style={screenPad}>
        <BookingSteps current={2} />
        <DayStrip
          days={days}
          value={day}
          today={today}
          onChange={setDay}
          hint="60 gün ileriye kadar"
        />
        <TimeSlotGrid slots={slots} value={time} onChange={setTime} headingLevel={3} />
      </div>
      <StickyFooter
        summary="750 ₺"
        detail={day && time ? `${formatDate(day, 'compact')} · ${time}–15:45` : undefined}
        action={<BookingButton disabled={!time}>Devam et</BookingButton>}
      />
    </Phone>
  );
};

export const DoluGun = () => {
  const [day, setDay] = useState<string | null>('2026-11-26');
  return (
    <Phone>
      <TopBar title="Gün ve saat" />
      <div style={screenPad}>
        <DayStrip days={days} value={day} today={today} onChange={setDay} showMonth={false} />
        <TimeSlotGrid
          slots={slots.map((s) => ({ ...s, available: false }))}
          value={null}
          onChange={() => undefined}
          empty={
            <FullDayNotice
              title="Perşembe dolu"
              description="Elif Kaya'nın 26 Kasım'da boş saati yok. En yakın uygun saat Cuma 10:30."
              action={{ label: "Cuma 10:30'u seç", onClick: () => undefined }}
            />
          }
        />
      </div>
    </Phone>
  );
};

export const Ertele = () => {
  const [day, setDay] = useState<string | null>('2026-11-27');
  const [time, setTime] = useState<string | null>('15:15');
  return (
    <Phone>
      <div style={screenPad}>
        <h1 style={sectionTitle}>Yeni saat seçin</h1>
        <DayStrip days={days} value={day} today={today} onChange={setDay} showMonth={false} />
        <h2 style={groupTitle}>{day ? formatDate(day, 'weekdayDayMonth') : null}</h2>
        <TimeSlotGrid
          grouped={false}
          slots={[
            { time: '10:00', available: true },
            { time: '10:45', available: false },
            { time: '11:30', available: true },
            { time: '13:00', available: true },
            { time: '14:30', available: false },
            { time: '15:15', available: true },
            { time: '16:00', available: true },
            { time: '16:45', available: false },
          ]}
          value={time}
          onChange={setTime}
        />
      </div>
      <StickyFooter
        summary={day && time ? `${formatDate(day, 'compact')} · ${time}` : undefined}
        detail="16:30'da biter"
        action={<BookingButton>Saati değiştir</BookingButton>}
      />
    </Phone>
  );
};

export const Randevum = () => (
  <Phone>
    <AppointmentTicket
      salonName="Kuaför Elif"
      branchAddress="Moda Cad. 42, Kadıköy"
      status="onaylandi"
      startUtc="2026-11-24T11:30:00Z"
      timeZone={tz}
      serviceNames={['Saç kesimi', 'Fön']}
      staffName="Elif Kaya"
      durationMinutes={75}
      actions={
        <>
          <BookingButton variant="secondary" size="md" icon={<MapPin size={20} />}>
            Yol tarifi
          </BookingButton>
          <BookingButton variant="secondary" size="md" icon={<CalendarPlus size={20} />}>
            Takvime ekle
          </BookingButton>
        </>
      }
    />
    <div style={screenPad}>
      <BookingButton variant="danger" size="md" block>
        Randevuyu iptal et
      </BookingButton>
    </div>
  </Phone>
);

export const Havale = () => (
  <Phone>
    <div style={screenPad}>
      <h1 style={{ ...sectionTitle, fontSize: 'var(--yazi-baslik-1-boyut)' }}>
        Kaporayı havale ile gönderin
      </h1>
      <CopyList>
        <CopyField label="Alıcı" value="Elif Kaya" onCopy={() => undefined} />
        <CopyField
          label="IBAN"
          value="TR120006400000112345678901"
          displayValue={formatIban('TR120006400000112345678901')}
          onCopy={() => undefined}
        />
        <CopyField label="Açıklama kodu" value="OMV-7K2X" onCopy={() => undefined} />
        <CopyField
          label="Tutar"
          value="150"
          displayValue={formatMoney(tl(150))}
          onCopy={() => undefined}
        />
      </CopyList>
    </div>
    <StickyFooter
      action={<BookingButton>Havaleyi yaptım, salona bildir</BookingButton>}
      note="Bu bilgiler SMS ile de gönderildi."
    />
  </Phone>
);

export const Anahtarlar = () => {
  const [pkg, setPkg] = useState(true);
  const [other, setOther] = useState(false);
  return (
    <Phone>
      <div style={screenPad}>
        <SwitchCard
          label="Fön paketinizden düşülsün"
          description="10 seansın 7'si kaldı · son kullanım 30 Mart"
          icon={<Package size={20} />}
          checked={pkg}
          onCheckedChange={setPkg}
        />
        <SwitchCard
          label="Başka personel de olur"
          description="Elif Kaya dışında yer açılırsa da haber ver"
          checked={other}
          onCheckedChange={setOther}
        />
        <SwitchCard
          label="İki hizmet de aynı kişiden"
          description="Saç kesimi, Fön"
          checked
          disabled
          onCheckedChange={() => undefined}
        />
      </div>
    </Phone>
  );
};

export const Masaustu = () => (
  <div
    style={{
      display: 'grid',
      gridTemplateColumns: 'minmax(0, 380px) minmax(0, 680px)',
      gap: 'var(--bosluk-8)',
      justifyContent: 'center',
      padding: 'var(--bosluk-8) var(--bosluk-4)',
      background: 'var(--zemin-ikincil)',
      minHeight: '100vh',
      alignItems: 'start',
    }}
  >
    <SalonHeader
      variant="card"
      name="Kuaför Elif"
      category="Kuaför · Saç, bakım, makyaj"
      rating={{ average: 4.9, count: 312 }}
      openStatus={openStatus}
      address="Moda Cad. 42, Kadıköy · İstanbul"
    />
    <div>
      <ServicesStep variant="card" />
    </div>
  </div>
);
