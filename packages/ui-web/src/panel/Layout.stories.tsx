import type { StoryDefault } from '@ladle/react';
import {
  BarChart3,
  Calendar,
  LayoutGrid,
  Package,
  Plus,
  Scissors,
  Settings,
  Users,
  Wallet,
} from 'lucide-react';
import { useState } from 'react';
import { Badge } from './Badge';
import { Button } from './Button';
import { DataTable, PersonCell } from './DataTable';
import { Sidebar } from './Sidebar';
import type { SidebarGroup, ThemePreference } from './Sidebar';
import { Bento, BentoBlock, StatCard } from './StatCard';
import { Topbar } from './Topbar';

export default { title: 'Panel/Sayfa' } satisfies StoryDefault;

const groups: SidebarGroup[] = [
  {
    title: 'İşletme',
    items: [
      { key: 'genel', label: 'Genel bakış', icon: <LayoutGrid size={18} />, href: '#genel' },
      { key: 'takvim', label: 'Takvim', icon: <Calendar size={18} />, href: '#takvim', count: 3 },
      { key: 'musteriler', label: 'Müşteriler', icon: <Users size={18} />, href: '#musteriler' },
      { key: 'kasa', label: 'Kasa', icon: <Wallet size={18} />, href: '#kasa' },
    ],
  },
  {
    title: 'Yönetim',
    items: [
      { key: 'hizmetler', label: 'Hizmetler', icon: <Scissors size={18} />, href: '#hizmetler' },
      { key: 'personel', label: 'Personel', icon: <Users size={18} />, href: '#personel' },
      { key: 'paketler', label: 'Paketler', icon: <Package size={18} />, href: '#paketler' },
      {
        key: 'raporlar',
        label: 'Raporlar ve prim',
        icon: <BarChart3 size={18} />,
        href: '#raporlar',
      },
      { key: 'ayarlar', label: 'Ayarlar', icon: <Settings size={18} />, href: '#ayarlar' },
    ],
  },
];

interface Row {
  id: string;
  name: string;
  phone: string;
  visits: number;
  spent: string;
  tag?: string;
}

const rows: Row[] = [
  {
    id: '1',
    name: 'Ayşe Yılmaz',
    phone: '0532 *** ** 12',
    visits: 14,
    spent: '6.300 ₺',
    tag: 'VIP',
  },
  { id: '2', name: 'Selin Öztürk', phone: '0533 *** ** 48', visits: 3, spent: '1.350 ₺' },
  {
    id: '3',
    name: 'Zeynep Arslan',
    phone: '0541 *** ** 07',
    visits: 1,
    spent: '450 ₺',
    tag: 'Yeni',
  },
];

export const Panel = () => {
  const [active, setActive] = useState('genel');
  const [branch, setBranch] = useState('kadikoy');
  const [selected, setSelected] = useState<Set<string>>(new Set());
  const [theme, setTheme] = useState<ThemePreference>('system');
  return (
    <div style={{ display: 'flex', minHeight: '720px', margin: 'calc(var(--bosluk-6) * -1)' }}>
      <Sidebar
        groups={groups}
        active={active}
        onNavigate={(item) => {
          setActive(item.key);
        }}
        branches={[
          { id: 'kadikoy', name: 'Kadıköy' },
          { id: 'nisantasi', name: 'Nişantaşı' },
        ]}
        branchId={branch}
        onBranchChange={setBranch}
        user={{ name: 'Elif Kaya', role: 'İşletme sahibi' }}
        theme={{ preference: theme, onChange: setTheme }}
        onSignOut={() => undefined}
      />
      <main style={{ flex: 1, minWidth: 0 }}>
        <Topbar
          title="Genel bakış"
          context="Salı, 25 Kasım · Kadıköy"
          search={{
            placeholder: 'Müşteri, telefon veya randevu ara',
            label: 'Ara',
            onSearch: () => undefined,
          }}
          notifications={{ count: 3, onOpen: () => undefined }}
          action={
            <Button variant="primary" icon={<Plus size={16} />}>
              Randevu ekle
            </Button>
          }
        />
        <div
          style={{
            display: 'flex',
            flexDirection: 'column',
            gap: 'var(--bosluk-5)',
            padding: '0 var(--bosluk-6) var(--bosluk-6)',
          }}
        >
          <Bento label="Bugün">
            <BentoBlock tone="marka" label="Bugünkü randevu" value="12" sub="İlki 09:30" />
            <BentoBlock
              tone="vurgu"
              label="Onay bekliyor"
              value="3"
              sub="En eskisi 40 dk önce"
              href="#onay"
            />
            <BentoBlock tone="kum" label="Boş saat" value="5" sub="Öğleden sonra" />
            <BentoBlock label="Bugünkü ciro" value="8.450 ₺" sub="Tahmini" />
          </Bento>
          <div
            style={{
              display: 'grid',
              gridTemplateColumns: 'repeat(3, minmax(0, 1fr))',
              gap: 'var(--bosluk-3)',
            }}
          >
            <StatCard
              label="Gelen"
              value="42"
              delta={{ text: '%12', direction: 'up', meaning: 'good' }}
              context="geçen salıya göre"
            />
            <StatCard
              label="Gelmeyen"
              value="4"
              delta={{ text: '2', direction: 'up', meaning: 'bad' }}
              context="geçen salıya göre"
            />
            <StatCard label="Yeni müşteri" value="—" context="Henüz veri yok" />
          </div>
          <DataTable
            caption="Müşteriler"
            rows={rows}
            rowKey={(row) => row.id}
            onRowClick={() => undefined}
            selection={{ selected, onChange: setSelected, rowLabel: (row) => `${row.name} seç` }}
            columns={[
              {
                key: 'name',
                label: 'Müşteri',
                render: (row) => <PersonCell name={row.name} detail={row.phone} />,
              },
              {
                key: 'tag',
                label: 'Etiket',
                render: (row) =>
                  row.tag ? (
                    <Badge tone={row.tag === 'VIP' ? 'bilgi' : 'vurgu'}>{row.tag}</Badge>
                  ) : null,
              },
              { key: 'visits', label: 'Ziyaret', numeric: true, render: (row) => row.visits },
              { key: 'spent', label: 'Harcama', numeric: true, render: (row) => row.spent },
            ]}
            footer={<span>3 müşteriden 1–3</span>}
          />
        </div>
      </main>
    </div>
  );
};
